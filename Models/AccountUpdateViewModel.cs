namespace GrocerySysAPI.Models
{
    public class AccountUpdateViewModel
    {
        public string? NewUsername { get; set; }
        public string? NewPassword { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}