namespace Api.Dtos.Paycheck;

public class GetPaycheckDto
{
    /// <summary>
    /// Employee's unique identifier.
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// The employee's gross pay for the pay period.
    /// </summary>
    public decimal GrossPay { get; set; }
    
    /// <summary>
    /// The total cost of benefits deducted from the gross pay.
    /// </summary>
    public decimal BenefitsCost { get; set; }

    /// <summary>
    /// The net pay after deducting benefits from the gross pay.
    /// </summary>
    public decimal NetPay { get; set; }
    
    /// <summary>
    /// The start date of the pay period.
    /// </summary>
    public DateTime PayPeriodStart { get; set; }
    
    /// <summary>
    /// The end date of the pay period.
    /// </summary>
    public DateTime PayPeriodEnd { get; set; }
}
