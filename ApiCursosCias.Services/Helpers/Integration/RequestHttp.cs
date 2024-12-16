using ApiCursosCias.Services.Helpers.Settings;
using System.Configuration;
using System.Text.Json;
using System.Text;

namespace ApiCursosCias.Services.Helpers.Integration;

public class RequestHttp
{
    //private static IntegrationSettings Settings { set; get; } = AppSettings.Settings.Integration;
    private static IntegrationSettings Settings { set; get; } = AppSettings.Settings.Integration;

    public enum TypeBody
    { Body, Query }

    public static async Task<TR> CallMethod<TR>(string service, string action, object model, HttpMethod method, TypeBody typeBody, string token = null)
    {
        try
        {
            TR dataobject = default;
            var _set = Settings;
            var _service = _set.Services.Where(s => s.Name.Equals(service)).FirstOrDefault();
            var _action = _service.Methods.Where(a => a.Method.Equals(action)).FirstOrDefault().Value;
            HttpContent content = null;
            if (model != null)
            {
                switch (typeBody)
                {
                    case TypeBody.Body:
                        string json = JsonSerializer.Serialize(model);
                        if (service.Equals("mensajeria"))
                            json = (string)model;
                        content = new StringContent(json, Encoding.UTF8, "application/json");
                        break;

                    case TypeBody.Query:
                        var queryParams = (Dictionary<string, string>)model;
                        _action += $"{string.Join("&", queryParams.Select(i => $"{i.Key}={i.Value}"))}";
                        content = new FormUrlEncodedContent(queryParams);
                        break;
                }
            }

            using var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

            using var client = new HttpClient(handler);
            using var request = new HttpRequestMessage(method, $"{_service.Url}/{_action}");

            request.Content = content;

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (response.Content != null)
            {
                var result = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(result))
                    if (response.IsSuccessStatusCode)
                        dataobject = JsonSerializer.Deserialize<TR>(result);
                    else
                        throw new Exception(result);
            }
            return dataobject;
        }
        catch
        {
            throw;
        }
    }
}