using RealWorldApp.Models.ModelsProd;
using RealWorldApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealWorldApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscountPage : ContentPage
    {
        public ICommand TapCommand => new Command(async () => await Navigation.PushModalAsync(new ReferalPage()));
        AddressDto _address { get; set; }
        CustomerBasket _basket { get; set; }
        UserDto _currentUser { get; set; }
        public DiscountPage(CustomerBasket basket, AddressDto address)
        {
            InitializeComponent();
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

        private void To_Review(object sender, EventArgs e)
        {

        }
    }
}