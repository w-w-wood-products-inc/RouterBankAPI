using System.Security.Cryptography.X509Certificates;
using demoWebAPI.models;
using FirstAPI.Services;
using FirstAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace demoWebAPI.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class RouterBankController : ControllerBase {

        private IUserAccountManager _userAccountManager;
        private IBankAccountManager _bankAccountManager;

        public RouterBankController(IUserAccountManager uAM, IBankAccountManager bAM) {
            _userAccountManager = uAM;
            _bankAccountManager = bAM;
        }
        
        // Retrieves the user account class for a user
        [HttpGet]
        [Route("/{accountid}/useraccount")]
        [Authorize]
        public async Task<ActionResult<UserAccount>> GetUserAccount([FromRoute] int accountid) {
            // Checks the token to make sure user has access to the requested bank account
            if (!_bankAccountManager.AuthenticateBankAccount(HttpContext.User, accountid)) {
                return Unauthorized();
            }

            // Checks to see if the requested user account exists
            UserAccount? result = await _userAccountManager.GetUserAccount(accountid);
            if (result == null) {
                return BadRequest();
            }

            return Ok(result);
        }
        
        // Retrieves the bank account class for a user
        [HttpGet]
        [Route("/{accountid}/bankaccount")]
        [Authorize]
        public async Task<ActionResult<BankAccount>> GetBankAccount([FromRoute] int accountid) {
            // Checks the token to make sure user has access to the requested bank account
            if (!_bankAccountManager.AuthenticateBankAccount(HttpContext.User, accountid)) {
                return Unauthorized();
            }

            // Checks to see if the requested bank account exists
            BankAccount? result = await _bankAccountManager.GetBankAccount(accountid);
            if (result == null) {
                return BadRequest();
            }

            return Ok(result);
        }
        
        // retrevies an array of all bank account ids that the user owns
        [HttpGet]
        [Route("/bankaccountsowned")]
        [Authorize]
        public ActionResult<List<int>> GetBankAccountIds() { 
            List<int>? results = _bankAccountManager.GetBankAccountIds(HttpContext.User);

            if (results == null) {
                return BadRequest();
            }

            return Ok(results);
        }
        
        // Retrieves the transaction history of a user's bank account
        [HttpGet]
        [Route("/{accountid}/transactionhistory")]
        [Authorize]
        public async Task<ActionResult<List<Transact>>> GetTransactionHistory([FromRoute] int accountid) {
            // Checks the token to make sure user has access to the requested bank account
            if (!_bankAccountManager.AuthenticateBankAccount(HttpContext.User, accountid)) {
                return Unauthorized();
            }

            List<Transact>? result = await _bankAccountManager.GetTransactionHistory(accountid);
            if (result == null) {
                return BadRequest();
            }

            return Ok(result);
        }
        
        // Authenticates the user's login and returns a JWT token if successful
        [HttpPost]
        [Route("/login")]
        public async Task<ActionResult<string>> Login([FromHeader] string username, [FromHeader] string password) {
            string? result = await _userAccountManager.Login(username, password);
            if (result == null) {
                return Unauthorized("Invalid username or password");
            }

            return Ok(result);
        }
        
        // Creates a new user account
        [HttpPost]
        [Route("/createuseraccount")]
        public async Task<ActionResult<string>> CreateUserAccount([FromBody] UserAccount userAccount) {
            string userResult = await _userAccountManager.CreateUserAccount(userAccount);

            if (userResult != null) {
                return Ok(userResult);
            }

            return BadRequest("Invalid user account data");
        }
        
        // Creates a new bank account
        [HttpPost]
        [Route("/createbankaccount")]
        [Authorize]
        public async Task<ActionResult<string>> CreateBankAccount([FromBody] BankAccount bankAccount) {
            string bankResult = await _bankAccountManager.CreateBankAccount(bankAccount);

            if (bankResult != "") {
                return Ok(bankResult);
            }

            return BadRequest("Invalid bank account data");
        }
        
        // Deposits routez into a user's savings bank account
        [HttpPut]
        [Route("/{accountid}/depositsavings")]
        [Authorize]
        public async Task<ActionResult<string>> DepositSavings([FromHeader] double amount, [FromRoute] int accountid) {
            // Checks the token to make sure user has access to the requested bank account
            if (!_bankAccountManager.AuthenticateBankAccount(HttpContext.User, accountid)) {
                return Unauthorized();
            }

            if (await _bankAccountManager.DepositSavings(amount, accountid) == 1) {
                return Ok($"{amount} has been added to your savings account");
            }

            return BadRequest();
        }
        
        // Deposits routez into a user's checking bank account
        [HttpPut]
        [Route("/{accountid}/withdrawsavings")]
        [Authorize]
        public async Task<ActionResult<string>> WithdrawSavings([FromHeader] double amount, [FromRoute] int accountid) {
            // Checks the token to make sure user has access to the requested bank account
            if (!_bankAccountManager.AuthenticateBankAccount(HttpContext.User, accountid)) {
                return Unauthorized();
            }

            if (await _bankAccountManager.WithdrawSavings(amount, accountid) == 1) {
                return Ok($"{amount} has been withdrawn from your savings account");
            }

            return BadRequest();
        }
        
        // Withdraws routez from a user's savings bank account
        [HttpPut]
        [Route("/{accountid}/depositchecking")]
        [Authorize]
        public async Task<ActionResult<string>> DepositChecking([FromHeader] double amount, [FromRoute] int accountid) {
            // Checks the token to make sure user has access to the requested bank account
            if (!_bankAccountManager.AuthenticateBankAccount(HttpContext.User, accountid)) {
                return Unauthorized();
            }

            if (await _bankAccountManager.DepositChecking(amount, accountid) == 1) {
                return Ok($"{amount} has been added to your checking account");
            }

            return BadRequest();
        }
        
        // Withdraws routez from a user's checking bank account
        [HttpPut]
        [Route("/{accountid}/withdrawchecking")]
        [Authorize]
        public async Task<ActionResult<string>> WithdrawChecking([FromHeader] double amount, [FromRoute] int accountid) {
            // Checks the token to make sure user has access to the requested bank account
            if (!_bankAccountManager.AuthenticateBankAccount(HttpContext.User, accountid)) {
                return Unauthorized();
            }

            if (await _bankAccountManager.WithdrawChecking(amount, accountid) == 1) {
                return Ok($"{amount} has been withdrawn from your checking account");
            }

            return BadRequest();
        }
        
        // Updates the user's mpr and mpr_enable status in their bank account class
        [HttpPut]
        [Route("/{accountid}/updatempr")]
        [Authorize]
        public async Task<ActionResult<string>> UpdateMpr([FromHeader] double amount, [FromHeader] bool enabled,
            [FromRoute] int accountid) {
            // Checks the token to make sure user has access to the requested bank account
            if (!_bankAccountManager.AuthenticateBankAccount(HttpContext.User, accountid)) {
                return Unauthorized();
            }

            if (await _bankAccountManager.UpdateMpr(amount, accountid) == 1 &&
                await _bankAccountManager.UpdateMprEnable(enabled, accountid) == 1) {
                return Ok("Your MPR settings have been updated");
            }

            return BadRequest();
        }
        
        // Transfers routez from either savings or checking in a user's bank account
        [HttpPut]
        [Route("/{accountid}/transfer")]
        [Authorize]
        public async Task<ActionResult<string>> Transfer([FromHeader] double amount, [FromHeader] string transferTo,
            [FromRoute] int accountid) {
            // Checks the token to make sure user has access to the requested bank account
            if (!_bankAccountManager.AuthenticateBankAccount(HttpContext.User, accountid)) {
                return Unauthorized();
            }

            if (transferTo == "checking" || transferTo == "savings") {
                if (await _bankAccountManager.Transfer(amount, transferTo, accountid) == 1) {
                    return Ok($"{amount} successfully transferred from savings to checking");
                }
            }

            return BadRequest();
        }
    }
}

