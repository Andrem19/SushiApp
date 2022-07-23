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
    public partial class PlaceOrderPage : ContentPage
    {
        private UserDto _currentUser { get; set; }
        private AddressDto Address { get; set; }
        public PlaceOrderPage()
        {
            InitializeComponent();

            GetCurrentUser();
        }

        public async void GetCurrentUser()
        {
            Address = await ApiService.GetUserAddress();

            EntName.Text = Address.Name;
            EntPhone.Text = Address.TelephoneNumber;
            EntPostcode.Text = Address.PostCode;
            EntLine1.Text = Address.NumberOfHouse;
            EntLine2.Text = Address.Street;
            EntCity.Text = Address.City;
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void Next_Button(object sender, EventArgs e)
        {

        }
    }
}