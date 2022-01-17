using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using Dapper;
using demoWebAPI.models;
using FirstAPI.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DefaultNamespace; 

public class Repository : IRepository {
    private readonly IConfiguration _iconfiguration;
    private readonly string _connectionString = 
            "Server=5DZZFK2\\MSSQLSERVER01;Database=RouterBank;Trusted_Connection=True;";

    public Repository(IConfiguration iconfiguration) {
        this._iconfiguration = iconfiguration;
    }
    
    // Get Queries -----------------------
    
    /// <summary>
    /// Gets a users account by social security number
    /// </summary>
    /// <param name="snn">The users social security number</param>
    /// <returns>Possibly a user account matching the ssn</returns>
    public async Task<UserAccount?> GetUserAccountBySnn(string snn) {
        await using var connection = new SqlConnection(_connectionString);
        
        connection.Open();
        
        try {
            IEnumerable<UserAccount?> result = await connection.QueryAsync<UserAccount>(
                @"
                    SELECT 
                        Name, 
                        Username, 
                        Birthdate, 
                        Addr, 
                        Phone, 
                        Snn 
                    FROM 
                        USERACCOUNT 
                    WHERE 
                        Snn = @Snn
                ", 
                new {
                    Snn = snn
                }
            );
            return result.ToList()[0];
        } catch {
            return null;
        }
        
    }
    
    /// <summary>
    /// Gets a user account by username and password.
    /// </summary>
    /// <param name="user">The user's username</param>
    /// <param name="pass">The user's password</param>
    /// <returns>Possibly a user account linked to the login information</returns>
    public async Task<UserAccount?> GetUserAccountByLogin(string user, string pass) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            IEnumerable<UserAccount?> result =
                await connection.QueryAsync<UserAccount>(
                    @"
                        SELECT 
                            Name, 
                            Username, 
                            Birthdate, 
                            Addr, 
                            Phone, 
                            Snn 
                        FROM 
                             USERACCOUNT 
                        WHERE 
                             Username = @user AND 
                             Pass     = @pass
                    ", 
                    new {
                        user, pass
                    }
                );
            return result.ToList()[0];
        } catch (Exception e) {
            Console.WriteLine(e);
            return null;
        }
        
    }

    /// <summary>
    /// Get a user's account by its account id.
    /// </summary>
    /// <param name="accountid">The id of the account to get</param>
    /// <returns>Possibly a UserAccount associated with the id</returns>
    public async Task<UserAccount?> GetUserAccountByAccountid(int accountid) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            IEnumerable<UserAccount?> result = await connection.QueryAsync<UserAccount>(
                @"
                    SELECT 
                        Name, 
                        Username, 
                        Birthdate, 
                        Addr, 
                        Phone, 
                        Snn 
                    FROM 
                        USERACCOUNT AS U, 
                        BANKACCOUNT AS B 
                    WHERE 
                        B.Accountid = @Accountid AND 
                        B.Ussn      = U.Snn
                ", 
                new {
                    Accountid = accountid
                }
            );
            
            return result.ToList()[0];
        } catch {
            return null;
        }
        
    }

    /// <summary>
    /// Returns all of the user accounts.
    /// </summary>
    /// <returns>All of the user accounts</returns>
    public async Task<List<UserAccount?>> GetAllUserAccounts() {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            IEnumerable<UserAccount?> result = await connection.QueryAsync<UserAccount>(
                @"
                    SELECT 
                        Name, 
                        Username, 
                        Birthdate, 
                        Addr, 
                        Phone, 
                        Snn 
                    FROM 
                         USERACCOUNT
                 "
            );
            
            return result.ToList();
        } catch {
            return null;
        }
        
    }
    
    /// <summary>
    /// Gets a bank account using the account id
    /// </summary>
    /// <param name="accountid">The id of the bank account to get</param>
    /// <returns>Possibly the bank account associated with the specified account id</returns>
    public async Task<BankAccount?> GetBankAccountByAccountid(int accountid) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            IEnumerable<BankAccount?> result = await connection.QueryAsync<BankAccount>(
                @"
                    SELECT 
                        * 
                    FROM 
                         BANKACCOUNT 
                    WHERE 
                         Accountid = @Accountid
                ", 
                new {
                    Accountid = accountid
                }
            );
            return result.ToList()[0];
        } catch {
            return null;
        }
        
    }

    /// <summary>
    /// Gets all bank accounts associated with a particular social security
    /// number.
    /// </summary>
    /// <param name="snn">The social security number of the account holder</param>
    /// <returns>A list of bank accounts.</returns>
    public async Task<List<BankAccount?>> GetBankAccountsBySnn(string snn) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            IEnumerable<BankAccount?> result = await connection.QueryAsync<BankAccount>(
                @"
                    SELECT 
                        * 
                    FROM 
                        USERACCOUNT AS U, 
                        BANKACCOUNT AS B 
                    WHERE 
                        B.Ussn = U.Snn AND 
                        U.Snn  = @Snn
                ", 
                new {
                    Snn = snn
                }
            );
            return result.ToList();
        } catch {
            return null;
        }
        
    }

    /// <summary>
    /// Gets all of the bank accounts.
    /// </summary>
    /// <returns>
    /// All of the bank accounts.
    /// </returns>
    public async Task<List<BankAccount?>> GetAllBankAccounts() {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            IEnumerable<BankAccount?> result = await connection.QueryAsync<BankAccount>(
                "SELECT * FROM BANKACCOUNT"
            );
            
            return result.ToList();
        } catch {
            return null;
        }
        
    }

    /// <summary>
    /// Gets the transaction history by account id.
    /// </summary>
    /// <param name="accountid">
    /// The id of the account to get the transaction history for
    /// </param>
    /// <returns>A list of transactions</returns>
    public async Task<List<Transact?>> GetTransactionHistoryByAccountid(int accountid) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            IEnumerable<dynamic> result = await connection.QueryAsync(
                @"
                    SELECT 
                        T.* 
                    FROM
                        BANKACCOUNT AS B, 
                        TRANSACT    AS T 
                    WHERE 
                        B.Accountid = T.Acntid AND 
                        B.Accountid = @Accountid
                ", 
                new {
                    Accountid = accountid
                }
            );
            
            List<Transact> transactions = new List<Transact>();
            
            foreach (var v in result) {
                DateTime convert = v.TDate;
                transactions.Add(
                    new Transact(
                        (int)v.Acntid, 
                        (string)v.Act, 
                        (double)v.Amount, 
                        (string)v.Account, 
                        (double)v.Newbal, 
                        convert.ToString(
                            CultureInfo.GetCultureInfo("en-US")
                        )
                    )
                );
            }

            return transactions;
        } catch {
            return null;
        }
        
    }
    
    // Post Queries -----------------------
    
    /// <summary>
    /// Creates a new user account
    /// </summary>
    /// <param name="userAccount">The user account to create</param>
    /// <returns>1 if successful and 0 if there was an error</returns>
    public async Task<int> CreateUserAccount(UserAccount userAccount) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    INSERT INTO USERACCOUNT VALUES (
                        @Name, 
                        @Username, 
                        @Pass, 
                        @Birthdate, 
                        @Addr, 
                        @Phone, 
                        @Snn
                    )
                ", 
                userAccount
            );
            
            return 1;
        } catch {
            return 0;
        }
        
    }

    /// <summary>
    /// Creates a new bank account.
    /// </summary>
    /// <param name="bankAccount">
    /// The details used to create the new bank account.
    /// </param>
    /// <returns>
    /// 1 if success, 0 otherwise
    /// </returns>
    public async Task<int> CreateBankAccount(BankAccount bankAccount) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    INSERT INTO BANKACCOUNT VALUES (
                        @Ussn, 
                        @Accountid, 
                        @Checkbal, 
                        @Savebal, 
                        @Mpr, 
                        @Mpr_enable
                    )
                ", 
                bankAccount
            );
            
            return 1;
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Creates a new transaction
    /// </summary>
    /// <param name="transact">
    /// The transaction details used to create the new Transaction instance.</param>
    /// <returns>
    /// 1 if successful, 0 if failure
    /// </returns>
    public async Task<int> CreateTransaction(Transact transact) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    INSERT INTO TRANSACT VALUES (
                        @Acntid, 
                        @Act, 
                        @Amount, 
                        @Account, 
                        @Newbal, 
                        @TDate
                    )
                ", 
                transact
            );
            
            return 1;
        } catch {
            return 0;
        }
    }
    
    // Put Queries -----------------------
    
    /// <summary>
    /// Updates a user account with new information.
    /// </summary>
    /// <param name="userAccount">
    /// The useraccount
    /// </param>
    /// <returns>1 if successful, 0 if failure.</returns>
    public async Task<int> UpdateUserAccount(UserAccount userAccount) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    UPDATE 
                        USERACCOUNT 
                    SET 
                        Name      = @Name, 
                        Username  = @Username, 
                        Pass      = @Pass, 
                        Birthdate = @Birthdate, 
                        Addr      = @Addr, 
                        Phone     = @Phone 
                    WHERE 
                        Snn = @Snn
                ", 
                userAccount
            );
            
            return 1;
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Updates a bank account's balance
    /// </summary>
    /// <param name="checkBal">The correct balance</param>
    /// <param name="accountid">The id of the account to update</param>
    /// <returns>1 if successful and 0 if a failure</returns>
    public async Task<int> UpdateBankAccountCheckBal(double checkBal, int accountid) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    UPDATE 
                        BANKACCOUNT 
                    SET 
                        Checkbal = @checkBal 
                    WHERE 
                        Accountid = @accountid
                ", 
                new {
                    checkBal, 
                    accountid
                }
            );
            
            return 1;
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Updates the balanace of a savings account.
    /// </summary>
    /// <param name="saveBal">The correct balance</param>
    /// <param name="accountid">The id of the account to update</param>
    /// <returns>1 if success, 0 if failure</returns>
    public async Task<int> UpdateBankAccountSaveBal(double saveBal, int accountid) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    UPDATE 
                        BANKACCOUNT 
                    SET 
                        Savebal = @savebal 
                    WHERE 
                        Accountid = @accountid
                ", 
                new {
                    saveBal, 
                    accountid
                }
            );
            
            return 1;
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Updates the Mpr for the bank account.
    /// </summary>
    /// <param name="mpr">The Mpr</param>
    /// <param name="accountid">The id of the account to lookup</param>
    /// <returns>1 if success, 0 if failure</returns>
    public async Task<int> UpdateBankAccountMpr(double mpr, int accountid) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    UPDATE 
                        BANKACCOUNT 
                    SET 
                        Mpr = @mpr 
                    WHERE 
                        Accountid = @accountid
                ", 
                new {
                    mpr, 
                    accountid
                }
            );
            
            return 1;
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// ?
    /// </summary>
    /// <param name="mprEnable">Whether or not to enable Mpr</param>
    /// <param name="accountid">The id of the account to update</param>
    /// <returns></returns>
    public async Task<int> UpdateBankAccountMprEnable(bool mprEnable, int accountid) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    UPDATE 
                        BANKACCOUNT 
                    SET 
                        Mpr_enable = @mprEnable 
                    WHERE 
                        Accountid = @accountid
                ", 
                new {
                    mprEnable, 
                    accountid
                }
            );
            
            return 1;
        } catch {
            return 0;
        }
    }
    
    // Delete Queries -----------------------
    
    /// <summary>
    /// Deletes a user account by social security number
    /// </summary>
    /// <param name="snn">The ssn of the account to delete</param>
    /// <returns>1 on success and 0 on failure</returns>
    public async Task<int> DeleteUserAccountBySnn(string snn) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    DELETE FROM 
                        USERACCOUNT 
                    WHERE 
                        Snn = @Snn
                ", 
                new {
                    Snn = snn
                }
            );
            
            return 1;
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Deletes a bank account by id
    /// </summary>
    /// <param name="accountid">The id of the bank account to delete.</param>
    /// <returns>1 on success and 0 on failure</returns>
    public async Task<int> DeleteBankAccountByAccountid(int accountid) {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        try {
            await connection.ExecuteAsync(
                @"
                    DELETE FROM 
                        BANKACCOUNT 
                    WHERE 
                        Accountid = @accountid
                ", 
                new {
                    Accountid = accountid
                }
            );
            
            return 1;
        } catch {
            return 0;
        }
    }
    
}