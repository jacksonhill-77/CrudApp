using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudApp.services
{
    public interface IUserInputService
    {
        (string? message, string? userInput) GetUserInput(string? prompt);
        public (string? message, int? userInput) GetUserInputAsIntLoop(string? prompt);
        public (string? message, string? userInput) GetUserInputLoop(string? prompt);
        public (string? message, int? userInput) GetUserInputAsInt(string? prompt);
        public bool? CheckIfUserContinues(string prompt);
    }
    public class UserInputService : IUserInputService
    {

        public (string? message, int? userInput) GetUserInputAsIntLoop(string? prompt)
        {
            while (true)
            {
                var result = GetUserInputAsInt(prompt);
                if (result.userInput == null)
                {
                    Console.WriteLine(result.message);
                }
                else
                {
                    return (result);
                }

            }
        }
        public (string? message, string? userInput) GetUserInputLoop(string? prompt)
        {
            while (true)
            {
                var result = GetUserInput(prompt);
                if (result.userInput == null)
                {
                    Console.WriteLine(result.message);
                }
                else
                {
                    return (result);
                }

            }
        }

        public (string? message, int? userInput) GetUserInputAsInt(string? prompt)
        {
            var (message, userInput) = GetUserInput(prompt);

            if (userInput == null)
            {
                return (message, null);   
            }

            if (!int.TryParse(userInput, out var value))
            {
                return ("User input was not a valid number", null);
            }

            return (null, value);
        }

        public (string? message, string? userInput) GetUserInput(string? prompt)
        {
            Console.WriteLine(prompt);
            var userInput = Console.ReadLine();

            if (userInput == null)
            {
                return ("Could not get user input, please try again", null);
            }

            return (null, userInput);
        }

        public bool? CheckIfUserContinues(string prompt)
        {
            Console.WriteLine(prompt);
            var userResponse = Console.ReadLine();
            if (userResponse == "y")
            {
                return true;
            }
            else if (userResponse == "n")
            {
                return false;
            }
            else
            {
                Console.WriteLine("Invalid response, please try again.");
                return null;
            };
        }
    }
}
