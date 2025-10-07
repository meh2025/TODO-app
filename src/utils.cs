using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using System.Security.Cryptography;
using TODO;

namespace GuessingGame
{
    static class Utils
    {
        private static string dataDir = Path.Combine(Directory.GetCurrentDirectory(), ".UserData");
        private static string filePath = Path.Combine(dataDir, "users.json");

        // AES Key
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("1234567890123456");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("6543210987654321");

        // valid input between [min, max]
        public static int GetValidInput(string prompt, int min, int max)
        {
            int value;
            while (true)
            {
                Console.Write(prompt); // prompt user
                string? input = Console.ReadLine();
                if (input == null) continue;
                if (int.TryParse(input, out value) && value >= min && value <= max) // Check if input is valid
                    return value;

                Console.WriteLine($"❌ Invalid input! Enter a number between {min} and {max}.");
            }
        }

        // CTRL+C Exiting program
        public static void OnExit(object? sender, ConsoleCancelEventArgs e)
        {
            SaveUsers(Program.users); // Save user data before quit
            e.Cancel = true;
            Environment.Exit(0); // exit enviroment
        }

        // load User DATA
        public static List<User> LoadUsers()
        {
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            if (!File.Exists(filePath))
                return new List<User>();

            try
            {
                // Using AES key to decrypt data
                using Aes aes = Aes.Create();
                aes.Key = Key;
                aes.IV = IV;

                // Using var to automatically dispose resources
                using var fs = new FileStream(filePath, FileMode.Open);
                using var cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                string json = sr.ReadToEnd();

                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            catch
            {
                Console.WriteLine("⚠️ Failed to decrypt user data. File may be corrupted.");
                return new List<User>();
            }
        }

        // Save user DATA
        public static void SaveUsers(List<User> users)
        {
            if (!Directory.Exists(dataDir)) // checking if User Data exists
                Directory.CreateDirectory(dataDir);

            string json = JsonConvert.SerializeObject(users, Formatting.Indented); // create user data json

            using Aes aes = Aes.Create(); // create AES Key
            aes.Key = Key;
            aes.IV = IV;


            using var fs = new FileStream(filePath, FileMode.Create);
            using var cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            sw.Write(json);
        }

        // Read password without echoing
        public static string ReadPassword(string prompt)
        {
            Console.WriteLine(prompt);
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true); // true = not print

                if (key.Key == ConsoleKey.Backspace && password.Length > 0) // handle backspace
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar)) // handle normal character
                {
                    password += key.KeyChar;
                    Console.Write("");
                }
            }
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return password.Trim();
        }
        public static void ViewTasks()
        {
            if (Var.currentUser == null || Var.currentUser.Todos == null || Var.currentUser.Todos.Count == 0) // No tasks
            {
            Console.WriteLine("📭 Your todo list is empty!");
            Console.WriteLine("Press any key to return to Main Menu...");
            Console.ReadKey(true);
            // Clear the last line (the "Press any key..." line)
            Console.Clear();
            return;
            }

            Console.WriteLine("\n📋 Your Tasks:");
            for (int i = 0; i < Var.currentUser.Todos.Count; i++) // list all tasks
            {
            Console.WriteLine($"{i + 1}. {Var.currentUser.Todos[i]}"); // Display task number and description
            }
            Console.WriteLine("\nPress any key to return to Main Menu...");
            Console.ReadKey(true);
            Console.Clear();
        }

        public static void MainMenu()
        {
            Console.WriteLine("\nPERSONAL TODO LIST");
            Console.WriteLine("1. View Tasks");
            Console.WriteLine("2. Add Task");
            Console.WriteLine("3. Remove Task");
            Console.WriteLine("4. Exit");
            Console.Write(">>> ");
        }

        public static void AddTask()
        {
            Console.Write("Enter new task: ");
            string? task = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(task))
            {
                if (Var.currentUser != null && Var.currentUser.Todos != null)
                {
                    Var.currentUser.Todos.Add(task);
                    Console.WriteLine("✅ Task added!");
                    SaveUsers(Program.users); // Save after adding
                }
                else
                {
                    Console.WriteLine("❌ Unable to add task: User or task list is not initialized.");
                }
                Console.WriteLine("\nPress any key to return to Main Menu...");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
        public static void DeleteTask()
        {
            ViewTasks();
            if (Var.currentUser == null || Var.currentUser.Todos == null || Var.currentUser.Todos.Count == 0) return;
            int index = GetValidInput("Enter task number to delete: ", 1, Var.currentUser.Todos.Count);
            Var.currentUser.Todos.RemoveAt(index - 1);
            Console.WriteLine("🗑️ Task removed!");
            SaveUsers(Program.users); // Save after deletion
        }
        public static void ExitApp()
        {
            SaveUsers(Program.users);
            Console.WriteLine("Goodbye!");
        }
    }
}
