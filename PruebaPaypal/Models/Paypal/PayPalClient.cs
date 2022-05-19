using PayPalCheckoutSdk.Core;

namespace PruebaPaypal.Models.Paypal
{
    public class PayPalClient
    {
        public static PayPalEnvironment Enviroment()
        {
            return new SandboxEnvironment("AXuHiPGbTuM4DL_SCyBa5ff9gTTfzuvFiE3vwbN_cxgpOK5j5AZTyYkb6vwHFyopLoSsVzDX0GiJbdIE", "EDJG7YLpWoSyteDDYlypr23AjXzVOMNpfla2d70NCHS3wBRbIl81SmOXxjCFj8l2Tvd42fWey5IqEijQ");
        }

        public static PayPalHttpClient Client()
        {
            return new PayPalHttpClient(Enviroment());
        }
    }
}
