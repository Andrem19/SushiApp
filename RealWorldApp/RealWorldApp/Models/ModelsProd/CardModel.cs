using System;
using System.Collections.Generic;
using System.Text;

namespace RealWorldApp.Models.ModelsProd
{
    public class CardModel
    {
        public string Number { get; set; }
        public long ExpMonth { get; set; }
        public long ExpYear { get; set; }
        public string Cvc { get; set; }
    }
}
