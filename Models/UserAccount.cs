namespace demoWebAPI.models;

public class UserAccount {
    public string Name { get; set; }
    public string Username { get; set; }
    public string Pass { get; set; }
    public string Birthdate { get; set; }
    public string Addr { get; set; }
    public string Phone { get; set; }
    public string Snn { get; set; }

    public override string ToString() {
        return $"({Name}, {Username}, {Pass}, {Birthdate}, " +
               $"{Addr}, {Phone}, {Snn})";
    }
}