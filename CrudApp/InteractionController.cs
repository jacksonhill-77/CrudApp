using CrudApp.models;
using CrudApp.services;
using CrudApp.utils;

namespace CrudApp;

/// <summary>
/// The role of the InteractionController is to implement all console read and write operations
/// </summary>
/// <param name="bookService">Class that implements IBookService</param>
public class InteractionController(IBookService bookService, IUserInputService userInputService)
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
            var userInput = _userInputService.GetUserInput("\nPlease enter a number from the options above:\n").userInput;
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

    void DisplayBooks()
    {
        var fetchResult = _bookService.FetchBooks();

        if (fetchResult.isSuccess == false)
        {
            Console.WriteLine(fetchResult.message);
            return;
        }

        PrintBooks(fetchResult.books);
    }

    void AddBook()
    {
        // May need to change to give multiple book functionality
        var book = GetUserInputAsBook().book;
        _bookService.AddBook(book);
    }

    void RemoveBook()
    {
        var getBookTitleResult = _userInputService.GetUserInput("Please enter the title of the book you wish to remove:");

        if (getBookTitleResult.userInput == null)
        {
            Console.WriteLine(getBookTitleResult.message);
            return;
        }

        _bookService.RemoveBook(getBookTitleResult.userInput);
    }

    void UpdateBook()
    {
        var titleOfBookToUpdate = _userInputService.GetUserInput("Please enter the title of the book you wish to update").userInput;
        Console.WriteLine("... and press any key to start entering the new information that you would like the book to have");
        var book = GetUserInputAsBook().book;
        _bookService.UpdateBook(titleOfBookToUpdate, book);
    }

    void PrintOptions()
    {
        Console.WriteLine("1. Display current books in the library");
        Console.WriteLine("2. Add a book");
        Console.WriteLine("3. Remove a book");
        Console.WriteLine("4. Update a book's contents");
        Console.WriteLine("5. Close application");
    }
    
    void PrintBooks(List<Book?>? books)
    {
        Console.WriteLine("\n");

        books
            .GroupBy(x => x.Author, (s, enumerable) => new { Author = s, Books = enumerable.ToList() })
            .Select(x => ConvertLineToReadableInfo(x.Author, x.Books))
            .ToList()
            .ForEach(Console.WriteLine);
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

    static int GetIndexOfBookToModify(string modificationType)
    {
        // TODO: Re-think
        Console.WriteLine($"Please select the number of a book to {modificationType}:");
        //the below line should be in InteractionController
        //PrintLines(FileUtility.ReadLinesFromFile(filePath), filePath);
        return int.Parse(Console.ReadLine()) - 1;
    }

    static string ChangeBookProperties(string book)
    {
        //TODO: Re - think
        var isRunning = true;
        while (isRunning)
        {
            Console.WriteLine("\nPlease select the part of the book you wish to update by selecting 1-3: ");
            //Console.WriteLine(InteractionController.ConvertLineToPropertiesList(book));
            var chosenProperty = int.Parse(Console.ReadLine()) - 1;
            //book = _bookService.ModifyBook(book, chosenProperty);
            Console.WriteLine("\nDo you wish to continue editing? y/ n");
            var continueEditing = Console.ReadLine();
            if (continueEditing == "y")
            {
                continue;
            }
            else if (continueEditing == "n")
            {
                break;
            }
        }

        return book;
    }
    string ConvertLineToReadableInfo(string? author, List<Book> books)
    {
        return $"Author: {author}\n" + string.Join("\n", books.Select(ConvertLineToReadableInfo));
    }
    
    string ConvertLineToReadableInfo(Book book, int index)
    {
        return $"{index + 1}. Title: {book.Title}. Author: {book.Author}. Year published: {book.PublishYear}";
    }

    // from BookHelper
    public List<Book> GetUsersListOfBooks()
    {
        var isRunning = true;
        var books = new List<Book>();

        do
        {
            books.Add(GetUserInputAsBook());
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

    public (bool isSuccess, Book? book) GetUserInputAsBook()
    {
        // dummy pid
        var pid = 0;

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
            return (false, null);
        }

        var book = new Book();
        book.Id = pid;
        book.Title = title;
        book.Author = author;
        book.PublishYear = publishDate ?? -1;

        return (true, book);
    }

    
}


