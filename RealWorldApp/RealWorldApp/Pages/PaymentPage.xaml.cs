using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RealWorldApp.Models;
using RealWorldApp.Models.ModelsProd;
using RealWorldApp.Services;
using Stripe;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealWorldApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentPage : ContentPage
    {
        public AddressDto _address { get; set; }
        public CustomerBasket _basket { get; set; }
        public PaymentPage(AddressDto address)
        {
            InitializeComponent();
            GetBasket();
            _address = address;
        }

        public async void GetBasket()
        {
            _basket = await ApiService.GetBasket(Preferences.Get("basket_id", string.Empty));
        }

        private async void PayCommand(object sender, EventArgs e)
        {
            await CreateOrder();
            CardModel card = new CardModel();
            card.Number = CardNumber.Text.Replace(" ", string.Empty);
            card.ExpMonth = Convert.ToInt16(Expiry.Text.Substring(0, 2));
            card.ExpYear = Convert.ToInt16(Expiry.Text.Substring(3, 2));
            card.Cvc = Cvc.Text;

            await Pay(_address, _basket, card);
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new HomePage());
        }
    
        public async Task CreateOrder()
        {
            var newOrder = new OrderToCreate();
            newOrder.BasketId = Preferences.Get("basket_id", string.Empty);
            newOrder.ShipToAddress = _address;
            await ApiService.PlaceOrder(newOrder);
        }

        public static async Task Pay(AddressDto address, CustomerBasket basket, CardModel card)
        {
            try
            {
                var Client = new StripeClient(AppSettings.stripe_pub_key);

                var payIntent = new PaymentIntentService(Client);

                var confirmOptions = new PaymentIntentConfirmOptions
                {
                    ClientSecret = basket.ClientSecret,
                    PaymentMethodData = new PaymentIntentPaymentMethodDataOptions
                    {
                        Type = "card",
                        BillingDetails = new PaymentIntentPaymentMethodDataBillingDetailsOptions
                        {
                            Name = address.Name
                        }
                    },
                    ReturnUrl = string.Format("{0}/webhook", AppSettings.ApiUrlProd) // Change this with your Return URL
                };

                // Add Card Info - Refer to official Stripe documentation
                confirmOptions.AddExtraParam("payment_method_data[card][number]", card.Number);
                confirmOptions.AddExtraParam("payment_method_data[card][exp_month]", card.ExpMonth);
                confirmOptions.AddExtraParam("payment_method_data[card][exp_year]", card.ExpYear);
                confirmOptions.AddExtraParam("payment_method_data[card][cvc]", card.Cvc);

                var payResult = await payIntent.ConfirmAsync(basket.PaymentIntentId, confirmOptions);

                if (!string.IsNullOrEmpty(payResult.Id))
                {
                    var response = await ApiService.DeleteBasket(Preferences.Get("basket_id", string.Empty));
                    if (response)
                    {
                        Preferences.Remove("basket_id");
                    }
                }
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}