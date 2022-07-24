using RealWorldApp.Models.ModelsProd;
using RealWorldApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealWorldApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryPage : ContentPage
    {
        PointReturn _pointInfo { get; set; }
        AddressDto _address { get; set; }
        CustomerBasket _basket { get; set; }
        public DeliveryPage(PointReturn pointInfo, AddressDto address)
        {
            InitializeComponent();
            _pointInfo = pointInfo;
            _address = address;
            CostDelivery.Content = "Normal delivery " + "£" + pointInfo.DeliveryCost.ToString();
            GetBasket();
            SetCheckBoxes();
        }
        private async void GetBasket()
        {
            _basket = await ApiService.GetBasket(Preferences.Get("basket_id", string.Empty));
        }
            private async void Next_Button(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new DiscountPage(_basket, _address));
        }

        public void SetCheckBoxes()
        {
            AddressLabel.Text = $"Point address: {_pointInfo.City} {_pointInfo.House} {_pointInfo.Street} {_pointInfo.PostCode}";
            if (_pointInfo.FreeZone == true)
            {
                CostDelivery.IsVisible = false;
            }
            if (_pointInfo.Enable == true && _pointInfo.DeliveryCost == 0)
            {
                CostDelivery.IsVisible = false;
                FreeDelivery.IsVisible = false;
            }
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void ChangeRadio1(object sender, CheckedChangedEventArgs e)
        {
            BtnNext.IsEnabled = true;
            _basket.DeliveryMethod = _pointInfo.DeliveryCost;
            await ApiService.AddItemToBasket(_basket);
        }

        private async void ChangeRadio2(object sender, CheckedChangedEventArgs e)
        {
            BtnNext.IsEnabled = true;
            _basket.DeliveryMethod = 0;
            await ApiService.AddItemToBasket(_basket);
        }

        private async void ChangeRadio3(object sender, CheckedChangedEventArgs e)
        {
            BtnNext.IsEnabled = true;
            _basket.DeliveryMethod = 1;
            await ApiService.AddItemToBasket(_basket);
        }
    }
}