using System;
using System.Collections.Generic;
using System.Text;

namespace RealWorldApp.Models.ModelsProd
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Ingridients { get; set; }
        public double Price { get; set; }
        public string PictureUrl { get; set; }
        public string ProductType { get; set; }
        public double? minutesToMake { get; set; }
    }
}
