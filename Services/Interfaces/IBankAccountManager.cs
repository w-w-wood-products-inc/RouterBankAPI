using System.Security.Claims;
using demoWebAPI.models;

namespace FirstAPI.Services.Interfaces; 

public interface IBankAccountManager {
    public bool AuthenticateBankAccount(ClaimsPrincipal claims, int accountIdToCheck);
    public List<int>? GetBankAccountIds(ClaimsPrincipal claims);
    public Task<BankAccount?> GetBankAccount(int accountid);
    public Task<List<Transact?>> GetTransactionHistory(int accountid);
    public Task<string> CreateBankAccount(BankAccount bankAccount);
    public Task<int> DepositSavings(double amount, int accountid);
    public Task<int> WithdrawSavings(double amount, int accountid);
    public Task<int> DepositChecking(double amount, int accountid);
    public Task<int> WithdrawChecking(double amount, int accountid);
    public Task<int> UpdateMpr(double amount, int accountid);
    public Task<int> UpdateMprEnable(bool enabled, int accountid);

    public Task<int> Transfer(double amount, string transferTo, int accountid);
    public Task<int> DeleteBankAccount(int accountid);
}