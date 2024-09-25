namespace BlazorAppAuth.Client.Models;

public static class TempMaster
{
    public static List<User> GetUsers()
    {
        return new List<User> { new User(user_name: "aa", password: "as"), new User(user_name: "bb", password: "bs"), };
    }
}

public class User(string user_name, string? password = null)
{
    public string user_name { get; set; }
    public string? password { get; set; }
}
//public record Users(string user_name, string password, string email, bool is_authenticated, IEnumerable<string> roles);
public class UserInfo
{
    public required string user_id { get; set; }
    public required string user_name { get; set; }

}