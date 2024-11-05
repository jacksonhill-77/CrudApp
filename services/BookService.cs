﻿using CrudApp.models;
using CrudApp.utils;
using Newtonsoft.Json;

namespace CrudApp.services;
public interface IBookService
{
    void AddBooks(List<Book> books);
    void RemoveBook(Book book);
    List<Book> FetchBooks();
    Book UpdateBook(Book book);
}

public class BookService(IFileService fileService) : IBookService
{
    private readonly IFileService _fileService = fileService;

    private string filePath = FilePathsUtility.filePath;

    public void RemoveBook(Book book)
    {
        // TODO: Move to void RemoveBook(Book book)
        var chosenBook = GetIndexOfBookToModify("remove");
        var lines = _fileService.ReadLinesFromFile(filePath).ToList();
        lines.RemoveAt(chosenBook);
        _fileService.WriteLinesToFile(lines, filePath);
        // the below line should be in InteractionController
        Console.WriteLine("Book removed.");
        DisplayBooks();
    }

    public void UpdateBook(Book book)
    {
        // TODO: Move to Book UpdateBook(Book book)

        var bookIndex = GetIndexOfBookToModify("update");
        List<string> lines = _fileService.ReadLinesFromFile(filePath).ToList();
        var book = lines[bookIndex];
        var updatedBook = ChangeBookProperties(book);
        lines[bookIndex] = updatedBook;
        _fileService.WriteLinesToFile(lines, filePath);

    }

    public void AddBooks(List<Book> newBooks)
    {
        var books = _fileService.ReadLinesFromFile(filePath);
        
        books.AddRange(newBooks.Select(JsonConvert.SerializeObject));

        // Should be in BookRepository
        _fileService.WriteLinesToFile(books, filePath);
    }

    public List<Book> FetchBooks()
    {
        throw new NotImplementedException();
    }

    // from BookHelper
    static string ModifyBook(string book, int propertyIndex)
    {
        var updatedBook = ChangeSinglePropertyOfBook(book, propertyIndex);
        //the below line should be in InteractionController
        _interactionController.PrintUpdatedBookProperties(updatedBook);
        return updatedBook;
    }

    // from BookHelper
    static string ChangeSinglePropertyOfBook(string book, int propertyIndex)
    {
        var properties = book.Split(',');
        //the below line should be in InteractionController
        Console.WriteLine("\nPlease enter what you would like to update to: ");
        var newProperty = Console.ReadLine();
        properties[propertyIndex] = newProperty;
        var updatedBook = String.Join(",", properties);
        return updatedBook;
    }

    // from BookHelper
    public static List<String> ConvertBookListToJSON(List<Book> books)
    {
        List<String> output = new List<String>();
        foreach (var book in books)
        {
            output.Add(JsonConvert.SerializeObject(book));
        }
        return output;
    }

    public void ConvertLineToPropertiesList()
    {

    }
}

