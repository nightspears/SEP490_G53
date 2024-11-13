using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class AreaController : Controller
    {
        private readonly HttpClient _httpClient;

        public AreaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");

        }
        public async Task<IActionResult> Index(int matchId)
        {
            string customerId = Request.Cookies["CustomerId"];
            string requestUri = "https://localhost:5000/api/SupplementaryItem";
            var response = await _httpClient.GetAsync(requestUri);
            List<SupplementaryIteamViewModel> newList = new List<SupplementaryIteamViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                newList = JsonConvert.DeserializeObject<List<SupplementaryIteamViewModel>>(jsonData);
            }

            // Fetch match area tickets by matchId
            string matchAreaTicketsUri = $"https://localhost:5000/api/Area/getmatchareaticketsbymatchid/{matchId}";
            var matchAreaResponse = await _httpClient.GetAsync(matchAreaTicketsUri);
            List<MatchAreaTicketViewModel> matchAreaTickets = new List<MatchAreaTicketViewModel>();
            if (matchAreaResponse.IsSuccessStatusCode)
            {
                var matchAreaData = await matchAreaResponse.Content.ReadAsStringAsync();
                matchAreaTickets = JsonConvert.DeserializeObject<List<MatchAreaTicketViewModel>>(matchAreaData);
            }

            ViewData["NewList"] = newList;
            ViewData["CustomerId"] = customerId;
            ViewData["MatchAreaTickets"] = matchAreaTickets;

            return View();
        }

        public class MatchAreaTicketViewModel
        {
            public int MatchId { get; set; }
            public int AreaId { get; set; }
            public int? AvailableSeats { get; set; }
        }



    }
}
