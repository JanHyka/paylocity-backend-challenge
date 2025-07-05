namespace Api.Models;

public class Paycheck
{
    public int EmployeeId { get; set; }
    public decimal GrossPay { get; set; }
    public decimal BenefitsCost { get; set; }
    public decimal NetPay => GrossPay - BenefitsCost;

    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
}
