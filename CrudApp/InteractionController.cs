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
            var userInput = Console.ReadLine();
            switch (userInput)
            {
                case "1":
                    DisplayBooks();
                    break;
                case "2":
                    AddBooks();
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
            PrintOptions();
        } while (_isRunning == true);
    }

    void DisplayBooks()
    {
        //PrintBooks(_bookService.FetchBooks());
        throw new NotImplementedException();
    }

    void AddBooks()
    {
        throw new NotImplementedException();
    }

    void RemoveBook()
    {
        throw new NotImplementedException();
    }

    void UpdateBook()
    {
        var bookIndex = GetIndexOfBookToModify("update");
        //_bookService.UpdateBook();
        throw new NotImplementedException();
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
    
    void PrintBooks(List<Book> books)
    {
        Console.WriteLine("\n");

        books
            .GroupBy(x => x.Author, (s, enumerable) => new { Author = s, Books = enumerable.ToList() })
            .Select(x => ConvertLineToReadableInfo(x.Author, x.Books))
            .ToList()
            .ForEach(Console.WriteLine);
    }

    // from BookHelper
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



    // from BookHelper
    static int GetIndexOfBookToModify(string modificationType)
    {
        // TODO: Re-think
        //the below line should be in InteractionController
        Console.WriteLine($"Please select the number of a book to {modificationType}:");
        //the below line should be in InteractionController
        //PrintLines(FileUtility.ReadLinesFromFile(filePath), filePath);
        return int.Parse(Console.ReadLine()) - 1;
    }

    // from BookHelper
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

    // from BookHelper
    public Book GetUserInputAsBook()
    {
        // dummy pid
        var pid = 0;

        Console.WriteLine("\nPlease enter the book title: ");
        var title = Console.ReadLine();

        Console.WriteLine("\nPlease enter the author name: ");
        var author = Console.ReadLine();

        Console.WriteLine("\nPlease enter the publish date: ");
        var publishDate = int.Parse(Console.ReadLine());

        var book = new Book();
        book.Id = pid;
        book.Title = title;
        book.Author = author;
        book.PublishYear = publishDate;

        return book;
    }
}


