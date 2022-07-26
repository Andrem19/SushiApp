using Microsoft.AspNetCore.SignalR.Client;
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
        private AddressDto Address { get; set; }
        HubConnection _connection;
        public bool addressComplite { get; set; }
        public PlaceOrderPage(HubConnection connection)
        {
            InitializeComponent();
            _connection = connection;
            DisableButton();
            GetCurrentUser();
        }
        public void DisableButton()
        {
            if (string.IsNullOrEmpty(EntName.Text) || string.IsNullOrEmpty(EntLine1.Text) || string.IsNullOrEmpty(EntCity.Text) ||
                string.IsNullOrEmpty(EntPostcode.Text) || string.IsNullOrEmpty(EntLine2.Text) || string.IsNullOrEmpty(EntPhone.Text))
            {
                addressComplite = false;
            }
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

        private async void Next_Button(object sender, EventArgs e)
        {
            AddressDto address = new AddressDto();
            address.Name = EntName.Text;
            address.TelephoneNumber = EntPhone.Text;
            address.PostCode = EntPostcode.Text;
            address.Street = EntLine2?.Text;
            address.NumberOfHouse = EntLine1?.Text;
            address.City = EntCity.Text;
            if (string.IsNullOrEmpty(address.Name) || string.IsNullOrEmpty(address.TelephoneNumber) || string.IsNullOrEmpty(address.PostCode)
                || string.IsNullOrEmpty(address.Street) || string.IsNullOrEmpty(address.NumberOfHouse) || string.IsNullOrEmpty(address.City))
            {
                await DisplayAlert("", "Fill in all the fields", "Ok");
            }
            else
            {
                PointReturn pointInfo = await ApiService.WhereIAm(address);
                if (pointInfo.Enable != false)
                {
                    if (SaveAddress.IsChecked == true)
                    {
                        await ApiService.UpdateAddress(address);
                    }
                    await Navigation.PushModalAsync(new DeliveryPage(pointInfo, address, _connection));
                }
                else
                {
                    await DisplayAlert("Sorry", "There is no service in your city. We are working on expanding our network", "Ok");
                }
                
            }
            
        }
    }
}