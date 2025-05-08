using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using mpesaintergration.Data;
using mpesaintergration.Models;
using Newtonsoft.Json;

namespace mpesaintergration.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly MpesaSettings _mpesaSettings;
    private ApplicationDbContext _dbcontext;
    public HomeController(ILogger<HomeController> logger,
    IHttpClientFactory clientFactory,
    ApplicationDbContext dbContext,
    IOptions<MpesaSettings> mpesaSettings)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _dbcontext = dbContext;
        _mpesaSettings = mpesaSettings.Value;
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    // Access token
    public async Task<string> GetToken()
    {
        var client = _clientFactory.CreateClient("mpesa");
        var authString = $"_{_mpesaSettings.ConsumerKey}:{_mpesaSettings.ConsumerSecret}";
        var encodedString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authString));
        var _url = "/oauth/v1/generate?grant_type=client_credentials";
        var request = new HttpRequestMessage(HttpMethod.Get, _url);
        request.Headers.Add("Authorization", $"Basic {encodedString}");
        var response = await client.SendAsync(request);
        var mpesaResponse = await response.Content.ReadAsStringAsync();
        Token tokenObject = JsonConvert.DeserializeObject<Token>(mpesaResponse);
        return tokenObject.access_token;
    }

    class Token
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }

    public IActionResult RegisterURLs()
    {
        return View();
    }
    [HttpGet]
    [Route("register-urls")]
    public async Task<string> RegisterMpesaUrls()
    {
        var jsonBody = JsonConvert.SerializeObject(new
        {
            ValidationURL = _mpesaSettings.ValidationURL,
            ConfirmationURL = _mpesaSettings.ConfirmationURL,
            ResponseType = "Completed",
            Shortcode = _mpesaSettings.Shortcode
        });

        var jsonBodyReady = new StringContent(
            jsonBody.ToString(),
            Encoding.UTF8,
            "application/json"
        );
        var token = await GetToken();
        var client = _clientFactory.CreateClient("mpesa");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var url = "/mpesa/c2b/v1/registerurl";
        var response = await client.PostAsync(url, jsonBodyReady);
        return await response.Content.ReadAsStringAsync();
    }
    //Confirmation endpoint
    [HttpPost]
    [Route("payments/confirmation")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<JsonResult> PaymentConfirmation([FromBody] MpesaC2B c2bPayments)
    {
        var respond = new
        {
            ResponseCode = 0,
            ResponseDesc = "Processed"
        };
        if (ModelState.IsValid)
        {
            _dbcontext.Add(c2bPayments);
            var saveResponse = await _dbcontext.SaveChangesAsync();
        }
        else
        {
            return Json(new { code = 0, errors = ModelState });
        }
        return Json(c2bPayments);
    }
    //Payment validation
    [HttpPost]
    [Route("payments/validation")]
    public async Task<JsonResult> PaymentValidation([FromBody] MpesaC2B c2bPayments)
    {
        var respond = new
        {
            ResponseCode = 0,
            ResponseDesc = "Processed"
        };
        return Json(respond);
    }
}
