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
using Serilog;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;

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
            IEnumerable<ImageModel> image;
            int pageSize = 20;
            try
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "desc" : "";
                ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "Date";
                var client = _httpClientFactory.CreateClient(Constants.ClientWithToken);
                //HTTP GET
                var searchmodel = new SearchImages
                {
                    Description = desc,
                    FileSize = FileSize,
                    FileType = filetype
                };
                var responseTask = await client.PostAsJsonAsync(Constants.Routes.SearchImages, searchmodel);

                var result = responseTask;
               
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
                        TempData["count"] = image.Count();
                    }
                }
                else
                {
                    image = Enumerable.Empty<ImageModel>();
                   // ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
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
                    case "desc":
                        image = image.OrderBy(s => s.Description);
                        break;
                    case "date":
                        image = image.OrderByDescending(s => s.CreatedAt);
                        break;
                    case "dateasc":
                        image = image.OrderBy(s => s.CreatedAt);
                        break;
                    case "size":
                        image = image.OrderByDescending(s => s.FileSize);
                        break;
                    case "sizeasc":
                        image = image.OrderBy(s => s.FileSize);
                        break;
                    default:
                        image = image.OrderByDescending(s => s.CreatedAt);
                        break;
                }       
                return View(PaginatedList<ImageModel>.Create(image, pageNumber ?? 1, pageSize));
            }
            catch(Exception ex)
            {
                Log.Error("An error occurred " + ex);
                image = Enumerable.Empty<ImageModel>();
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return View(PaginatedList<ImageModel>.Create(image, pageNumber ?? 1, pageSize));
            }        
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        protected IActionResult RedirectOrHome(string location = null)
        {
            if (string.IsNullOrWhiteSpace(location) || !Url.IsLocalUrl(location))
                return Redirect(Constants.URL.Default);

            location = Constants.URL.Default;
            return Redirect(location);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string location)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient(Constants.ClientNoToken);

                    var response = await httpClient.PostAsJsonAsync(Constants.Routes.Token, model);
                    var resp = await response.Content.ReadAsAsync<TokenModel>();
                    if (response.IsSuccessStatusCode && resp.Token != null)
                    {
                        var principal = await GetUserClaims(resp);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                            new AuthenticationProperties
                            {
                                IsPersistent = true,
                                AllowRefresh = true,
                                ExpiresUtc = resp.Expires,
                                IssuedUtc = DateTime.UtcNow
                            });
                        //if (string.IsNullOrWhiteSpace(location) || !Url.IsLocalUrl(location))
                        //    return Redirect(Constants.URL.Default);

                        //location = Constants.URL.Default;
                        //return Redirect("Home/Index");
                        return RedirectToAction("Index");
                        return Redirect(Constants.URL.Default);
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        var desc = resp.ShortDescription ?? "";
                        ModelState.AddModelError("", desc);
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    Log.Error("An Error occurred " + ex);
                    ModelState.AddModelError("", "An error occured during login progress.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please check your inputs");
                return View(model);
            }
            return Redirect(Constants.URL.Default);
            return RedirectToAction("Index");
        }
        private async Task<ClaimsPrincipal> GetUserClaims(TokenModel tokenModel)
        {
            var httpClient = _httpClientFactory.CreateClient(Constants.ClientNoToken);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenModel.Token);

            var responseMessage = await httpClient.GetAsync(Constants.Routes.AccountGetClaims);

            responseMessage.EnsureSuccessStatusCode();

            var responseText = await responseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<List<Claim>>(responseText, new ClaimConverter());

            var claims = response;

            claims.Add(new Claim("access_token", tokenModel.Token));
            claims.Add(new Claim("refresh_token", tokenModel.RefreshToken));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
