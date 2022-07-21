using RealWorldApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RealWorldApp.Models;
using RealWorldApp.Models.ModelsProd;
using Xamarin.Essentials;
using System.Diagnostics;

namespace RealWorldApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public ObservableCollection<ProductToReturnDto> ProductsCollection;
        public ObservableCollection<Category> CategoriesCollection;
        public int Id;
        public HomePage()
        {
            if (Debugger.IsAttached) Preferences.Clear();
            InitializeComponent();
            ProductsCollection = new ObservableCollection<ProductToReturnDto>();
            CategoriesCollection = new ObservableCollection<Category>();
            GetPopularProducts();
            GetCategories();
            LblUserName.Text = Preferences.Get("userName", string.Empty);
        }
        private async void GetCategories()
        {
            var categories = await ApiService.GetCategories();
            foreach (var category in categories)
            {
                CategoriesCollection.Add(category);
            }
            CvCategories.ItemsSource = CategoriesCollection;
        }
        private async void GetPopularProducts()
        {
            var products = await ApiService.GetProducts();
            foreach (var product in products)
            {
                ProductsCollection.Add(product);
            }
            CvProducts.ItemsSource = ProductsCollection;
        }
        private async void ImgMenu_Tapped(object sender, EventArgs e)
        {
            GridOverlay.IsVisible = true;
            await SlMenu.TranslateTo(0, 0, 400, Easing.Linear);
        }
        private void TapCloseMenu_Tapped(object sender, EventArgs e)
        {
            CloseHamBurgerMenu();
        }
        private async void CloseHamBurgerMenu()
        {
            await SlMenu.TranslateTo(-250, 0, 400, Easing.Linear);
            GridOverlay.IsVisible = false;
        }
        private void TapLogout_Tapped(object sender, EventArgs e)   
        {
            Preferences.Set("accessToken", string.Empty);
            Preferences.Set("tokenExpirationTime", 0);
            Application.Current.MainPage = new NavigationPage(new SignupPage());
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            string basket_id = Preferences.Get("basket_id", string.Empty);
            if (!string.IsNullOrEmpty(basket_id))
            {
                CustomerBasket basket = await ApiService.GetBasket(basket_id);
                LblTotalItems.Text = basket.Items.Count.ToString();
            }
            else LblTotalItems.Text = "0";
            
            
        }

        private async void AddToCard_Button(object sender, EventArgs e)
        {
            string basket_id = Preferences.Get("basket_id", string.Empty);
            if (string.IsNullOrEmpty(basket_id))
            {
                ProductToReturnDto product = GetProductFromId(((Button)sender).BindingContext.ToString());
                CustomerBasket basket = new CustomerBasket();
                string bas_id = Guid.NewGuid().ToString();
                basket.basket_id = bas_id;
                BasketItem item = new BasketItem();
                item.IdProd = product.Id;
                item.ProductName = product.Name;
                item.Price = product.Price;
                item.PictureUrl = product.PictureUrl;
                item.Quantity = 1;
                item.Type = product.ProductType;
                basket.Items.Add(item);
                Preferences.Set("basket_id", bas_id);
                await ApiService.CreateBasket(basket);
                LblTotalItems.Text = basket.Items.Count.ToString();
            }
            else
            {
                CustomerBasket basket = await ApiService.GetBasketById(basket_id);
                int index = basket.Items.FindIndex(x => x.Id == Convert.ToInt32(((Button)sender).BindingContext.ToString()));
                if (index == -1)
                {
                    ProductToReturnDto product = GetProductFromId(((Button)sender).BindingContext.ToString());
                    BasketItem item = new BasketItem();
                    item.IdProd = product.Id;
                    item.ProductName = product.Name;
                    item.Price = product.Price;
                    item.PictureUrl = product.PictureUrl;
                    item.Quantity = 1;
                    item.Type = product.ProductType;
                    basket.Items.Add(item);
                }
                else
                {
                    basket.Items[index].Quantity += 1;
                }
                await ApiService.AddItemToBasket(basket);
                LblTotalItems.Text = basket.Items.Count.ToString();
            }
            
        }
        public ProductToReturnDto GetProductFromId(string product_id)
        {
            List<ProductToReturnDto> prodList = ProductsCollection.ToList();
            ProductToReturnDto product = prodList.FirstOrDefault(x => x.Id == Convert.ToInt32(product_id));
            return product;
        }
    }
}