using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using ImagesUpload.WEB.Models;
using ImagesUpload.WEB.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace ImagesUpload.WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger _logger;
        private IDistributedCache _cache;

        public AccountController(IHttpClientFactory httpClient, ILogger<AccountController> logger)
        {
            _httpClientFactory = httpClient;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Signup()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient(Constants.ClientNoToken);

                    var response = await httpClient.PostAsJsonAsync(Constants.Routes.Register, model);
                    var resp = await response.Content.ReadAsAsync<AddUserResponseDTO>();
                    if (response.IsSuccessStatusCode && resp.resp == true)
                    {
                        TempData["success"] = "Registration Successful. Kindly sign in";
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        var desc = resp.ShortDescription ?? "";
                        ModelState.AddModelError("", desc);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    ModelState.AddModelError("", "Oops. An error occurred during the sign up process. We are looking into it. Kindly try again. ");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please check your inputs");
            }

            return View(model);
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
        public IActionResult Logoff()
        {
            if (User.Identity.IsAuthenticated)
            {
                Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect(Constants.URL.LoginPath);
            }

            return Redirect(Constants.URL.LoginPath);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {

            if (User.Identity.IsAuthenticated)
            {
                Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect(Constants.URL.LoginPath);
            }

            return Redirect(Constants.URL.LoginPath);
        }
        #region comments
        //[AllowAnonymous]
        //public IActionResult Login()
        //{
        //    return View();
        //}
        //[AllowAnonymous]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginModel model, string location)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var httpClient = _httpClientFactory.CreateClient(Constants.ClientNoToken);

        //            var response = await httpClient.PostAsJsonAsync(Constants.Routes.Token, model);
        //            var resp = await response.Content.ReadAsAsync<TokenModel>();
        //            if (response.IsSuccessStatusCode && resp.Token != null)
        //            {                   
        //                var principal = await GetUserClaims(resp);

        //                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        //                    new AuthenticationProperties
        //                    {
        //                        IsPersistent = true,
        //                        AllowRefresh = true,
        //                        ExpiresUtc = resp.Expires,
        //                        IssuedUtc = DateTime.UtcNow
        //                    });
        //                if (string.IsNullOrWhiteSpace(location) || !Url.IsLocalUrl(location))
        //                    return Redirect(Constants.URL.Default);

        //                location = Constants.URL.Default;
        //                //return Redirect("Home/Index");
        //                return RedirectToAction("Index", "Home");
        //            }
        //            else
        //            {
        //                var desc = resp.ShortDescription ?? "";
        //                ModelState.AddModelError("", desc);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex.Message);

        //            ModelState.AddModelError("", "An error occured during login progress.");
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Please check your inputs");
        //    }

        //    return View(model);
        //}
        //protected IActionResult RedirectOrHome(string location = null)
        //{
        //    if (string.IsNullOrWhiteSpace(location) || !Url.IsLocalUrl(location))
        //        return Redirect(Constants.URL.Default);

        //    location = Constants.URL.Default;
        //    return Redirect(location);
        //}
        #endregion
    }
}
