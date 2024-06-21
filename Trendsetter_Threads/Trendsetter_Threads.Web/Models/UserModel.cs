namespace Trendsetter_Threads.Web.Models;
public class UserModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsAdmin { get; set; }
    public string Token { get; set; }
    public string TokenExpiryTime { get; set; }
}
