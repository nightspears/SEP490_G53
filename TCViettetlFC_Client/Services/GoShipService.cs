using Newtonsoft.Json;
using System.Net.Http.Headers;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Services
{
    public class GoShipService
    {
        private readonly HttpClient _httpClient;

        public GoShipService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }



        public async Task<ShipmentResponse> GetShipmentAsync(string shipmentCode)
        {
            var requestUrl = $"https://sandbox.goship.io/api/v2/shipments/search?code={shipmentCode}";

            // Add authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6ImRlYmI4YjNlYTc3OTYzY2JhZWNiNmY5M2ZjOWQ5M2QwZjRhNzRmOTBjZTgyMDc2MjlmM2Q5ODcxY2M2MDYyMWU5N2EzZjgxNjMzNTVhOGMyIn0.eyJhdWQiOiIxMTYiLCJqdGkiOiJkZWJiOGIzZWE3Nzk2M2NiYWVjYjZmOTNmYzlkOTNkMGY0YTc0ZjkwY2U4MjA3NjI5ZjNkOTg3MWNjNjA2MjFlOTdhM2Y4MTYzMzU1YThjMiIsImlhdCI6MTcyNjc5ODk3MCwibmJmIjoxNzI2Nzk4OTcwLCJleHAiOjE3NTgzMzQ5NzAsInN1YiI6IjMzMTYiLCJzY29wZXMiOltdfQ.Rd_OwL2rf4B7VmpSYa4ryaEqL5K2QQCZ24pN-rWclHZ_Ltx7ItWtUrEz8SIXGODTe3vfDrO80KvsMkvRLlB0-e8h96jMoAMsDz8z5qpbekfQTaneFDdMbQPWLPWzYtbzhQv44p6oWDLI9g0gPNIwfLXMYqZvPwTXPh9PdJrY4zqiQKWbZnfZIgbR1ovPifqMmZwiszQd8y-wVgCxqvb8I4zWJkfCw-Qva081kOgx0wphAEbfngInpzs8EngFZg0tMyH-mKBL1d1Zx20DhqQbrzGBTRUkZjCur_x2jgwdUtwu36O3ESuHfCxI-I-qCAIbHssbKSCe1gtZS5UcK5M5-H60rsRfSBTEnU3hqW1RM1CBt2cGu0pzc3RGtOzwBXzhyJaORI6PI9oDrb2Z_S7riZO9xXiedz-EUNh1oO27--ltBksJs2wBuDUvUhNRi-bewXAxCo_gYbY0Dvc8urSrij6h7bo8wNcu2KjelCrbA16Mo_n-Lz6gL6IfYV3M072YSnGTTejZ0d3nodekXR6SKun_cx85Dvf7US5TMIC5Pfndm0b0Q9ly0fnEVD_8WN9_9tjZMrC98nrS1Hm-4oxHrWzAuW3OeotJlgze9h42exNxOjt438J4zUvJd6fRCXbog4ewVPiqQRvhRpaBBnqBrx_7FyjtOAOOVNA3rQq2jhw");

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode(); // Throw if not a success code.

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ShipmentResponse>(jsonResponse);
            }
            catch (HttpRequestException ex)
            {
                // Handle request exceptions here (e.g., log or throw)
                throw new Exception("Error fetching shipment data.", ex);
            }
        }
    }
}
