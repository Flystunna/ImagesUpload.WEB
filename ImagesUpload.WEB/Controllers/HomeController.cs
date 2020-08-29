using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImagesUpload.WEB.Models;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using ImagesUpload.WEB.Utils;

namespace ImagesUpload.WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<ImageController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<ImageController> logger, IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
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
                if(image.Count() > 0)
                {
                    foreach(var item in image)
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
