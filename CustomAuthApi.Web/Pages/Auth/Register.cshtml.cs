using CustomAuthApi.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace CustomAuthApi.Web.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterDTO Register { get; set; } = null!;

        public void OnGet()
        {
        }

        public async Task OnPost()
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var registerEndpoint = "https://localhost:7001/Authentication/Register";
                    var httpContent = new
                        StringContent(JsonConvert.SerializeObject(Register),
                        Encoding.UTF8, "application/json");

                    var result = await client.PostAsync(registerEndpoint, httpContent);
                    TempData["RegisterState"] = await result.Content.ReadAsStringAsync();
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
