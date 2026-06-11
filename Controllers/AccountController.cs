using GrocerySysAppService;
using GrocerySysModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrocerySysAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountAppService _accountService;

        public AccountController()
        {
            _accountService = new AccountAppService();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Accounts>> GetAllAccounts()
        {
            var accounts = _accountService.GetAccounts();
            return Ok(accounts);
        }

        [HttpGet("{username}", Name = "GetAccountRoute")]
        public ActionResult<Accounts> GetAccountByUsername(string username)
        {
            var exists = _accountService.GetUsername(username);

            if (!exists)
            {
                return NotFound(new { Message = $"Account with username '{username}' not found." });
            }

            return Ok(new { Username = username });
        }

        [HttpPost("register")]
        public IActionResult RegisterAccount([FromBody] Accounts newAccount)
        {
            if (newAccount == null || string.IsNullOrEmpty(newAccount.Username))
            {
                return BadRequest("Valid account registration data is required.");
            }

         
            if (newAccount.AccountID == Guid.Empty)
            {
                newAccount.AccountID = Guid.NewGuid();
            }

            bool registrationSuccess = _accountService.Register(newAccount);

            if (!registrationSuccess)
            {
                return Conflict(new { Message = $"Username '{newAccount.Username}' is already taken." });
            }

            return CreatedAtRoute(
                "GetAccountRoute",
                new { username = newAccount.Username },
                newAccount);
        }

        [HttpPatch("{username}")]
        public IActionResult UpdateAccount(string username, [FromBody] Models.AccountUpdateViewModel updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Update data is required.");
            }

            if (!_accountService.GetUsername(username))
            {
                return NotFound(new { Message = $"Account with username '{username}' not found." });
            }

            
            if (!string.IsNullOrEmpty(updateDto.NewUsername))
            {
                bool dynamicUserUpdate = _accountService.UpdateUsername(username, updateDto.NewUsername);
                if (!dynamicUserUpdate) return BadRequest("Could not update username.");
                username = updateDto.NewUsername; 
            }

            if (!string.IsNullOrEmpty(updateDto.NewPassword))
            {
                bool dynamicPassUpdate = _accountService.UpdatePassword(username, updateDto.NewPassword);
                if (!dynamicPassUpdate) return BadRequest("Could not update password.");
            }

            return NoContent();
        }

        [HttpDelete("{username}")]
        public IActionResult DeleteEmployee(string username)
        {
            bool successfullyDeleted = _accountService.RemoveEmployee(username);

            if (!successfullyDeleted)
            {
                return NotFound(new { Message = $"Cannot delete. Employee with username '{username}' does not exist or cannot be removed." });
            }

            return NoContent();
        }

        [HttpPost("authenticate")]
        public ActionResult<Accounts> Authenticate([FromBody] Models.LoginRequest loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Username and Password are required.");
            }

            var account = _accountService.Authenticate(loginDto.Username, loginDto.Password);

            if (account == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            return Ok(account);
        }

        [HttpGet("logs")]
        public ActionResult<IEnumerable<AccessLogs>> GetAccessLogs()
        {
            var logs = _accountService.GetAccessLogs();
            return Ok(logs);
        }

        [HttpPost("logs")]
        public IActionResult CreateAccessLog([FromBody] AccessLogs accessLog)
        {
            if (accessLog == null)
            {
                return BadRequest("Log data cannot be empty.");
            }

            _accountService.AddAccessLog(accessLog);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}