using System;

namespace mpesaintergration.Controllers;

public class MpesaSettings
{
    public required string ConsumerKey { get; set; }
    public required string ConsumerSecret { get; set; }
    public required string Shortcode { get; set; }
    public required string ValidationURL { get; set; }
    public required string ConfirmationURL { get; set; }
}
