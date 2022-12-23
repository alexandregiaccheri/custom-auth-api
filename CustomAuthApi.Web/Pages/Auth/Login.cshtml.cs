using CustomAuthApi.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CustomAuthApi.Web.Pages.Auth
{
    public class LoginPageModel : PageModel
    {
        [BindProperty]
        public LoginDTO Login { get; set; } = null!;

        public void OnGet()
        {
        }
        public async Task OnPost()
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var loginEndpoint = "https://localhost:7001/Authentication/Login";
                    var httpContent = new
                        StringContent(JsonConvert.SerializeObject(Login),
                        Encoding.UTF8, "application/json");


                    var result = await client.PostAsync(loginEndpoint, httpContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var jwtHandler = new JwtSecurityTokenHandler();
                        var jwtResult = await result.Content.ReadAsStringAsync();
                        var tokenResult = jwtHandler.ReadJwtToken(jwtResult);

                        HttpContext.Session.SetString("JWT", jwtResult);
                        HttpContext.Session.SetString("User", Login.Email);
                        if (tokenResult.Claims
                                .FirstOrDefault(c => c.Type.Contains("role")) is not null)
                        {
                            HttpContext.Session.SetString("Role", tokenResult.Claims
                                .FirstOrDefault(c => c.Type.Contains("role"))!.Value);
                        }
                    }

                    else
                    {
                        TempData["LoginError"] = await result.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        public ActionResult OnPostSignout()
        {
            HttpContext.Session.Remove("JWT");
            HttpContext.Session.Remove("User");
            HttpContext.Session.Remove("Role");
            return RedirectToPage();
        }
    }
}
