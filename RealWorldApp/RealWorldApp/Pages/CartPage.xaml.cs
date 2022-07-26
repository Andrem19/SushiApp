using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using RealWorldApp.Services;
using Xamarin.Forms.Xaml;
using RealWorldApp.Models.ModelsProd;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.SignalR.Client;

namespace RealWorldApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartPage : ContentPage
    {
        public ObservableCollection<BasketItemDto> ShoppingCartCollection;
        double totalPrice { get; set; }
        HubConnection _connection;
        public CartPage(HubConnection connection)
        {
            InitializeComponent();
            _connection = connection;
            ShoppingCartCollection = new ObservableCollection<BasketItemDto>();
            GetShoppingCartItems();
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void TapClearCart_Tapped(object sender, EventArgs e)
        {
            var response = await ApiService.DeleteBasket(Preferences.Get("basket_id", string.Empty));
            if (response)
            {
                Preferences.Remove("basket_id");
                LvShoppingCart.ItemsSource = null;
                LblTotalPrice.Text = "0";
                await DisplayAlert("", "Your basket has been cleared", "Alright");
            }
        }

        private async void GetShoppingCartItems()
        {
            var shoppingCartItems = await ApiService.GetBasket(Preferences.Get("basket_id", string.Empty));
            
            foreach (var shoppingCart in shoppingCartItems.Items)
            {
                BasketItemDto ItemWithTotal = new BasketItemDto();

                ItemWithTotal.ProductName = shoppingCart.ProductName;
                ItemWithTotal.Price = shoppingCart.Price;
                ItemWithTotal.Quantity = shoppingCart.Quantity;
                ItemWithTotal.PictureUrl = shoppingCart.PictureUrl;
                ItemWithTotal.Id = shoppingCart.Id;
                ItemWithTotal.IdProd = shoppingCart.IdProd;
                ItemWithTotal.Type = shoppingCart.Type;
                ItemWithTotal.Total = shoppingCart.Price * shoppingCart.Quantity;
                ShoppingCartCollection.Add(ItemWithTotal);
            }
            LvShoppingCart.ItemsSource = ShoppingCartCollection;
            for (int i = 0; i < shoppingCartItems.Items.Count; i++)
            {
                totalPrice += (shoppingCartItems.Items[i].Quantity * shoppingCartItems.Items[i].Price);
            }
            LblTotalPrice.Text = totalPrice.ToString();
        }

        private void BtnProceed_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new PlaceOrderPage(_connection));
        }

        private async void TapDecrement_Tapped(object sender, EventArgs e)
        {
            CustomerBasket shoppingCartItems = null;
            Image lblClicked = (Image)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            var id = item.CommandParameter;

            var shoppingCarts = await ApiService.GetBasket(Preferences.Get("basket_id", string.Empty));
            int index = shoppingCarts.Items.FindIndex(x => x.IdProd == Convert.ToInt32(id));
            if (index != -1)
            {
                if (shoppingCarts.Items[index].Quantity > 1)
                {
                    shoppingCarts.Items[index].Quantity -= 1;
                }
                else
                {
                    shoppingCarts.Items.RemoveAt(index);
                }
                shoppingCartItems = await ApiService.AddItemToBasket(shoppingCarts);
            }
            if (shoppingCartItems != null)
            {
                ShoppingCartCollection.Clear();
                foreach (var shoppingCart in shoppingCartItems.Items)
                {
                    BasketItemDto ItemWithTotal = new BasketItemDto();

                    ItemWithTotal.ProductName = shoppingCart.ProductName;
                    ItemWithTotal.Price = shoppingCart.Price;
                    ItemWithTotal.Quantity = shoppingCart.Quantity;
                    ItemWithTotal.PictureUrl = shoppingCart.PictureUrl;
                    ItemWithTotal.Id = shoppingCart.Id;
                    ItemWithTotal.IdProd = shoppingCart.IdProd;
                    ItemWithTotal.Type = shoppingCart.Type;
                    ItemWithTotal.Total = shoppingCart.Price * shoppingCart.Quantity;
                    ShoppingCartCollection.Add(ItemWithTotal);
                }
                LvShoppingCart.ItemsSource = ShoppingCartCollection;
                totalPrice = 0;
                for (int i = 0; i < shoppingCartItems.Items.Count; i++)
                {
                    totalPrice += (shoppingCartItems.Items[i].Quantity * shoppingCartItems.Items[i].Price);
                }
                LblTotalPrice.Text = totalPrice.ToString();
            }
        }

        private async void TapIncrement_Tapped(object sender, EventArgs e)
        {
            CustomerBasket shoppingCartItems = null;
            Image lblClicked = (Image)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            var id = item.CommandParameter;

            var shoppingCarts = await ApiService.GetBasket(Preferences.Get("basket_id", string.Empty));
            int index = shoppingCarts.Items.FindIndex(x => x.IdProd == Convert.ToInt32(id));
            if (index != -1)
            {
                shoppingCarts.Items[index].Quantity += 1;
                shoppingCartItems = await ApiService.AddItemToBasket(shoppingCarts);
            }
            if (shoppingCartItems != null)
            {
                ShoppingCartCollection.Clear();
                foreach (var shoppingCart in shoppingCartItems.Items)
                {
                    BasketItemDto ItemWithTotal = new BasketItemDto();

                    ItemWithTotal.ProductName = shoppingCart.ProductName;
                    ItemWithTotal.Price = shoppingCart.Price;
                    ItemWithTotal.Quantity = shoppingCart.Quantity;
                    ItemWithTotal.PictureUrl = shoppingCart.PictureUrl;
                    ItemWithTotal.Id = shoppingCart.Id;
                    ItemWithTotal.IdProd = shoppingCart.IdProd;
                    ItemWithTotal.Type = shoppingCart.Type;
                    ItemWithTotal.Total = shoppingCart.Price * shoppingCart.Quantity;
                    ShoppingCartCollection.Add(ItemWithTotal);
                }
                LvShoppingCart.ItemsSource = ShoppingCartCollection;
                totalPrice = 0;
                for (int i = 0; i < shoppingCartItems.Items.Count; i++)
                {
                    totalPrice += (shoppingCartItems.Items[i].Quantity * shoppingCartItems.Items[i].Price);
                }
                LblTotalPrice.Text = totalPrice.ToString();
            }
        }
    }
}