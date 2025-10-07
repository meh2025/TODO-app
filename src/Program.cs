using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using GuessingGame;

namespace TODO
{
    class Program
    {
        public static List<User> users = new List<User>();
        static void Main()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Utils.OnExit); // if user use CTRL+C to exit, it will save data

            users = Utils.LoadUsers(); // load user data

            // User login or register
            Console.Write("Enter username: ");
            string username = Console.ReadLine()!;
            string password = Utils.ReadPassword("Enter password: ");
            Var.currentUser = users.Find(u => u.Username == username);

            if (Var.currentUser == null) // Register account
            {
                // Sign up
                Var.currentUser = new User { Username = username, Password = password, Todos = new List<string>() }; // Initialize with empty todo list
                users.Add(Var.currentUser); // add new user to user list
                Console.WriteLine("✅ Account created! Your todo list is empty.");
            }
            else
            {
                // Login
                if (Var.currentUser.Password != password)
                {
                    Console.WriteLine("❌ Wrong password!");
                    return;
                }
                Console.WriteLine($"👋 Welcome back, {username}! You have {Var.currentUser.Todos.Count} tasks.\n");
            }

            // Main menu
            while (true)
            {
                Utils.MainMenu();
                string? option = Console.ReadLine()?.Trim(); // Get user input from ">>> " prompt

                // Handle menu options
                switch (option)
                {
                    case "1": Utils.ViewTasks(); break;
                    case "2": Utils.AddTask(); break;
                    case "3": Utils.DeleteTask(); break;
                    case "4": Utils.ExitApp(); return;
                    default: Console.WriteLine("❌ Invalid option! Please choose between 1-4.\n"); continue;
                }
            }
        }
    }    
}
