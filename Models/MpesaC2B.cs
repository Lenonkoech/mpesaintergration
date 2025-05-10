using System;

namespace mpesaintergration.Models;

public class MpesaC2B
{
    public int Id { get; set; }
    public string? TransactionType { get; set; }
    public string? TransID { get; set; }
    public string? TransTime { get; set; }
    public string? TransAmount { get; set; }
    public string? BusinessShortCode { get; set; }
    public string? BillRefNumber { get; set; }
    public string? OrgAccountBalance { get; set; }
    public string? ThirdPartyTransID { get; set; }
    public string? loan_id { get; set; }
    public string? MSISDN { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? src_ip_address { get; set; }
    public string? refferer_address { get; set; }
    public DateTime created_at { get; set; } = DateTime.Now;
}
