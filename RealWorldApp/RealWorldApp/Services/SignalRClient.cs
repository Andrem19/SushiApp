using Microsoft.AspNetCore.SignalR.Client;
using RealWorldApp.Pages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RealWorldApp.Services
{
    public class SignalRClient
    {
        public static async Task NewOrderCreated(string paymentId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            await httpClient.GetAsync(AppSettings.ApiUrlProd + "api/Notification?paymentId=" + paymentId);

        }
        public static async Task OrderReady(string id)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            await httpClient.GetAsync(AppSettings.ApiUrlProd + "api/Notification/orderReady?id=" + id);

        }

    }
}