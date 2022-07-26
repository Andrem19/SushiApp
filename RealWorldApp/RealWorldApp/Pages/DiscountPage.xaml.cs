using Microsoft.AspNetCore.SignalR.Client;
using RealWorldApp.Models.ModelsProd;
using RealWorldApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealWorldApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscountPage : ContentPage
    {
        public ICommand TapCommand => new Command(async () => await Navigation.PushModalAsync(new ReferalPage()));
        AddressDto _address { get; set; }
        HubConnection _connection;
        CustomerBasket _basket { get; set; }
        UserDto _currentUser { get; set; }
        public DiscountPage(CustomerBasket basket, AddressDto address, HubConnection connection)
        {
            InitializeComponent();
            _connection = connection;
            BindingContext = this;
            _basket = basket;
            _address = address;
            GetCurrentUser();
        }
        public async void GetCurrentUser()
        {
           _currentUser = await ApiService.GetUser();
            if (_currentUser.RefDiscount == 0)
            {
                QuantityOfRefDisc.IsVisible = false;
                ReferalDiscount.IsVisible = false;
                RefPerc.IsVisible = false;
                RefI.IsVisible = false;
                RefInfo.IsVisible = true;
            }
            else
            {
                QuantityOfRefDisc.Text = _currentUser.RefDiscount.ToString();
            }
            if (_currentUser.AcumDiscount == false)
            {
                AcumulateDiscount.IsVisible = false;
                AcumPerc.IsVisible = false;
                AcumI.IsVisible = false;
                AcumInfo.IsVisible = true;
            }
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void PromoCodeChanging(object sender, TextChangedEventArgs e)
        {
            string promo = promoCodeEditor.Text.ToString();
            if (promo.Length > 0)
            {
                QuantityOfRefDisc.IsVisible = false;
                ReferalDiscount.IsVisible = false;
                RefPerc.IsVisible = false;
                RefI.IsVisible = false;
                RefInfo.IsVisible = false;
                AcumulateDiscount.IsVisible = false;
                AcumPerc.IsVisible = false;
                AcumI.IsVisible = false;

                AcumInfo.Text = "You can't use Promo Code and another discounts togather.";
                AcumInfo.IsVisible = true;
            }
            else
            {
                completeImg.IsVisible = false;
                if (_currentUser.RefDiscount == 0)
                {
                    QuantityOfRefDisc.IsVisible = false;
                    ReferalDiscount.IsVisible = false;
                    RefPerc.IsVisible = false;
                    RefI.IsVisible = false;
                    RefInfo.IsVisible = true;

                }
                else
                {
                    QuantityOfRefDisc.IsVisible = true;
                    ReferalDiscount.IsVisible = true;
                    RefPerc.IsVisible = true;
                    RefI.IsVisible = true;
                    RefInfo.IsVisible = false;
                }
                if (_currentUser.AcumDiscount == false)
                {
                    AcumulateDiscount.IsVisible = false;
                    AcumPerc.IsVisible = false;
                    AcumI.IsVisible = false;

                    AcumInfo.Text = "At the moment you have no accumulated discounts.  To get them you should order more.";
                    AcumInfo.IsVisible = true;
                }
                else
                {
                    AcumulateDiscount.IsVisible = true;
                    AcumPerc.IsVisible = true;
                    AcumI.IsVisible = true;

                    AcumInfo.Text = "At the moment you have no accumulated discounts.  To get them you should order more.";
                    AcumInfo.IsVisible = false;
                }
            }

            
            if (promo.Length > 8)
            {
                bool triger = await ApiService.CheckPromo(promo);
                if (triger == true)
                {
                    _basket.promoDisc = promo;
                    completeImg.IsVisible = true;
                    completeImg.Source = "complete.png";
                }
                else
                {
                    _basket.promoDisc = "1";
                    completeImg.IsVisible = true;
                    completeImg.Source = "close.png";
                }
            }
        }

        private async void To_Review(object sender, EventArgs e)
        {
            string basket_id = Preferences.Get("basket_id", string.Empty);
            await ApiService.AddItemToBasket(_basket);
            await ApiService.CreatePaymentIntent(basket_id);
            await Navigation.PushModalAsync(new ReviewPage(_address, _basket, _connection));
        }

        private void RefCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            completeImg.IsVisible = false;
            promoCodeEditor.IsVisible = false;
            Promo1.IsVisible = false;
            Promo2.IsVisible = false;
            PromoInfo.IsVisible = true;
            _basket.promoDisc = "1";
            if (ReferalDiscount.IsChecked == true)
            {
                _basket.refDisc = true;
            }
            else if (AcumulateDiscount.IsChecked == false && ReferalDiscount.IsChecked == false)
            {
                _basket.refDisc = false;
                completeImg.IsVisible = false;
                promoCodeEditor.IsVisible = true;
                Promo1.IsVisible = true;
                Promo2.IsVisible = true;
                PromoInfo.IsVisible = false;
            }
        }

        private void AcumCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            completeImg.IsVisible = false;
            promoCodeEditor.IsVisible = false;
            Promo1.IsVisible = false;
            Promo2.IsVisible = false;
            PromoInfo.IsVisible = true;
            _basket.promoDisc = "1";
            if (AcumulateDiscount.IsChecked == true)
            {
                _basket.acumDisc = true;
            }
            else if (AcumulateDiscount.IsChecked == false && ReferalDiscount.IsChecked == false)
            {
                _basket.acumDisc = false;
                completeImg.IsVisible = false;
                promoCodeEditor.IsVisible = true;
                Promo1.IsVisible = true;
                Promo2.IsVisible = true;
                PromoInfo.IsVisible = false;
            }
        }
    }
}