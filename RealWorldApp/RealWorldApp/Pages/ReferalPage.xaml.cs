using Microsoft.AspNetCore.SignalR.Client;
using RealWorldApp.Models.ModelsProd;
using RealWorldApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
        HubConnection _connection;
        ICustomNotification notification;
        public ObservableCollection<ReferalsDto> ReferalsCollection;
        public string MyCode { get; set; }

        public ReferalPage()
        {
            InitializeComponent();
            notification = DependencyService.Get<ICustomNotification>();
            ReferalsCollection = new ObservableCollection<ReferalsDto>();
            GetMyCode();
            GetReferals();
        }
        protected async override void OnAppearing()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{AppSettings.ApiUrlProd}hub/toastr", options =>
                {
                    options.HttpMessageHandlerFactory = (message) =>
                    {
                        if (message is HttpClientHandler clientHandler)
                            // bypass SSL certificate
                            clientHandler.ServerCertificateCustomValidationCallback +=
                                                    (sender, certificate, chain, sslPolicyErrors) => { return true; };
                        return message;
                    };
                })
                .Build();

            await _connection.StartAsync();

            base.OnAppearing();
        }
        public void CreateConnection()
        {

            if (_connection.State == HubConnectionState.Connected)
            {
                notification.send("Message", "Connected");
            }

            _connection.On<string>("ReceiveMessage", (message) =>
            {
                notification.send("FromServer", message);
            }
            );
        }

        private async void GetReferals()
        {
            var referals = await ApiService.GetAllReferals();
            foreach (var referal in referals)
            {
                ReferalsCollection.Add(referal);
            }
            LvReferals.ItemsSource = ReferalsCollection;
        }
        public async void GetMyCode()
        {
            UserDto user = await ApiService.GetUser();
            MyCode = user.MyRefCode;
            ButtonCode.Text = MyCode;
            LinkButton.Text = $"{AppSettings.ApiUrlProd}account/register?ref={MyCode}";
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void ButtonCode_Button(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(MyCode);
            await DisplayAlert("", "Code Copied", "OK");
        }

        private async void LinkButtin_Button(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(LinkButton.Text);
            await DisplayAlert("", "Link Copied", "OK");
        }

        private void Send_notif(object sender, EventArgs e)
        {
            CreateConnection();
            //var t = Task.Run(async delegate
            //{
            //    await Task.Delay(5000);
            //    notification.send("New Order", "New Order Created");
            //});
            //t.Wait();
        }
    }
    public interface ICustomNotification 
    {
        void send(string title, string message);
    }
}