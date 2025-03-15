using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using CrudApp.models;
using Newtonsoft.Json;

// when setting up database naming schemes, you can say all camelcase is converted to SQL conventions

namespace CrudApp.services.data
{
    public interface IBookRepository
    {
        // create a test for reading database
        // public unneccessary 
        // the classes based on this interface only return from database, rather than return and print. so readdatabase shouldn't be void 
        // the interface class should deal with the printing 
        List<Book> ReadDatabase();
        (Book? book, List<Book>? databaseBooks, string? message) GetBookByTitle(string titleOfBook);
        (bool isSuccess, string? message) AddBook(Book book);
        (bool isSuccess, string? message) UpdateBook(string titleOfBookToUpdate, Book updatedBook);
        (bool isSuccess, string? message) RemoveBook(string titleOfBookToRemove);
    }

    public class LibraryContext : DbContext
    {
        public DbSet<Book> simple_library { get; set; }

        public static string connectionString = "Server=localhost;User ID=sa;Password=9n8kZ81J0iuB;Initial Catalog=SIMPLE_LIBRARY;Integrated Security=false;TrustServerCertificate=True";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

    }

    public class FileDbConnection : IBookRepository
    {
        private IFileService _fileService;
        private readonly string _filePath;

        public FileDbConnection(IFileService fileService, string filePath)
        {
            _fileService = fileService;
            _filePath = filePath;
        }
        public List<Book> ReadDatabase()
        {
            var lines = _fileService.ReadLinesFromFile(_filePath);

            var listOfBookJSON = lines
                .Where(x => x != null)
                .ToList();

            return ConvertListOfJSONToBooks(listOfBookJSON);
        }

        public (bool isSuccess, string? message) AddBook(Book book)
        { 
            try
            {
                string bookJSON = ConvertBookToJSON(book);
                _fileService.WriteLineToFile(bookJSON, _filePath);
                return (true, null);
            } catch (Exception ex)
            {
                return (false, $"Adding book failed with message: {ex.Message}");
            }
        }

        public (bool isSuccess, string? message) UpdateBook(string titleOfbookToUpdate, Book updatedBook)
        {
            var databaseWithoutBookResult = ReturnDatabaseWithoutBook(titleOfbookToUpdate);

            if (databaseWithoutBookResult.databaseBooks == null)
            {
                return (false, databaseWithoutBookResult.message);
            }

            databaseWithoutBookResult.databaseBooks.Add(updatedBook);
            WriteBooksToDatabase(databaseWithoutBookResult.databaseBooks);

            return (true, null);

        }

        public (bool isSuccess, string? message) RemoveBook(string titleOfBookToUpdate)
        {
            var returnDatabaseResult = ReturnDatabaseWithoutBook(titleOfBookToUpdate);

            if (returnDatabaseResult.databaseBooks == null)
            {
                return (false, returnDatabaseResult.message);
            }

            WriteBooksToDatabase(returnDatabaseResult.databaseBooks);

            return (true, "Book removed succesfully");
        }

        public void WriteBooksToDatabase(List<Book> databaseBooks)
        {
            var writeableLines = ConvertListOfBooksToJSON(databaseBooks);
            _fileService.WriteLinesToFile(writeableLines, _filePath);
        }

        public (List<Book>? databaseBooks, string? message) ReturnDatabaseWithoutBook(string titleOfBook)
        {
            var (book, databaseBooks, message) = GetBookByTitle(titleOfBook);

            if (book == null)
            {
                return (null, message);
            }

            if (databaseBooks == null)
            {
                return (null, message);
            }
            
            databaseBooks.Remove(book);
            
            return (databaseBooks, null);
        }

        public (Book? book, List<Book>? databaseBooks, string? message) GetBookByTitle(string titleOfBook)
        {
            var databaseBooks = ReadDatabase();

            if (databaseBooks == null)
            {
                return (null, null, "Could not find books in database");
            }

            var book = databaseBooks.FirstOrDefault(book => book.Title == titleOfBook);

            if (book == null)
            {
                return (null, null, "Could not find book title");
            }

            return (book, databaseBooks, null);
        }

        private List<Book> ConvertListOfJSONToBooks(List<string> listOfBookJSON)
        {
            var listOfBooks = new List<Book>();

            listOfBookJSON.ForEach(book =>
            {
                var result = ConvertJSONToBook(book);
                if (result.book != null)
                {
                    listOfBooks.Add(result.book);
                }
            });

            return listOfBooks;
        }

        private List<string> ConvertListOfBooksToJSON(List<Book> listOfBooks)
        {
            var listOfBookJSON = new List<string>();
            foreach (var book in listOfBooks)
            {
                listOfBookJSON.Add(ConvertBookToJSON(book));
            }

            return listOfBookJSON;
        }

        public (string? message, Book? book) ConvertJSONToBook(string bookJSON)
        {
            var book = JsonConvert.DeserializeObject<Book>(bookJSON);
            if (book == null)
            {
                return ("Converting book to JSON returned null", book);
            }

            return (null, book);
        }

        public string ConvertBookToJSON(Book book)
        {
            return JsonConvert.SerializeObject(book);
        }

    }

    //public class DapperDbConnection : IBookRepository
    //{
    //    public static string connectionString = "Server=localhost;User ID=sa;Password=9n8kZ81J0iuB;Initial Catalog=SIMPLE_LIBRARY;Integrated Security=false;TrustServerCertificate=True";

    //    public List<Book> ReadDatabase(string filePath)
    //    {
    //        using (var connection = new SqlConnection(connectionString))
    //        {
    //            var sql = "SELECT * FROM dbo.simple_library";

    //            var books = connection.Query<Book>(sql).ToList();

    //            // should be in interaction controller
    //            foreach (Book book in books)
    //            {
    //                Console.WriteLine(book.ToString());
    //            }
    //        }

    //        return new List<Book>();
    //    }

    //    public Book? GetBookByTitle(string title)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public (bool isSuccess, string? message) AddBook(string bookJSON)
    //    {
    //        using (var connection = new SqlConnection(connectionString))
    //        {
    //            // get value from user

    //            var sql = "INSERT INTO dbo.simple_library";

    //            var books = connection.Query<Book>(sql).ToList();

    //            foreach (Book book in books)
    //            {
    //                Console.WriteLine(book.ToString());
    //            }
    //        }

    //        return (true, null);
    //    }

    //    public (bool isSuccess, Book updatedBook) UpdateBook(string bookToUpdate, Book updatedBook)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool UpdateBookInRepo(string bookToUpdate, Book updatedBook)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void RemoveBook(string titleOfBookToRemove)
    //    {
    //        throw new NotImplementedException();
    //    }

    //}
}
