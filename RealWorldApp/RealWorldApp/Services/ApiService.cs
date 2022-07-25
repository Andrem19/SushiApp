using Newtonsoft.Json;
using RealWorldApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Net.Http.Headers;
using UnixTimeStamp;
using RealWorldApp.Models.ModelsProd;

namespace RealWorldApp.Services
{
    public static class ApiService
    {
        public static async Task<bool> RegisterUser(string name, string email, string password)
        {
            var register = new Register()
            {
                Name = name,
                Email = email,
                Password = password
            };
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(register);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrlProd + "api/Account/Register", content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public static async Task<bool> Login(string email, string password)
        {
            var login = new Login()
            {
                Email = email,
                Password = password
            };

            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrlProd + "api/Account/Login", content);
            if (!response.IsSuccessStatusCode) return false;
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserDto>(jsonResult);
            Preferences.Set("accessToken", result.Token);
            Preferences.Set("userName", result.DisplayName);
            Preferences.Set("Email", result.Email);
            return true;
        }
        public static async Task<bool> FBLogin(string token, string referal = "0")
        {
            var login = new FBLogin()
            {
                AccessToken = token,
                ReferalCode = referal
            };

            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrlProd + "api/Account/loginfb", content);
            if (!response.IsSuccessStatusCode) return false;
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserDto>(jsonResult);
            Preferences.Set("accessToken", result.Token);
            Preferences.Set("userName", result.DisplayName);
            Preferences.Set("Email", result.Email);
            return true;
        }
        public static async Task<UserDto> GetUser()
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetAsync(AppSettings.ApiUrlProd + "api/Account/currentUser");
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserDto>(jsonResult);
            return result;
        }
        public static async Task<bool> CheckPromo(string promo)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetAsync(AppSettings.ApiUrlProd + "api/Discount/promocode?promo=" + promo);
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<bool>(jsonResult);
            return result;
        }

        public static async Task<AddressDto> GetUserAddress()
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetAsync(AppSettings.ApiUrlProd + "api/Account/address");
            var resp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AddressDto>(resp);
            return result;
        }

        public static async Task<List<Category>> GetCategories()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/product/category");
            return JsonConvert.DeserializeObject<List<Category>>(response);
        }

        public static async Task<List<ProductToReturnDto>> GetProducts()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/product/getallproducts");
            return JsonConvert.DeserializeObject<List<ProductToReturnDto>>(response);
        }
        public static async Task<List<ProductToReturnDto>> GetProductsByCategory(string category)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/product/getallproducts?category=" + category);
            return JsonConvert.DeserializeObject<List<ProductToReturnDto>>(response);
        }
        public static async Task<CustomerBasket> CreateBasket(CustomerBasket basket)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(basket);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PostAsync(AppSettings.ApiUrlProd + "api/basket", content);
            var resp = await response.Content.ReadAsStringAsync();
            CustomerBasket basket_response = JsonConvert.DeserializeObject<CustomerBasket>(resp);
            return basket_response;
        }
        public static async Task<AddressDto> UpdateAddress(AddressDto address)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(address);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PutAsync(AppSettings.ApiUrlProd + "api/account/address", content);
            var resp = await response.Content.ReadAsStringAsync();
            AddressDto addressReturned = JsonConvert.DeserializeObject<AddressDto>(resp);
            return addressReturned;
        }
        public static async Task<PointReturn> WhereIAm(AddressDto address)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(address);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PutAsync(AppSettings.ApiUrlProd + "api/map/where", content);
            var resp = await response.Content.ReadAsStringAsync();
            PointReturn basket_response = JsonConvert.DeserializeObject<PointReturn>(resp);
            return basket_response;
        }
        public static async Task<CustomerBasket> AddItemToBasket(CustomerBasket basket)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(basket);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PutAsync(AppSettings.ApiUrlProd + "api/basket", content);
            var resp = await response.Content.ReadAsStringAsync();
            CustomerBasket basket_response = JsonConvert.DeserializeObject<CustomerBasket>(resp);
            return basket_response;
        }

        public static async Task<CustomerBasket> GetBasket(string basket_id)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/basket?id=" + basket_id);
            return JsonConvert.DeserializeObject<CustomerBasket>(response);
        }
        public static async Task<bool> DeleteBasket(string basket_id)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.DeleteAsync(AppSettings.ApiUrlProd + "api/Basket?id=" + basket_id);
            return true;
        }

        public static async Task<bool> CreatePaymentIntent(string basket_id)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            await httpClient.PostAsync(AppSettings.ApiUrlProd + "api/payments/" + basket_id, null);
            return true;
        }

        public static async Task<OrderResponse> PlaceOrder(OrderToCreate order)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PostAsync(AppSettings.ApiUrlProd + "api/Orders", content);
            var jsonResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OrderResponse>(jsonResult);
        }

        public static async Task<List<OrderResponse>> GetOrdersByUser()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/Orders");
            return JsonConvert.DeserializeObject<List<OrderResponse>>(response);
        }

        public static async Task<OrderResponse> GetOrderDetails(int orderId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/Orders/" + orderId);
            return JsonConvert.DeserializeObject<OrderResponse>(response);
        }

    }
    public static class TokenValidator
    {
        public static async Task CheckTokenValidity()
        {
            var expirationTime = Preferences.Get("tokenExpirationTime", 0);
            Preferences.Set("currentTime", UnixTime.GetCurrentTime());
            var currentTime = Preferences.Get("currentTime", 0);
            if (expirationTime < currentTime)
            {
                var email = Preferences.Get("email", string.Empty);
                var password = Preferences.Get("password", string.Empty);
                await ApiService.Login(email, password);
            }
        }
    }
}
