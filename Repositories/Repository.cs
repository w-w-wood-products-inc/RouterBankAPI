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
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return null;
        }
        
    }

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
        }
        catch {
            return null;
        }
        
    }

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
        }
        catch {
            return null;
        }
        
    }
    
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
        }
        catch {
            return null;
        }
        
    }

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
        }
        catch {
            return null;
        }
        
    }

    public async Task<List<BankAccount?>> GetAllBankAccounts() {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        try {
            IEnumerable<BankAccount?> result = await connection.QueryAsync<BankAccount>(
                "SELECT * FROM BANKACCOUNT"
            );
            
            return result.ToList();
        }
        catch {
            return null;
        }
        
    }

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
        }
        catch {
            return null;
        }
        
    }
    
    // Post Queries -----------------------
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
        }
        catch {
            return 0;
        }
        
    }

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
        }
        catch {
            return 0;
        }
    }

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
        }
        catch {
            return 0;
        }
    }
    
    // Put Queries -----------------------
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
        }
        catch {
            return 0;
        }
    }

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
        }
        catch {
            return 0;
        }
    }

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
        }
        catch {
            return 0;
        }
    }

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
        }
        catch {
            return 0;
        }
    }

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
        }
        catch {
            return 0;
        }
    }
    
    // Delete Queries -----------------------
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
        }
        catch {
            return 0;
        }
    }

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
        }
        catch {
            return 0;
        }
    }
    
}