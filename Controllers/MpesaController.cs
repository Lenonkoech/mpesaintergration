using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using mpesaintergration.Data;
using mpesaintergration.Models;
using Newtonsoft.Json;

namespace mpesaintergration.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly MpesaSettings _mpesaSettings;
    private readonly ApplicationDbContext _dbContext;

    public HomeController(
        ILogger<HomeController> logger,
        IHttpClientFactory clientFactory,
        ApplicationDbContext dbContext,
        IOptionsSnapshot<MpesaSettings> mpesaSettings)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _dbContext = dbContext;
        _mpesaSettings = mpesaSettings.Value;
    }

    public IActionResult Index() => View();
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    private record Token(string Access_Token, string Expires_In);

    // Access token
    public async Task<string> GetToken()
    {
        var client = _clientFactory.CreateClient("mpesa");

        var credentials = $"{_mpesaSettings.ConsumerKey}:{_mpesaSettings.ConsumerSecret}";
        var encodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));

        var request = new HttpRequestMessage(HttpMethod.Get, "/oauth/v1/generate?grant_type=client_credentials");
        request.Headers.Add("Authorization", $"Basic {encodedCredentials}");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<Token>(responseBody);

        return token?.Access_Token ?? throw new Exception("Failed to parse token from M-Pesa API.");
    }
    // C2B payment intergrations
    [HttpGet]
    [Route("register-urls")]
    public async Task<IActionResult> RegisterMpesaUrls()
    {
        var payload = new
        {
            _mpesaSettings.ShortCode,
            ResponseType = "Completed",
            _mpesaSettings.ConfirmationURL,
            _mpesaSettings.ValidationURL,


        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        var token = await GetToken();
        var client = _clientFactory.CreateClient("mpesa");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await client.PostAsync("/mpesa/c2b/v1/registerurl", content);
        var result = await response.Content.ReadAsStringAsync();

        return Content(result, "application/json");
    }

    public IActionResult Paymentrequest() => View();

    [HttpPost]
    [Route("makePayment")]

    public async Task<IActionResult> MakePayment()
    {
        var payment = new
        {
            _mpesaSettings.ShortCode,
            _mpesaSettings.CommandID,
            Amount = 1,
            _mpesaSettings.Msisdn,
            _mpesaSettings.BillRefNumber
        };
        var content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");
        var token = await GetToken();
        var client = _clientFactory.CreateClient("mpesa");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("/mpesa/c2b/v1/simulate", content);
        var result = await response.Content.ReadAsStringAsync();
        return Content(result, "application/json");
    }

    [HttpPost]
    [Route("payments/confirmation")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> PaymentConfirmation([FromBody] MpesaC2B c2bPayments)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { code = 0, errors = ModelState });
        }

        await _dbContext.AddAsync(c2bPayments);
        await _dbContext.SaveChangesAsync();

        return Json(c2bPayments);
    }

    [HttpPost]
    [Route("payments/validation")]
    public IActionResult PaymentValidation([FromBody] MpesaC2B c2bPayments)
    {
        var response = new
        {
            ResponseCode = 0,
            ResponseDesc = "Processed"
        };

        return Json(response);
    }
    // Mpesa Express Intergrations

    [HttpPost]
    [Route("mpesaExpressPayment")]
    public async Task<IActionResult> MpesaExpress([FromForm] string phonenumber)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string password = GeneratePassword(_mpesaSettings.BusinessShortCode,
        _mpesaSettings.PassKey, timestamp);

        var payment = new
        {
            _mpesaSettings.BusinessShortCode,
            _mpesaSettings.TransactionType,
            _mpesaSettings.Amount,
            PartyA = phonenumber,
            _mpesaSettings.PartyB,
            PhoneNumber = phonenumber,
            _mpesaSettings.CallBackURL,
            _mpesaSettings.AccountReference,
            _mpesaSettings.TransactionDesc,
            Password = password,
            Timestamp = timestamp
        };
        var content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");
        var token = await GetToken();
        var client = _clientFactory.CreateClient("mpesa");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("/mpesa/stkpush/v1/processrequest", content);
        var result = await response.Content.ReadAsStringAsync();
        return Content(result, "application/json");
    }
    public static string GeneratePassword(string shortCode, string passKey, string timestamp)
    {
        string rawPassword = shortCode + passKey + timestamp;
        byte[] bytes = Encoding.UTF8.GetBytes(rawPassword);
        return Convert.ToBase64String(bytes);
    }
}
