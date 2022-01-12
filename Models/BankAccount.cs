namespace demoWebAPI.models; 

public class BankAccount {
    public string Ussn { get; set; }
    public int Accountid { get; set; }
    public double Checkbal { get; set; }
    public double Savebal { get; set; }
    public double Mpr { get; set; }
    public bool Mpr_enable { get; set; }

    public override string ToString() {
        return $"{Ussn}, {Accountid}, {Checkbal}, {Savebal}, {Mpr}, {Mpr_enable}";
    }
}