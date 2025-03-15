using CrudApp.models;
using CrudApp.services.data;
using CrudApp.utils;
using Newtonsoft.Json;

namespace CrudApp.services;
public interface IBookService
{
    (bool isSuccess, string? message) AddBook(Book book);
    void AddBooks(List<Book> books);
    (bool isSuccess, string? message) RemoveBook(string titleOfBookToRemove);
    (bool isSuccess, string? message, List<Book>? books) FetchBooks();
    (bool isSuccess, string? message, Book? updatedBook) UpdateBook(string titleOfBookToUpdate, Book updatedBook);
}

public class BookService(IBookRepository bookRepository) : IBookService
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public (bool isSuccess, string message) AddBook(Book newBook)
    {
        var existingBook = _bookRepository.GetBookByTitle(newBook.Title);
        
        if(existingBook.book != null)
        {
            return (false, "Already exists");
        }
        
        _bookRepository.AddBook(newBook);
        
        return (true, null)!;
    }

    public void AddBooks(List<Book> newBooks)
    {
        // var books = _fileService.ReadLinesFromFile(filePath);
        //
        // books.AddRange(newBooks.Select(JsonConvert.SerializeObject));
        //
        // // Should be in BookRepository
        // _fileService.WriteLinesToFile(books, filePath);
        throw new NotImplementedException();
    }

    public (bool isSuccess, string? message) RemoveBook(string titleOfBookToRemove)
    {
        var booksToFetch = _bookRepository.ReadDatabase();

        if (booksToFetch == null)
        {
            return (false, "No books found in library");
        }

        var (isSuccess, message) = _bookRepository.RemoveBook(titleOfBookToRemove);

        if (!isSuccess)
        {
            return (false, message);
        }

        return (true, message);
    }

    public (bool isSuccess, string? message, List<Book>? books) FetchBooks()
    {
        var booksToFetch = _bookRepository.ReadDatabase();

        if(booksToFetch == null)
        {
            return (false, "No books found in library", null);
        }

        return (true, null, booksToFetch);
    }

    public (bool isSuccess, string? message, Book? updatedBook) UpdateBook(string titleOfBookToUpdate, Book updatedBook)
    {
        var bookToUpdate = _bookRepository.UpdateBook(titleOfBookToUpdate, updatedBook);

        if (bookToUpdate.isSuccess == false)
        {
            return (false, "Book doesn't exist in library", null);
        }

        return (true, null, updatedBook);
    }
}

