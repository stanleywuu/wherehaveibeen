using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WhereHaveIBeen.Mobile.Data;

namespace WhereHaveIBeen.Mobile.ApiClient
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T Data { get; set; }
    }

    public static class ApiCalls
    {
        public static async Task<ApiResponse<LoginResponse>> Login(string username, string password)
        {
            try
            {
                var rawContent = JsonConvert.SerializeObject(new
                {
                    Username = username,
                    Password = password
                });

                var content = new StringContent(rawContent);

                var response = await Client.Instance.PostAsync("membership/login", content);
                var responseContent = await response.Content?.ReadAsStringAsync();

                return new ApiResponse<LoginResponse>()
                {
                    StatusCode = response.StatusCode,
                    Data = string.IsNullOrEmpty(responseContent) ? null
                    : JsonConvert.DeserializeObject<LoginResponse>(responseContent)
                };
            }
            catch
            {
                return new ApiResponse<LoginResponse>()
                {
                    Data = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
