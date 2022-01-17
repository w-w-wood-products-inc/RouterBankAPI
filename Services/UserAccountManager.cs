using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DefaultNamespace;
using demoWebAPI.models;
using FirstAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace FirstAPI.Services; 

public class UserAccountManager : IUserAccountManager {
    private Repository    _repository;
    private IConfiguration _config;

    public UserAccountManager(IConfiguration config) {
        _repository = new Repository(config);
        _config      = config;
    }
    
    // 
    // 
    
    /// <summary>
    /// Authenticates the user login and returns a JWT token to the user containing
    /// the bankAccount accountids they own in the claims section
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    /// <returns>A JWT Token</returns>
    public async Task<string?> Login(string username, string password) {
        UserAccount? result = await _repository.GetUserAccountByLogin(username, password);

        // Only continues if the user entered correct login credentials
        if (result != null) {
            Token token = new Token(_config);
            
            return await token.GenerateToken(result.Snn);
        }

        return null;
    }

    public async Task<UserAccount?> GetUserAccount(string snn) {
        return await _repository.GetUserAccountBySnn(snn);
    }
    
    public async Task<UserAccount?> GetUserAccount(int accountid) {
        return await _repository.GetUserAccountByAccountid(accountid);
    }

    public async Task<string> CreateUserAccount(UserAccount userAccount) {
        if (
            userAccount.Name     != null && userAccount.Name.Length     <= 128 &&
            userAccount.Username != null && userAccount.Username.Length <= 128 && 
            userAccount.Pass     != null && userAccount.Pass.Length     == 64  && 
            userAccount.Addr     == null || userAccount.Addr.Length     <= 128 &&
            userAccount.Phone    == null || userAccount.Phone.Length    == 10  && userAccount.Phone.All(char.IsDigit) &&
            userAccount.Snn      != null && userAccount.Snn.Length      == 9   && userAccount.Snn.All(char.IsDigit)
        ) {
            int result = await _repository.CreateUserAccount(userAccount);
            if (result == 1) {
                Token token = new Token(_config);
                return await token.GenerateToken(userAccount.Snn);
            }
        }

        return null;
    }

    public async Task<int> UpdateUserAccount(UserAccount userAccount) {
        if (
            userAccount.Name     != null && userAccount.Name.Length     <= 128 &&
            userAccount.Username != null && userAccount.Username.Length <= 128 && 
            userAccount.Pass     != null && userAccount.Pass.Length     == 64  && 
            userAccount.Addr     == null || userAccount.Addr.Length     <= 128 &&
            userAccount.Phone    == null || userAccount.Phone.Length    == 10  && userAccount.Phone.All(char.IsDigit) &&
            userAccount.Snn      != null && userAccount.Snn.Length      == 9   && userAccount.Snn.All(char.IsDigit)
        ) {
            return await _repository.UpdateUserAccount(userAccount);
        }

        return 0;
    }

    public async Task<int> DeleteUserAccount(UserAccount userAccount) {
        return await _repository.DeleteUserAccountBySnn(userAccount.Snn);
    }
}
