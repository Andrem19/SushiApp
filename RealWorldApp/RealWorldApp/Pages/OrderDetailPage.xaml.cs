using RealWorldApp.Models.ModelsProd;
using RealWorldApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}