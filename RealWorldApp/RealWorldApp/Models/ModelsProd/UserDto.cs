using System;
using System.Collections.Generic;
using System.Text;

namespace RealWorldApp.Models.ModelsProd
{
    public class UserDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string telegramBotChatId { get; set; }
        public string MyRefCode { get; set; }
        public int RefDiscount { get; set; }
        public bool AcumDiscount { get; set; }
        public int Point { get; set; }
        public IList<string> Roles { get; set; }
        public AddressDto Address { get; set; }
    }
}
