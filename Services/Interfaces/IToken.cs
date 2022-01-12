namespace FirstAPI.Services.Interfaces; 

public interface IToken {
    public string GenerateToken(string ssn);
}