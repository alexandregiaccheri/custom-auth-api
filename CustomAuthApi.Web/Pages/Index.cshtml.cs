using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace CustomAuthApi.Web.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

        }

        public async Task OnPostUnprotectedAsync()
        {
            using (var client = new HttpClient())
            {
                var httpMessage = new HttpRequestMessage();
                httpMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer",
                    HttpContext.Session.GetString("JWT"));
                httpMessage.Method = HttpMethod.Get;
                httpMessage.RequestUri = new Uri(
                    "https://localhost:7001/TestAuthentication/AuthenticationNotRequired");

                var response = await client.SendAsync(httpMessage);

                if (response.IsSuccessStatusCode)
                    ViewData["TestResponseOk"] = await response
                        .Content.ReadAsStringAsync();
                else
                    ViewData["TestResponseFail"] = "There was an error processing your request";
            }
        }

        public async Task OnPostProtectedAsync()
        {
            using (var client = new HttpClient())
            {
                var httpMessage = new HttpRequestMessage();
                httpMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer",
                    HttpContext.Session.GetString("JWT"));
                httpMessage.Method = HttpMethod.Get;
                httpMessage.RequestUri = new Uri(
                    "https://localhost:7001/TestAuthentication/AuthenticationRequired");

                var response = await client.SendAsync(httpMessage);

                if (response.IsSuccessStatusCode)
                    ViewData["TestResponseOk"] = await response
                        .Content.ReadAsStringAsync();
                else
                    ViewData["TestResponseFail"] = "There was an error processing your request";
            }
        }

        public async Task OnPostAdminProtectedAsync()
        {
            using (var client = new HttpClient())
            {
                var httpMessage = new HttpRequestMessage();
                httpMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer",
                    HttpContext.Session.GetString("JWT"));
                httpMessage.Method = HttpMethod.Get;
                httpMessage.RequestUri = new Uri(
                    "https://localhost:7001/TestAuthentication/AdminRoleRequired");

                var response = await client.SendAsync(httpMessage);

                if (response.IsSuccessStatusCode)
                    ViewData["TestResponseOk"] = await response
                        .Content.ReadAsStringAsync();
                else
                    ViewData["TestResponseFail"] = "There was an error processing your request";
            }
        }
    }
}