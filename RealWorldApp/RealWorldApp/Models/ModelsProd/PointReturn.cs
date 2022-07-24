using System;
using System.Collections.Generic;
using System.Text;

namespace RealWorldApp.Models.ModelsProd
{
    public class PointReturn
    {
        public bool Enable { get; set; }
        public string PointNumber { get; set; }
        public bool FreeZone { get; set; }
        public double DeliveryCost { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string PostCode { get; set; }
    }
}
