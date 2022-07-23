using System;
using System.Collections.Generic;
using System.Text;

namespace RealWorldApp.Models.ModelsProd
{
    public class BasketItemDto
    {
        public int Id { get; set; }
        public int IdProd { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }
        public string Type { get; set; }
        public double Total { get; set; }
    }
}
