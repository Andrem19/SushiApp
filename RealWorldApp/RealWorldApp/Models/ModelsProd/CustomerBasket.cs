using System;
using System.Collections.Generic;
using System.Text;

namespace RealWorldApp.Models.ModelsProd
{
    public class CustomerBasket
    {
        public int Id { get; set; }
        public string basket_id { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public string Point { get; set; }
        public bool refDisc { get; set; }
        public bool acumDisc { get; set; }
        public string promoDisc { get; set; }
        public string ClientSecret { get; set; }
        public string PaymentIntentId { get; set; }
        public string ShippingPrice { get; set; }
        public double DeliveryMethod { get; set; }
    }
}
