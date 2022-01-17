using demoWebAPI.models;

namespace FirstAPI.Repositories; 

public interface IRepository {
    // Get
    public Task<UserAccount?>       GetUserAccountBySnn(string snn);
    public Task<UserAccount?>       GetUserAccountByLogin(string user, string pass);
    public Task<UserAccount?>       GetUserAccountByAccountid(int accountid);
    public Task<List<UserAccount?>> GetAllUserAccounts();
    public Task<BankAccount?>       GetBankAccountByAccountid(int accountid);
    public Task<List<BankAccount?>> GetBankAccountsBySnn(string snn);
    public Task<List<BankAccount?>> GetAllBankAccounts();
    public Task<List<Transact?>>    GetTransactionHistoryByAccountid(int accountid);
    
    // Post
    public Task<int> CreateUserAccount(UserAccount userAccount);
    public Task<int> CreateBankAccount(BankAccount bankAccount);
    public Task<int> CreateTransaction(Transact transact);
    
    // Put
    public Task<int> UpdateUserAccount(UserAccount userAccount);
    public Task<int> UpdateBankAccountCheckBal(double checkBal, int accountid);
    public Task<int> UpdateBankAccountSaveBal(double saveBal, int accountid);
    public Task<int> UpdateBankAccountMpr(double mpr, int accountid);
    public Task<int> UpdateBankAccountMprEnable(bool mprEnable, int accountid);
    
    // Delete
    public Task<int> DeleteUserAccountBySnn(string snn);
    public Task<int> DeleteBankAccountByAccountid(int accountid);

}