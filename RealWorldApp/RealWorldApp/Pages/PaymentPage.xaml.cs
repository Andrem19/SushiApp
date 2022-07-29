using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
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
        HubConnection _connection;
        ICustomNotification notification;
        public PaymentPage(AddressDto address, HubConnection connection)
        {
            InitializeComponent();
            GetBasket();
            notification = DependencyService.Get<ICustomNotification>();
            _connection = connection;
            _address = address;
        }

        public async void GetBasket()
        {
            _basket = await ApiService.GetBasket(Preferences.Get("basket_id", string.Empty));
        }

        private async void PayCommand(object sender, EventArgs e)
        {
            OrderResponse order = await CreateOrder();
            CardModel card = new CardModel();
            card.Number = CardNumber.Text.Replace(" ", string.Empty);
            card.ExpMonth = Convert.ToInt16(Expiry.Text.Substring(0, 2));
            card.ExpYear = Convert.ToInt16(Expiry.Text.Substring(3, 2));
            card.Cvc = Cvc.Text;

            string paymentId = await Pay(CardName.Text, _basket, card);
            
            if (_connection.State != HubConnectionState.Connected)
            {
                await _connection.StartAsync();
            }
            if (!string.IsNullOrEmpty(paymentId))
            {
                await SignalRClient.NewOrderCreated(paymentId);
                if (order.DeliveryMethod == 1)
                {
                    
                    await _connection.InvokeAsync("authMe", Preferences.Get("Email", string.Empty));
                    _connection.On<string>("orderIsReady", (message) =>
                    {
                        notification.send($"Order {order.Id}", "Your Order Is Ready To Collect");
                        _connection.StopAsync();
                    });
                }
            }
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new HomePage());
        }
    
        public async Task<OrderResponse> CreateOrder()
        {
            var newOrder = new OrderToCreate();
            newOrder.BasketId = Preferences.Get("basket_id", string.Empty);
            newOrder.ShipToAddress = _address;
            var order = await ApiService.PlaceOrder(newOrder);
            return order;
        }

        public static async Task<string> Pay(string CardName, CustomerBasket basket, CardModel card)
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
                            Name = CardName
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
                        return basket.PaymentIntentId;
                    }
                }
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            return "";
        }
    }
}