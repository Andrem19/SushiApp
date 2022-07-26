using Microsoft.AspNetCore.SignalR.Client;
using RealWorldApp.Pages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RealWorldApp.Services
{
    public class SignalRClient
    {
        HubConnection _connection;
        ICustomNotification notification;

        public SignalRClient()
        {
            notification = DependencyService.Get<ICustomNotification>();
        }
        public async Task StartConnection()
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
            if (_connection.State == HubConnectionState.Connected)
            {
                notification.send("", "Connected");
            }
            else
            {
                notification.send("", "Disconected");
            }
        }

        public async Task Disconnect()
        {
            await _connection.StopAsync();
        }

        public async Task AuthMe(string email)
        {
            try
            {
                await _connection.InvokeAsync("authMe", email);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task SendMessage(string point)
        {
            await _connection.SendAsync("sendMsg", point);
        }

        public void ListenMessage()
        {
            _connection.On<string>("sendMsgResponse", (message) =>
            {
                notification.send("New Order", message);
            });
        }
        
    }
}