using RealWorldApp.Models.ModelsProd;
using RealWorldApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealWorldApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReferalPage : ContentPage
    {

        public string MyCode { get; set; }
        public ReferalPage()
        {
            InitializeComponent();
            GetMyCode();
        }
        public async void GetMyCode()
        {
            UserDto user = await ApiService.GetUser();
            MyCode = user.MyRefCode;
            Code.Text = MyCode;
            Link.Text = $"{AppSettings.ApiUrlProd}account/register?ref={MyCode}";
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void CopyLink(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(Link.Text);
            await DisplayAlert("", "Link Copied", "OK");
        }

        private async void CopyCode(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(MyCode);
            await DisplayAlert("", "Code Copied", "OK");
        }
    }
}