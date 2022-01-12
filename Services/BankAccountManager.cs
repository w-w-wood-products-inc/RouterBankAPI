using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DefaultNamespace;
using demoWebAPI.models;
using FirstAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FirstAPI.Services; 

public class BankAccountManager: IBankAccountManager {
    private Repository _repository;
    private IConfiguration _config;

    public BankAccountManager(IConfiguration config) {
        _repository = new Repository(config);
        _config = config;
    }
    
    // Authenticates user access to a specific bank account by comparing the claims of the JWT token
    // to the bank account the user wishes to access
    public bool AuthenticateBankAccount(ClaimsPrincipal claims, int accountIdToCheck) {
             if (claims.HasClaim(c => c.Type == "BankAccountsOwned")) {
                 string parsedClaims = claims.Claims.FirstOrDefault(c => c.Type == "BankAccountsOwned").Value;
                 string[] bankAccountsOwnedString = parsedClaims.Split(" ");
                 
                 // Parses all claims from the JWT token into an integer list for easy comparing
                 List<int> bankAccountsOwned = new();
                 foreach (string bankAccountString in bankAccountsOwnedString) {
                     if (int.TryParse(bankAccountString, out int returnedValue)) {
                         bankAccountsOwned.Add(returnedValue);
                     }
                 }
                
                 // Checks to ensure the bank account the user wishes to access is in the JWT claims section
                 if (bankAccountsOwned.Contains(accountIdToCheck)) {
                     return true;
                 }
             }
     
             return false;
         }

    // Returns all bank account ids that a user has access to based on their JWT claims section
    public List<int>? GetBankAccountIds(ClaimsPrincipal claims) {
        if (claims.HasClaim(c => c.Type == "BankAccountsOwned")) {
            string parsedClaims = claims.Claims.FirstOrDefault(c => c.Type == "BankAccountsOwned").Value;
            string[] bankAccountsOwnedString = parsedClaims.Split(" ");
            
            // Parses all claims from the JWT token into an integer list for easy comparing
            List<int> bankAccountsOwned = new();
            foreach (string bankAccountString in bankAccountsOwnedString) {
                if (int.TryParse(bankAccountString, out int returnedValue)) {
                    bankAccountsOwned.Add(returnedValue);
                }
            }

            return bankAccountsOwned;
        }
     
        return null;
    }

    public async Task<BankAccount?> GetBankAccount(int accountid) {
        return await _repository.GetBankAccountByAccountid(accountid);
    }

    public async Task<List<Transact?>> GetTransactionHistory(int accountid) {
        return await _repository.GetTransactionHistoryByAccountid(accountid);
    }

    public async Task<string> CreateBankAccount(BankAccount bankAccount) {
        // Sets any null values that have a default value to that value
        if (bankAccount.Checkbal == null) bankAccount.Checkbal = 0;
        if (bankAccount.Savebal == null) bankAccount.Savebal = 0;
        if (bankAccount.Mpr == null) bankAccount.Mpr = 0.05;
        if (bankAccount.Mpr_enable == null) bankAccount.Mpr_enable = false;
        
        // Checks to ensure the bankAccount variables meets the database constraints
        if (
            bankAccount.Ussn != null && bankAccount.Ussn.Length == 9 && bankAccount.Ussn.All(char.IsDigit) &&
            bankAccount.Checkbal >= 0 &&
            bankAccount.Savebal > -0 &&
            bankAccount.Mpr >= 0 && bankAccount.Mpr <= 3
        ) {
            // Finds the next available accountid that is not occupied yet
            List<BankAccount> allBankAccounts = await _repository.GetAllBankAccounts();
            int count = 1;
            while (true) {
                if (allBankAccounts.FirstOrDefault(k => k.Accountid == count) == null) {
                    bankAccount.Accountid = count;
                    break;
                }
                count++;
            }

            // Attempts to insert a new bank account into the database and returns a new token if successful
            int result = await _repository.CreateBankAccount(bankAccount);
            if (result == 1) {
                Token token = new Token(_config);
                return await token.GenerateToken(bankAccount.Ussn);
            }
        }

        return "";
    }
    
    public async Task<int> DepositSavings(double amount, int accountid) {
        BankAccount? bankAccount = await _repository.GetBankAccountByAccountid(accountid);
        if (bankAccount != null && amount > 0) {
            bankAccount.Savebal += amount;
            if (
                await _repository.UpdateBankAccountSaveBal(bankAccount.Savebal, accountid) == 1 &&
                await _repository.CreateTransaction(new Transact(accountid, "deposit", amount, "savings",
                    bankAccount.Savebal, DateTime.Now.ToString(CultureInfo.GetCultureInfo("en-US")))) == 1
            ) {
                return 1;
            }
        }
        Console.WriteLine("LEEEDLEEEDLEEEEDDDLEEEEE");
        return 0;
    }

    public async Task<int> WithdrawSavings(double amount, int accountid) {
        BankAccount? bankAccount = await _repository.GetBankAccountByAccountid(accountid);
        if (bankAccount != null && amount > 0 && bankAccount.Savebal >= amount) {
            bankAccount.Savebal -= amount;
            if (
                await _repository.UpdateBankAccountSaveBal(bankAccount.Savebal, accountid) == 1 &&
                await _repository.CreateTransaction(new Transact(accountid, "withdraw", amount, "savings",
                    bankAccount.Savebal, DateTime.Now.ToString(CultureInfo.GetCultureInfo("en-US")))) == 1
            ) {
                return 1;
            }
        }

        return 0;
    }

    public async Task<int> DepositChecking(double amount, int accountid) {
        BankAccount? bankAccount = await _repository.GetBankAccountByAccountid(accountid);
        if (bankAccount != null && amount > 0) {
            bankAccount.Checkbal += amount;
            if (
                await _repository.UpdateBankAccountCheckBal(bankAccount.Checkbal, accountid) == 1 &&
                await _repository.CreateTransaction(new Transact(accountid, "deposit", amount, "checking",
                    bankAccount.Checkbal, DateTime.Now.ToString(CultureInfo.GetCultureInfo("en-US")))) == 1
            ) {
                return 1;
            }
        }

        return 0;
    }

    public async Task<int> WithdrawChecking(double amount, int accountid) {
        BankAccount? bankAccount = await _repository.GetBankAccountByAccountid(accountid);
        if (bankAccount != null && amount > 0 && bankAccount.Checkbal >= amount) {
            bankAccount.Checkbal -= amount;
            if (
                await _repository.UpdateBankAccountCheckBal(bankAccount.Checkbal, accountid) == 1 &&
                await _repository.CreateTransaction(new Transact(accountid, "withdraw", amount, "checking",
                    bankAccount.Checkbal, DateTime.Now.ToString(CultureInfo.GetCultureInfo("en-US")))) == 1
            ) {
                return 1;
            }
        }

        return 0;
    }

    public async Task<int> UpdateMpr(double amount, int accountid) {
        if (amount >= 0 && amount <= 3) {
            return await _repository.UpdateBankAccountMpr(amount, accountid);
        }

        return 0;
    }

    public async Task<int> UpdateMprEnable(bool enabled, int accountid) {
        return await _repository.UpdateBankAccountMprEnable(enabled, accountid);
    }

    public async Task<int> Transfer(double amount, string transferTo, int accountid) {
        BankAccount? bankAccount = await _repository.GetBankAccountByAccountid(accountid);

        if (bankAccount != null) {
            // Attempts to transfer money from savings to checking
            if (transferTo == "checking" && bankAccount.Savebal >= amount) {
                if (
                    await _repository.UpdateBankAccountSaveBal(bankAccount.Savebal - amount, accountid) == 1 &&
                    await _repository.CreateTransaction(new Transact(accountid, "withdraw", amount, "savings",
                        bankAccount.Savebal - amount, DateTime.Now.ToString(CultureInfo.GetCultureInfo("en-US")))) == 1 &&
                    await _repository.UpdateBankAccountCheckBal(bankAccount.Checkbal + amount, accountid) == 1 &&
                    await _repository.CreateTransaction(new Transact(accountid, "deposit", amount, "checking",
                        bankAccount.Checkbal + amount, DateTime.Now.ToString(CultureInfo.GetCultureInfo("en-US")))) == 1
                ) {
                    return 1;
                }
                
            }
            // Attempts to transfer money from checking to savings
            if (transferTo == "savings" && bankAccount.Checkbal >= amount) {
                if (
                    await _repository.UpdateBankAccountCheckBal(bankAccount.Checkbal - amount, accountid) == 1 &&
                    await _repository.CreateTransaction(new Transact(accountid, "withdraw", amount, "checking",
                        bankAccount.Checkbal - amount, DateTime.Now.ToString(CultureInfo.GetCultureInfo("en-US")))) == 1 &&
                    await _repository.UpdateBankAccountSaveBal(bankAccount.Savebal + amount, accountid) == 1 &&
                    await _repository.CreateTransaction(new Transact(accountid, "deposit", amount, "savings",
                        bankAccount.Savebal + amount, DateTime.Now.ToString(CultureInfo.GetCultureInfo("en-US")))) == 1
                ) {
                    return 1;
                }
            }
        }

        return 0;
    }

    public async Task<int> DeleteBankAccount(int accountid) {
        return await _repository.DeleteBankAccountByAccountid(accountid);
    }
}

