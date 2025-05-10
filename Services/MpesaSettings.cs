namespace mpesaintergration.Controllers;

public class MpesaSettings
{
    public required string ConsumerKey { get; set; }
    public required string ConsumerSecret { get; set; }
    public required string ShortCode { get; set; }
    public required string Msisdn { get; set; }
    public required string CommandID { get; set; }
    public required string ValidationURL { get; set; }
    public required string ConfirmationURL { get; set; }
    public required string BillRefNumber { get; set; }

    // STK Push
    public required string TransactionType { get; set; }
    public required string Amount { get; set; }
    public required string PartyA { get; set; }
    public required string PartyB { get; set; }
    public required string PhoneNumber { get; set; }
    public required string CallBackURL { get; set; }
    public required string AccountReference { get; set; }
    public required string TransactionDesc { get; set; }
    public required string BusinessShortCode { get; set; }
    public required string PassKey { get; set; }
}
