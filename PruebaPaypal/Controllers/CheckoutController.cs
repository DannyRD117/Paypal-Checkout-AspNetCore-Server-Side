using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PruebaPaypal.Models.Paypal;

namespace PruebaPaypal.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly PaypalSettings _config;
        private readonly PaypalClient _paypalClient;
        public CheckoutController(IOptions<PaypalSettings> config, PaypalClient paypalClient)
        {
            _config = config.Value;
            _paypalClient = paypalClient;
            
        }
        public async Task<IActionResult> Paypal()
        {

            
            var accesstoken = await _paypalClient.GetToken(_config.ClientID, _config.SecretID);
            if(accesstoken != null)
            {
                //    //Creacion de orden simple

                //    //var order = new Order()
                //    //{
                //    //    Intent = "CAPTURE",
                //    //    Purchase_units = new List<PurchaseUnit>() {

                //    //        new PurchaseUnit() {

                //    //            Amount = new Amount() {
                //    //                Currency_code = "USD",
                //    //                Value = "10"
                //    //            },
                //    //            Description = "Prueba"
                //    //        }
                //    //    },
                //    //    Application_context = new ApplicationContext()
                //    //    {
                //    //        Brand_name = "Libreria Colibri",
                //    //        Landing_page = "NO_PREFERENCE",
                //    //        User_action = "PAY_NOW", //Accion para que paypal muestre el monto de pago
                //    //        Return_url = "https://localhost:7004/Checkout/Success",// cuando se aprovo la solicitud del cobro
                //    //        Cancel_url = "https://localhost:7004/Checkout/Cancel"// cuando cancela la operacion
                //    //    }
                //    //};


                //Creacion de orden indicando items

                var order = new Order()
                {
                    Intent = "CAPTURE",
                    Purchase_units = new List<PurchaseUnit>() {

                            new PurchaseUnit() {

                                Amount = new Amount() {
                                    Currency_code = "USD",
                                    Value = "70",
                                    Breakdown = new Breakdown()
                                    {
                                        Item_total = new Amount()
                                        {
                                            Currency_code="USD",
                                            Value ="70"
                                        }
                                    }
                                },
                                Description = "Prueba",
                                Items = new List<Items>
                                {
                                    new Items()
                                    {
                                        Name = "Item 1",
                                        Unit_amount = new Amount()
                                        {
                                            Currency_code = "USD",
                                            Value = "10"
                                        },
                                        Quantity = "1"
                                    },
                                    new Items()
                                    {
                                        Name = "Item 2",
                                        Unit_amount = new Amount()
                                        {
                                            Currency_code = "USD",
                                            Value = "30"
                                        },
                                        Quantity = "2"
                                    }
                                }
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
                var orderResult = await _paypalClient.CreateOrder(order, accesstoken);
                if (orderResult != null)
                {
                    return Redirect(orderResult.Links[1].Href);
                }             
            }
            return RedirectToAction("Cancel");           
        }

        [HttpGet]
        public async Task<IActionResult> Success(string token)
        {
            var accesstoken = await _paypalClient.GetToken(_config.ClientID, _config.SecretID);
            if(accesstoken != null)
            {
                var result = await _paypalClient.CaptureOrder(accesstoken, token);
                if (result)
                {
                    return View();
                }
            }
            return RedirectToAction("Cancel");   
        }

        [HttpGet]
        public async Task<IActionResult> Cancel()
        {
            return View();
        }
    }
}
