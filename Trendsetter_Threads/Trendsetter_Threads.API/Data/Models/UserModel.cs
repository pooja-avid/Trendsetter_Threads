namespace Trendsetter_Threads.API.Data.Models;
public class UserModel
{
    public int? Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime? createdAt { get; set; }
    public bool IsAdmin { get; set; }
    public string? Token { get; set; }
    public string? TokenExpiryTime { get; set; }
}
