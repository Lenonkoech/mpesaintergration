using System;

namespace mpesaintergration.Controllers;

public class MpesaSettings
{
    public string ConsumerKey { get; set; }
    public string ConsumerSecret { get; set; }
    public string Shortcode { get; set; }
    public string ValidationURL { get; set; }
    public string ConfirmationURL { get; set; }
}
