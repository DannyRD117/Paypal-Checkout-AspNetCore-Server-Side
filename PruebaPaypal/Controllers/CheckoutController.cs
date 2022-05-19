using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
using PruebaPaypal.Models.Paypal;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PruebaPaypal.Controllers
{
    public class CheckoutController : Controller
    {
        public async Task<IActionResult> Paypal()
        {
            bool status = false;
            string result = string.Empty;

            using (var client = new HttpClient())
            {

                //INGRESA TUS CREDENCIALES AQUI -> CLIENT ID - SECRET
                var userName = "AXuHiPGbTuM4DL_SCyBa5ff9gTTfzuvFiE3vwbN_cxgpOK5j5AZTyYkb6vwHFyopLoSsVzDX0GiJbdIE";
                var passwd = "EDJG7YLpWoSyteDDYlypr23AjXzVOMNpfla2d70NCHS3wBRbIl81SmOXxjCFj8l2Tvd42fWey5IqEijQ";

                client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com");

                var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));


                var orden = new PaypalOrder()
                {
                    Intent = "CAPTURE",
                    Purchase_units = new List<PurchaseUnit>() {

                        new PurchaseUnit() {

                            Amount = new Amount() {
                                Currency_code = "USD",
                                Value = "10"
                            },
                            Description = "Prueba"
                        }
                    },
                    Application_context = new ApplicationContext()
                    {
                        Brand_name = "Libreria Colibri",
                        Landing_page = "NO_PREFERENCE",
                        User_action = "PAY_NOW", //Accion para que paypal muestre el monto de pago
                        Return_url = "https://localhost:7004/Checkout/Success",// cuando se aprovo la solicitud del cobro
                        Cancel_url = "https://localhost:7004/Checkout/Cancel"// cuando cancela la operacion
                    }
                };


                var json = JsonSerializer.Serialize(orden,
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("/v2/checkout/orders", content);

                if (httpResponse.IsSuccessStatusCode)
                {
                    result = await httpResponse.Content.ReadAsStringAsync();
                }

            }

            var orderResult = JsonSerializer.Deserialize<OrderResult>(result,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            return Redirect(orderResult.Links[1].Href);
        }

        [HttpGet]
        public async Task<IActionResult> Success(string token)
        {
            bool status = false;
            string result = string.Empty;

            using (var client = new HttpClient())
            {

                //INGRESA TUS CREDENCIALES AQUI -> CLIENT ID - SECRET
                var userName = "AXuHiPGbTuM4DL_SCyBa5ff9gTTfzuvFiE3vwbN_cxgpOK5j5AZTyYkb6vwHFyopLoSsVzDX0GiJbdIE";
                var passwd = "EDJG7YLpWoSyteDDYlypr23AjXzVOMNpfla2d70NCHS3wBRbIl81SmOXxjCFj8l2Tvd42fWey5IqEijQ";

                client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com");

                var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var content = new StringContent("{}", Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync($"v2/checkout/orders/{token}/capture", content);

                if (httpResponse.IsSuccessStatusCode)
                {
                    result = await httpResponse.Content.ReadAsStringAsync();
                }

            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Cancel()
        {
            return View();
        }
    }
}
