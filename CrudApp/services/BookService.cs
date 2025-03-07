using CrudApp.models;
using CrudApp.services.data;
using CrudApp.utils;
using Newtonsoft.Json;

namespace CrudApp.services;
public interface IBookService
{
    (bool isSuccess, string? message) AddBook(Book book);
    void AddBooks(List<Book> books);
    void RemoveBook(string titleOfBookToRemove);
    (bool isSuccess, string? message, List<Book>? books) FetchBooks();
    (bool isSuccess, string? message, Book? updatedBook) UpdateBook(string titleOfBookToUpdate, Book updatedBook);
}

public class BookService(IBookRepository bookRepository) : IBookService
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public void RemoveBook(Book book)
    {
        // TODO: Move to void RemoveBook(Book book)
        // var chosenBook = GetIndexOfBookToModify("remove");
        // var lines = _fileService.ReadLinesFromFile(filePath).ToList();
        // lines.RemoveAt(chosenBook);
        // _fileService.WriteLinesToFile(lines, filePath);
        // // the below line should be in InteractionController
        // Console.WriteLine("Book removed.");
        // DisplayBooks();
        throw new NotImplementedException();
    }

    //public (bool isSuccess, string? message) UpdateBook(string titleOfBookToUpdate, Book updatedBookProperties)
    //{
    //    var existingBook = _bookRepository.GetBookByTitle(titleOfBookToUpdate);

    //    if (existingBook == null)
    //    {
    //        return (false, "Can't update book - doesn't exist");
    //    }

    //    // possibly put into own method - bookRepository.UpdateBook(titleOfBookToUpdate, updatedBook)
    //    _bookRepository.RemoveBook(titleOfBookToUpdate);
    //    _bookRepository.AddBook(updatedBookProperties.Author, updatedBookProperties.Title, updatedBookProperties.PublishYear);

    //    // List<string> lines = _fileService.ReadLinesFromFile(filePath).ToList();
    //    // var book = lines[bookIndex];
    //    // var updatedBook = ChangeBookProperties(book);
    //    // lines[bookIndex] = updatedBook;
    //    //_fileService.WriteLinesToFile(lines, filePath);
    //    return (true, null);
    //}

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

    public void RemoveBook(string titleOfBookToRemove)
    {
        throw new NotImplementedException();
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

