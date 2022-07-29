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
    public partial class OrderListPage : ContentPage
    {
        public ObservableCollection<OrderResponse> OrdersCollection;
        public OrderListPage()
        {
            InitializeComponent();
            OrdersCollection = new ObservableCollection<OrderResponse>();
            GetOrderItems();
        }
        private async void GetOrderItems()
        {
            var orders = await ApiService.GetOrdersByPoint();
            foreach (var order in orders)
            {
                order.NormalizeData();
                OrdersCollection.Add(order);
            }
            LvOrders.ItemsSource = OrdersCollection;
        }
        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void LvOrders_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedOrder = e.SelectedItem as OrderResponse;
            if (selectedOrder != null)
            {
                Navigation.PushModalAsync(new OrderDetailPage(selectedOrder.Id));
            }
            ((ListView)sender).SelectedItem = null;
        }
    }
}