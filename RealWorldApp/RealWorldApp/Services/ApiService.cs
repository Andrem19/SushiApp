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

        public static async Task<List<Category>> GetCategories()
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/product/category");
            return JsonConvert.DeserializeObject<List<Category>>(response);
        }

        public static async Task<ProductToReturnDto> GetProductById(int productId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/Product/" + productId);
            return JsonConvert.DeserializeObject<ProductToReturnDto>(response);
        }

        public static async Task<List<ProductByCategory>> GetProductByCategory(int categoryId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Products/ProductsByCategory/" + categoryId);
            return JsonConvert.DeserializeObject<List<ProductByCategory>>(response);
        }

        public static async Task<List<ProductToReturnDto>> GetProducts()
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/product/getallproducts");
            return JsonConvert.DeserializeObject<List<ProductToReturnDto>>(response);
        }
        public static async Task<CustomerBasket> GetBasketById(string basket_id)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrlProd + "api/basket?id=" + basket_id);
            return JsonConvert.DeserializeObject<CustomerBasket>(response);
        }
        public static async Task<CustomerBasket> CreateBasket(CustomerBasket basket)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(basket);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PostAsync(AppSettings.ApiUrlProd + "api/basket", content);
            var resp = await response.Content.ReadAsStringAsync();
            CustomerBasket basket_response = JsonConvert.DeserializeObject<CustomerBasket>(resp);
            return basket_response;
        }

        public static async Task<CustomerBasket> AddItemToBasket(CustomerBasket basket)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(basket);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PutAsync(AppSettings.ApiUrlProd + "api/basket", content);
            var resp = await response.Content.ReadAsStringAsync();
            CustomerBasket basket_response = JsonConvert.DeserializeObject<CustomerBasket>(resp);
            return basket_response;
        }

        public static async Task<CartSubTotal> GetCartSubTotal(int userId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/ShoppingCartItems/SubTotal/" + userId);
            return JsonConvert.DeserializeObject<CartSubTotal>(response);
        }

        public static async Task<CustomerBasket> GetBasket(string basket_id)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Basket?id=" + basket_id);
            return JsonConvert.DeserializeObject<CustomerBasket>(response);
        }

        public static async Task<TotalCartItem> GetTotalCartItems(int userId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/ShoppingCartItems/TotalItems/" + userId);
            return JsonConvert.DeserializeObject<TotalCartItem>(response);
        }



        public static async Task<bool> ClearShoppingCart(int userId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.DeleteAsync(AppSettings.ApiUrl + "api/ShoppingCartItems/" + userId);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public static async Task<OrderResponse> PlaceOrder(Order order)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/Orders", content);
            var jsonResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OrderResponse>(jsonResult);
        }

        public static async Task<List<OrderByUser>> GetOrdersByUser(int userId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Orders/OrdersByUser/" + userId);
            return JsonConvert.DeserializeObject<List<OrderByUser>>(response);
        }

        public static async Task<List<Order>> GetOrderDetails(int orderId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Orders/OrderDetails/" + orderId);
            return JsonConvert.DeserializeObject<List<Order>>(response);
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
                await ApiService.Login(email,password);
            }
        }
    }
}
