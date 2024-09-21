namespace BlazorAppAuth.Client.Models;

public static class TempMaster
{
    public static List<Users> GetUsers()
    {
        return new List<Users> { new Users(user_name: "aa", password: "as"), new Users(user_name: "bb", password: "bs"), };
    }
}

public record Users(string user_name, string password);
