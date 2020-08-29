using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ImagesUpload.WEB.Models;
using ImagesUpload.WEB.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ImagesUpload.WEB.Controllers
{
    public class ImageController : Controller
    {
        private readonly ILogger<ImageController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public ImageController(ILogger<ImageController> logger, IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _logger = logger;
           _httpClientFactory = httpClientFactory;
            _env = env;
        }
       
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber, string desc, string filetype, long? FileSize)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            var client = _httpClientFactory.CreateClient(Constants.ClientNoToken);
            //HTTP GET
            var searchmodel = new SearchImages
            {
                Description = desc,
                FileSize = FileSize,
                FileType = filetype
            };
            var responseTask = await client.PostAsJsonAsync(Constants.Routes.SearchImages, searchmodel);

            var result = responseTask;
            IEnumerable<ImageModel> image;
            if (result.IsSuccessStatusCode)
            {
                var readTask = await result.Content.ReadAsAsync<IList<ImageModel>>();

                image = readTask;
                if (image.Count() > 0)
                {
                    foreach (var item in image)
                    {
                        var span = DateTime.Now - item.CreatedAt;
                        item.Span = String.Format("{0} days, {1} hours, {2} minutes, {3} seconds", span.Days, span.Hours, span.Minutes, span.Seconds);
                    }
                }
            }
            else
            {
                image = Enumerable.Empty<ImageModel>();
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                image = image.Where(s => s.Description.Contains(searchString)
                                       || s.FileType.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    image = image.OrderByDescending(s => s.Description);
                    break;
                case "Date":
                    image = image.OrderBy(s => s.CreatedAt);
                    break;
                case "Size":
                    image = image.OrderBy(s => s.FileSize);
                    break;
                default:
                    image = image.OrderByDescending(s => s.CreatedAt);
                    break;
            }

            int pageSize = 20;
            return View(PaginatedList<ImageModel>.Create(image, pageNumber ?? 1, pageSize));
        }
        public IActionResult ImageUpload()
        {
            return View();
        }

        [Route("PostImage")]
        [HttpPost]
        public async Task<IActionResult> PostImage(ImageModel model)
        {
            if(string.IsNullOrEmpty(model.Description) || string.IsNullOrWhiteSpace(model.Description))
            {
                TempData["error"] = "Image description is required";
                return RedirectToAction("ImageUpload");
            }
            var uploadss = Path.Combine(_env.WebRootPath, "images");
            if (model.Pictures != null)
            {
                foreach (var pics in model.Pictures)
                {
                    var maxSize = 500 * 1024;
                    if(pics.Length > maxSize)
                    {
                        TempData["error"] = "Image size restricted to 500KB";
                        return RedirectToAction("ImageUpload");
                    }
                    string fileExt = System.IO.Path.GetExtension(pics.FileName).ToLower();
                    if (pics.ContentType =="image/jpeg" || pics.ContentType == "image/png")
                    {
                        var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(pics.FileName).ToString();
                        
                        using (var fileStream = new FileStream(Path.Combine(uploadss, fileName), FileMode.Create))
                        {
                            await pics.CopyToAsync(fileStream);
                        }
                       // var picturepath = fileName;
                        //string filetype = pics.ContentType.ToLower();
                        long filesize = pics.Length/1024;
                        var desc = model.Description;

                        var modelPost = new ImageModel
                        {
                            FileSize = filesize,
                            Description = model.Description.Trim(),
                            FileType = pics.ContentType.ToLower(),
                            Path = fileName.Trim()
                        };

                        string fullpath = Path.Combine(uploadss, fileName);

                        var client = _httpClientFactory.CreateClient(Constants.ClientNoToken);

                        HttpResponseMessage response = await client.PostAsJsonAsync(Constants.Routes.PostImage, modelPost);

                        if (response.IsSuccessStatusCode)
                        {
                            // Get the URI of the created resource.  
                            var UrireturnUrl = response.Headers.Location;
                            var resp = await response.Content.ReadAsAsync<ImageModel>();
                            Console.WriteLine(UrireturnUrl);
                            TempData["success"] = "Image saved successfully";
                            return RedirectToAction("Index");                         
                        }                    
                    }
                    else
                    {
                        TempData["error"] = "Image type restricted to PNG or JPEG";
                        return RedirectToAction("ImageUpload");
                    }                       
                }
            }
            else
            {
                TempData["error"] = "Image is required";
                return RedirectToAction("ImageUpload");
            }
            TempData["error"] = "An error occurred. Kindly try again";
            return RedirectToAction("ImageUpload");
        }
    }
}
