using CrudApp;
using CrudApp.services.data;
using CrudApp.utils;
using CrudApp.services;

public static class Program
{
    public static void Main()
    {
        var fileService = new FileService();
        var fileDbConnection = new FileDbConnection(fileService, FilePathsUtility.filePath);
        var bookService = new BookService(fileDbConnection);
        var userInputService = new UserInputService();
        var interactionController = new InteractionController(bookService, userInputService);

        interactionController.StartInteraction();
    }
}