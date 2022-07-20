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

namespace RealWorldApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public ObservableCollection<ProductToReturnDto> ProductsCollection;
        public ObservableCollection<Category> CategoriesCollection;
        public HomePage()
        {
            InitializeComponent();
            ProductsCollection = new ObservableCollection<ProductToReturnDto>();
            CategoriesCollection = new ObservableCollection<Category>();
            GetPopularProducts();
            GetCategories();
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
    }
}