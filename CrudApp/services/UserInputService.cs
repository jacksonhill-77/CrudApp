using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudApp.services
{
    public interface IUserInputService
    {
        (bool isSuccess, string? message, string? userInput) GetUserInput(string prompt);
    }
    public class UserInputService : IUserInputService
    {
        public (bool isSuccess, string? message, string? userInput) GetUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            var userInput = Console.ReadLine();

            if (userInput == null)
            {
                return (false, "Could not get user input", null);
            }

            return (true, null, userInput);
        }
    }
}
