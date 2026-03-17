using System.Net.Http.Headers;

namespace PlatformManagementSystem.MVC.Helpers
{
    public static class ApiClientHelper
    {
        public static void AddJwtToken(HttpClient client, HttpContext context)
        {
            var token = context.Session.GetString("UserToken");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
