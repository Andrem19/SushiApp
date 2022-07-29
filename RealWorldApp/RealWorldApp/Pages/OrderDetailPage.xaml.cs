using RealWorldApp.Models.ModelsProd;
using RealWorldApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealWorldApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetailPage : ContentPage
    {
        public ObservableCollection<OrderItemDto> OrderDetailCollection;
        public OrderDetailPage(int orderId)
        {
            InitializeComponent();
            OrderDetailCollection = new ObservableCollection<OrderItemDto>();
            GetOrderDetail(orderId);
        }

        public void VisibleAddressBlock()
        {
            Address_Block.IsVisible = false;
            string role = Preferences.Get("Role", string.Empty);
            if (role == "Admin" || role == "Moderator")
            {
                Address_Block.IsVisible = true;
            }
        }

        private async void GetOrderDetail(int orderId)
        {
            var order = await ApiService.GetOrderDetails(orderId);
            
            var orderItems = order.OrderItems;
            foreach (var item in orderItems)
            {
                item.SubT();
                OrderDetailCollection.Add(item);
            }

            LvOrderDetail.ItemsSource = OrderDetailCollection;

            LblTotalPrice.Text = "£" + order.Total.ToString();

            if (order.DeliveryMethod == 1)
            {
                DeliveryMethodLabel.Text = "Customer pick it up";
            }
            else if (order.DeliveryMethod == 0)
            {
                DeliveryMethodLabel.Text = "Free Delivery";
            }
            else
            {
                DeliveryMethodLabel.Text = $"Delivery Cost: {order.DeliveryMethod}";
            }
            Name.Text = order.ShipToAddress.Name;
            PostCode.Text = order.ShipToAddress.PostCode;
            Line1.Text = order.ShipToAddress.NumberOfHouse;
            Line2.Text = order.ShipToAddress.Street;
            City.Text = order.ShipToAddress.City;
            PhoneNumber.Text = order.ShipToAddress.TelephoneNumber;
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}