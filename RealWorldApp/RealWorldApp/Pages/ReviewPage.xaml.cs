using RealWorldApp.Models.ModelsProd;
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
    public partial class ReviewPage : ContentPage
    {
        AddressDto _address { get; set; }
        CustomerBasket _basket { get; set; }
        public ObservableCollection<BasketItemDto> ShoppingCartCollection;
        double subTotalPrice { get; set; }
        public ReviewPage(AddressDto address, CustomerBasket basket)
        {
            InitializeComponent();
            ShoppingCartCollection = new ObservableCollection<BasketItemDto>();
            _address = address;
            _basket = basket;
            SetUpReview();
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void BtnProceed_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PaymentPage(_address));
        }

        private void SetUpReview()
        {
            var shoppingCartItems = _basket;

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
            //Subtotal
            for (int i = 0; i < shoppingCartItems.Items.Count; i++)
            {
                subTotalPrice += (shoppingCartItems.Items[i].Quantity * shoppingCartItems.Items[i].Price);
            }
            SubTotalPriceLbl.Text = $"£{subTotalPrice.ToString()}";
            //Delivery
            if (_basket.DeliveryMethod == 0)
            {
                DeliveryOptionsLbl.Text = "Free Delivery";
            }
            else if (_basket.DeliveryMethod == 1)
            {
                DeliveryOptionsLbl.Text = "Pick up yourself (- 10%)";
            }
            else if (_basket.DeliveryMethod > 1)
            {
                DeliveryOptionsLbl.Text = $"£{_basket.DeliveryMethod}";
            }
            //Discounts
            if ((_basket.acumDisc == true || _basket.refDisc == true) && _basket.promoDisc.Length > 8)
            {
                _basket.promoDisc = "1";
            }

            if (_basket.acumDisc == false && _basket.refDisc == false && _basket.promoDisc.Length<8)
            {
                DiscountsLbl.Text = "No discounts have been selrcted";
            }
            else
            {
                if (_basket.acumDisc == true && _basket.refDisc == true)
                {
                    DiscountsLbl.Text = "-20%";
                }
                else if ((_basket.acumDisc == false && _basket.refDisc == true) || (_basket.acumDisc == true && _basket.refDisc == false))
                {
                    DiscountsLbl.Text = "-10%";
                }
                else if (_basket.promoDisc.Length > 8)
                {
                    DiscountsLbl.Text = "-10%";
                }
            }

            //Total
            double mult = 1;
            if (_basket.DeliveryMethod == 1)
            {
                mult -= 0.1;
            }
            if ((_basket.acumDisc == false && _basket.refDisc == true) || (_basket.acumDisc == true && _basket.refDisc == false))
            {
                mult -= 0.1;
            }
            else if (_basket.acumDisc == true && _basket.refDisc == true)
            {
                mult -= 0.2;
            }
            else if (_basket.promoDisc.Length > 8)
            {
                mult -= 0.1;
            }
            var subT = subTotalPrice;
            subT *= mult;
            if (_basket.DeliveryMethod > 1)
            {
                subT += _basket.DeliveryMethod;
            }
            TotalPriceLbl.Text = $"£ {subT}";

        }
    }
}