using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class AZCotizaciones
{
    private readonly HttpClient _httpClient;

    public AZCotizaciones(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [Function("PostToApi")]
    public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, FunctionContext context)
    {
        var logger = context.GetLogger("PostToApi");
        logger.LogInformation($"Funcion ejecutada: {DateTime.UtcNow}");


        string baseUrl = Environment.GetEnvironmentVariable("VITE_REACT_APP_API_URL");
        string apiUrl = $"{baseUrl}cotizaciondolar/actualizar";
        string apiKey = Environment.GetEnvironmentVariable("VITE_REACT_APP_API_KEY");


        var requestData = new
        {

        };

        var requestJson = JsonSerializer.Serialize(requestData);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        // API Key a los headers
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

        try
        {
            //post
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation($"Éxito: {responseContent}");
            }
            else
            {
                logger.LogError($"Error: {response.StatusCode} - {responseContent}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Excepción: {ex.Message}");
        }
    }
}
