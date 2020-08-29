using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImagesUpload.WEB.Utils
{
    public static class HttpClientHelper
    {
        public static async Task<IServiceResponse<TResult>> PostAsJsonAsync<TModel, TResult>(this HttpClient client, string requestUri, TModel model)
        {
            try 
            {
                var response = await client.PostAsJsonAsync(requestUri, model);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsAsync<ServiceResponse<TResult>>();
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
                return null;
            }          
        }

        public static async Task<IServiceResponse<TResult>> DeleteAsync<TResult>(this HttpClient client, string requestUri)
        {
            var response = await client.DeleteAsync(requestUri);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<ServiceResponse<TResult>>();
        }

        public static async Task<IServiceResponse<TResult>> PutAsync<TModel, TResult>(this HttpClient client, string requestUri, TModel model)
        {
            var response = await client.PutAsJsonAsync(requestUri, model);
            return await response.Content.ReadAsAsync<ServiceResponse<TResult>>();
        }

        public static async Task<IServiceResponse<TResult>> GetAsync<TResult>(this HttpClient client, string requestUri)
        {
            var response = await client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();

            var str = response.Content.ReadAsStringAsync();
            return await response.Content.ReadAsAsync<ServiceResponse<TResult>>();
        }

        //public static bool IsAjaxRequest(this HttpRequest request)
        //{
        //    if (request == null)
        //        throw new ArgumentNullException(nameof(request));

        //    return request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest";
        //}
    }
}
