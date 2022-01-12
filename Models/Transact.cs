namespace demoWebAPI.models; 

public class Transact {
    public int Acntid { get; set; }
    public string Act { get; set; }
    public double Amount { get; set; }
    public string Account { get; set; }
    public double Newbal { get; set; }
    public String TDate { get; set; }

    public Transact(int acntid, string act, double amount, string account, double newbal, string tdate) {
        this.Acntid = acntid;
        this.Act = act;
        this.Amount = amount;
        this.Account = account;
        this.Newbal = newbal;
        this.TDate = tdate;
    }
    

    public override string ToString() {
        return $"{Acntid}, {Act}, {Amount}, {Account}, {Newbal} {TDate}";
    }
}