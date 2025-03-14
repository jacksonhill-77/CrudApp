using CrudApp.models;
using CrudApp.services;
using CrudApp.utils;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;

namespace CrudApp;


public interface IInteractionController
{
    void StartInteraction();
    public (bool isSuccess, string? message) DisplayBooks();
    public (bool isSuccess, string? message) AddBook();
    public (bool isSuccess, string? message) RemoveBook();
    public (bool isSuccess, string? message) UpdateBook();
}

/// <summary>
/// The role of the InteractionController is to implement all console read and write operations
/// </summary>
/// <param name="bookService">Class that implements IBookService</param>
public class InteractionController(IBookService bookService, IUserInputService userInputService) : IInteractionController
{
    private readonly IBookService _bookService = bookService;
    private readonly IUserInputService _userInputService = userInputService;

    private bool _isRunning = false;

    public void StartInteraction()
    {
        _isRunning = true;
        do
        {
            PrintOptions();
            var userInput = _userInputService.GetUserInput("").userInput;
            switch (userInput)
            {
                case "1":
                    DisplayBooks();
                    break;
                case "2":
                    AddBook();
                    break;
                case "3":
                    RemoveBook();
                    break;
                case "4":
                    UpdateBook();
                    break;
                case "5":
                    _isRunning = false;
                    break;
                default:
                    Console.WriteLine("Please choose a number between 1 - 4");
                    break;

            }
        } while (_isRunning == true);
    }

    public (bool isSuccess, string? message) DisplayBooks()
    {
        var fetchResult = _bookService.FetchBooks();

        if (fetchResult.isSuccess == false)
        {
            Console.WriteLine(fetchResult.message);
            return (false, fetchResult.message);
        }

        Console.WriteLine("Books currently in library:\n");
        PrintBooks(fetchResult.books);

        return (true, null);
    }

    public (bool isSuccess, string? message) AddBook()
    {
        // May need to change to give multiple book functionality
        var bookResult = GetUserInputAsBook();

        if (bookResult.book == null)
        {
            Console.WriteLine(bookResult.message);
            return (false, bookResult.message);
        }

        _bookService.AddBook(bookResult.book);
        return (true, null);
    }

    public (bool isSuccess, string? message) RemoveBook()
    {
        var getBookTitleResult = _userInputService.GetUserInput("Please enter the title of the book you wish to remove:");

        if (getBookTitleResult.userInput == null)
        {
            Console.WriteLine(getBookTitleResult.message);
            return (false, getBookTitleResult.message);
        }

        _bookService.RemoveBook(getBookTitleResult.userInput);
        return (true, null);
    }

    public (bool isSuccess, string? message) UpdateBook()
    {
        var (getTitleMessage, titleOfBookToUpdate) = _userInputService.GetUserInput("Please enter the title of the book you wish to update");
        if (titleOfBookToUpdate == null)
        {
            Console.WriteLine(getTitleMessage);
            return (false, getTitleMessage);
        }

        Console.WriteLine("... and press any key to start entering the new information that you would like the book to have");
        var (isSuccess, getBookMessage, book) = GetUserInputAsBook();

        if (book == null)
        {
            Console.WriteLine(getBookMessage);
            return (false, getBookMessage);
        }

        _bookService.UpdateBook(titleOfBookToUpdate, book);

        return (true, null);
    }

    void PrintOptions()
    {
        Console.WriteLine("\nPlease enter a number from the options below:\n");
        Console.WriteLine("1. Display current books in the library");
        Console.WriteLine("2. Add a book");
        Console.WriteLine("3. Remove a book");
        Console.WriteLine("4. Update a book's contents");
        Console.WriteLine("5. Close application");
    }
    
    string? PrintBooks(List<Book>? books)
    {
        Console.WriteLine("\n");

        if (books == null)
        {
            return "Could not find books to print";  
        }

        books
            .Select(x => ConvertLineToReadableInfo(x))
            .ToList()
            .ForEach(Console.WriteLine);

        return null;
    }

    void PrintUpdatedBookProperties(string updatedBook)
    {
        Console.WriteLine("\nUpdated. New properties are as follows: ");
        Console.WriteLine(ConvertLineToPropertiesList(updatedBook));
    }

    public string ConvertLineToPropertiesList(string line)
    {
        string[] properties = line.Split(',');
        return $"1. Title: {properties[0]}\n2. Author: {properties[1]}\n3. Year published: {int.Parse(properties[2])}";
    }

    public void PrintLines(string[] lines, string filePath)
    {
        Console.WriteLine("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            //Console.WriteLine(ConvertLineToReadableInfo(lines[i], i));
        };
    }

    string ConvertLineToReadableInfo(string? author, List<Book> books)
    {
        return $"Author: {author}\n" + string.Join("\n", books.Select(ConvertLineToReadableInfo));
    }
    
    string ConvertLineToReadableInfo(Book book)
    {
        return $"Title: {book.Title}. Author: {book.Author}. Year published: {book.PublishYear}";
    }

    // from BookHelper
    public List<Book> GetUsersListOfBooks()
    {
        var isRunning = true;
        var books = new List<Book>();

        do
        {
            var userInput = GetUserInputAsBook();

            if (userInput.book == null)
            {
                Console.WriteLine("No user input could be found");
                continue;
            }
            books.Add(userInput.book);

            Console.WriteLine("\nDo you wish to add another book? Y/N");

            var userResponse = Console.ReadLine();
            if (userResponse == "y")
            {
                continue;
            }
            else if (userResponse == "n")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid response, please try again.");
            };

        } while (isRunning);

        return books;
    }

    public (bool isSuccess, string? message, Book? book) GetUserInputAsBook()
    {
        // dummy pid
        var pid = 0;

        var (result, message, title, author, publishDate) = GetTitleAuthorPublishDate();

        if (result == false)
        {
            return (false, message, null);
        }

        var book = new Book();
        book.Id = pid;
        // is this the correct use of "!"? I am testing in the previous method that the result isn't null
        book.Title = title!;
        book.Author = author;
        book.PublishYear = publishDate ?? -1;

        return (true, null, book);
    }

    private (bool isSuccess, string? message, string? title, string? author, int? publishDate) GetTitleAuthorPublishDate()
    {
        var title = _userInputService
            .GetUserInputLoop("\nPlease enter the book title:")
            .userInput;
        var author = _userInputService
            .GetUserInputLoop("\nPlease enter the author name: ")
            .userInput;
        var publishDate = _userInputService
            .GetUserInputAsIntLoop("\nPlease enter the publish date: ")
            .userInput;

        if (title == null || author == null || publishDate == null)
        {
            return (false, "Could not enter information", null, null, null);
        }

        return (true, "Retrieved book information successfully", title, author, publishDate);
    }



}


