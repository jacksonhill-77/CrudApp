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
    }
    public class UserInputService : IUserInputService
    {
        public (string? message, string? userInput) GetUserInput(string? prompt)
        {
            if (prompt != null)
            {
                Console.WriteLine(prompt);
            }

            var userInput = Console.ReadLine();

            if (userInput == null)
            {
                return ("Could not get user input", null);
            }

            return (null, userInput);
        }
    }
}
