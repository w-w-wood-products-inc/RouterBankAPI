using demoWebAPI.models;

namespace FirstAPI.Services.Interfaces; 

public interface IUserAccountManager {
    public Task<UserAccount?> GetUserAccount(string snn);
    public Task<string?> Login(string username, string password);
    public Task<UserAccount?> GetUserAccount(int accountid);
    public Task<string> CreateUserAccount(UserAccount userAccount);
    public Task<int> UpdateUserAccount(UserAccount userAccount);
    public Task<int> DeleteUserAccount(UserAccount userAccount);
}