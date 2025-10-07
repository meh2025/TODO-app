using System;

namespace GuessingGame
{
    static class Var
    {
        public static User? currentUser;
    }

    class User
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public List<string> Todos { get; set; } = new List<string>();
    }
}
