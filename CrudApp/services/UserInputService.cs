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
            var result = GetUserInput(prompt);

            if (result.userInput == null)
            {
                return ("No input found from user", null);   
            }

            if (!int.TryParse(result.userInput, out var value))
            {
                return ("User input was not a valid number", null);
            }

            return (null, value);
        }

        public (string? message, string? userInput) GetUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            var userInput = Console.ReadLine();

            if (userInput == null)
            {
                return ("Could not get user input, please try again", null);
            }

            return (null, userInput);
        }
    }
}
