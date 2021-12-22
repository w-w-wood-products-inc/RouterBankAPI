using demoWebAPI.models;
using FirstAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace demoWebAPI.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class UserAccountController : ControllerBase {

        private IUserAccountManager _userAccountManager;

        public UserAccountController(IUserAccountManager uAM) {
            _userAccountManager = uAM;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserAccount>> Get() {
            return Ok(_userAccountManager.GetUsers());
        }

        [HttpGet("{userid}")]
        public ActionResult<UserAccount> GetUserAccount(int userId) {
            UserAccount? foundUser = _userAccountManager.FindUserById(userId);
            if (foundUser != null) {
                return Ok(foundUser);
            }
            return NotFound("User account not found");
        }

        [HttpPost]
        public ActionResult<string> PostUserAccount(UserAccount user) {
            if (_userAccountManager.AddUser(user)) {
                return Ok($"An Account has been created for {user.name}");
            }
            return BadRequest("An error occured while creating the user");
        }

        [HttpPut]
        public ActionResult<string> PutUserAccount(UserAccount user) {
            if (_userAccountManager.ModifyUser(user)) {
                return Ok($"Account {user.username} has been modified");
            }
            return NotFound($"User {user.name} with id {user.userId} was not found");
        }

        [HttpDelete("{userid}")]
        public ActionResult<string> DeleteUserAccount(int userId) {
            if (_userAccountManager.DeleteUser(userId)) {
                return Ok("User account has been deleted");
            }
            return NotFound("User account was not found or does not exist");
        }
    }
}

