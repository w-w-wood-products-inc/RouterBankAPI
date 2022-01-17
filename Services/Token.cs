using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DefaultNamespace;
using demoWebAPI.models;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.IdentityModel.Tokens;

namespace FirstAPI.Services; 

public class Token {
    private Repository    _repository;
    private IConfiguration _config;

    public Token(IConfiguration config) {
        _repository = new Repository(config);
        _config      = config;
    }
    
    public async Task<string> GenerateToken(string snn) {

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
        // Obtains all the bank accounts the user owns
        List<BankAccount>? userBankAccounts = await _repository.GetBankAccountsBySnn(snn);
        List<int>          bankIdOwned      = new();
        List<Claim>        claims           = new();
            
        // Adds the account ids of all bank accounts the user owns to the token claims
        if (
            userBankAccounts       != null && 
            userBankAccounts.Count > 0
        ) {
            foreach (BankAccount account in userBankAccounts) {
                bankIdOwned.Add(account.Accountid);
            }

            string bankIdOwnedString = "";
            
            foreach (int id in bankIdOwned) {
                bankIdOwnedString += "" + id + " ";
            }
            
            bankIdOwnedString = bankIdOwnedString.Substring(0, bankIdOwnedString.Length - 1);

            claims.Add(new Claim("BankAccountsOwned", bankIdOwnedString));
        }
            
        // Generates the token and returns it
        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Issuer"],
            claims:             claims,
            expires:            DateTime.Now.AddHours(4),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}