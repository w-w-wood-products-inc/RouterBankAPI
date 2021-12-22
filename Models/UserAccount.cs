namespace demoWebAPI.models;

public class UserAccount {
    public int userId { get; set; }
    public string name { get; set; }
    public string username { get; set; }
    public double savingsBalance { get; set; }
    public double checkingBalance { get; set; }
    public double minutePercentageRate { get; set; }
    public bool mprEnable { get; set; }

    public override string ToString() {
        return $"({userId}, {name}, {username}, {savingsBalance}, " +
               $"{checkingBalance}, {minutePercentageRate}, {mprEnable})";
    }
}