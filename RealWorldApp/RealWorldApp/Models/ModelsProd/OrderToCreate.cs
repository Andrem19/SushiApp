using System;
using System.Collections.Generic;
using System.Text;

namespace RealWorldApp.Models.ModelsProd
{
    public class OrderToCreate
    {
        public string BasketId { get; set; }
        public AddressDto ShipToAddress { get; set; }
    }
}
