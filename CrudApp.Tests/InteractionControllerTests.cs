using System.Collections.Generic;
using CrudApp.models;
using CrudApp.services;
using CrudApp.services.data;
using FluentAssertions;
using Moq;
using Xunit;
using System.Linq;

namespace CrudApp.Tests;
/// <summary>
/// SUT = Subject Under Test
/// </summary>
public class InteractionControllerTests
{
    [Fact]
    public void CanAddBook_WithSuccess()
    {
        // Setup
        var testHelper = new TestHelper();
        var book = new Book()
        {
            Id = 1,
            Title = "Test Title",
            Author = "Test Author",
            PublishYear = 1900,
        };

        var sut = testHelper
            .SetupAddBook(book, "Book added successfully", book.Title, book.Author, book.PublishYear)
            .CreateSut();

        // Act
        var response = sut.AddBook();

        // Assert
        response.Should().Be((true, null));
    }

    //[Fact]
    //public void CanRemoveBook_WithSuccess()
    //{
    //    // Setup
    //    var testHelper = new TestHelper();
    //    var book1 = new Book()
    //    {
    //        Id = 1,
    //        Title = "Book 1",
    //        Author = "Author 1",
    //        PublishYear = 1901,
    //    };

    //    var book2 = new Book()
    //    {
    //        Id = 2,
    //        Title = "Book 2",
    //        Author = "Author 2",
    //        PublishYear = 1902,
    //    };

    //    var books = new List<Book>
    //    {   book1,
    //        book2
    //    };

    //    var booksPreRemoval = books.ConvertAll(book => fileDbConnection.ConvertBookToJSON(book));
    //    var booksPostRemoval = booksPreRemoval
    //        .Skip(1)
    //        .ToList();

    //    var sut = testHelper
    //        .SetupRemoveBook(booksPreRemoval, booksPostRemoval)
    //        .CreateSut();

    //    // Act
    //    sut.RemoveBook(book1.Title);

    //    // Assert
    //    testHelper._booksPostRemoval.Should().BeEquivalentTo(booksPostRemoval);
    //}

    //[Fact]
    //public void CanUpdateBook_WithSuccess()
    //{
    //    // Setup
    //    var testHelper = new TestHelper();
    //    var fileService = new FileService();
    //    var filePath = "mockpath";
    //    var fileDbConnection = new FileDbConnection(new FileService(), filePath);
    //    var originalBook = new Book()
    //    {
    //        Id = 1,
    //        Title = "Book 1",
    //        Author = "Author 1",
    //        PublishYear = 1901,
    //    };

    //    var updatedBook = new Book()
    //    {
    //        Id = 2,
    //        Title = "Book 2",
    //        Author = "Author 2",
    //        PublishYear = 1902,
    //    };

    //    var originalBookJSON = new List<string>()
    //    {
    //        fileDbConnection.ConvertBookToJSON(originalBook)
    //    };

    //    var updatedBookJSON = new List<string>()
    //    {
    //        fileDbConnection.ConvertBookToJSON(updatedBook)
    //    };

    //    var sut = testHelper
    //        .SetupUpdateBook(originalBookJSON, updatedBookJSON)
    //        .CreateSut();

    //    // Act
    //    sut.UpdateBook(originalBook.Title, updatedBook);

    //    // Assert
    //    testHelper._booksPostUpdate.Should().BeEquivalentTo(updatedBookJSON);
    //}

    //[Fact]
    //public void CanReadDatabase_WithSuccess()
    //{
    //    // Setup
    //    var testHelper = new TestHelper();
    //    var fileService = new FileService();
    //    var filePath = "mockpath";
    //    var fileDbConnection = new FileDbConnection(new FileService(), filePath);
    //    var book1 = new Book()
    //    {
    //        Id = 1,
    //        Title = "Book 1",
    //        Author = "Author 1",
    //        PublishYear = 1901,
    //    };

    //    var book2 = new Book()
    //    {
    //        Id = 2,
    //        Title = "Book 2",
    //        Author = "Author 2",
    //        PublishYear = 1902,
    //    };

    //    var books = new List<Book>
    //    {   book1,
    //        book2
    //    };

    //    var databaseBooks = books.ConvertAll(book => fileDbConnection.ConvertBookToJSON(book));

    //    var sut = testHelper
    //        .SetupReadDatabase(databaseBooks)
    //        .CreateSut();

    //    // Act
    //    var databaseReadResult = sut.ReadDatabase();

    //    // Assert
    //    databaseReadResult.Should().BeEquivalentTo(books);
    //}

    class TestHelper
    {
        private readonly MockRepository _mockRepository = new MockRepository(MockBehavior.Strict);
        readonly Mock<IBookService> _bookServiceMock;
        readonly Mock<IUserInputService> _userInputServiceMock;


        public TestHelper()
        {
            _bookServiceMock = _mockRepository.Create<IBookService>();
            _userInputServiceMock = _mockRepository.Create<IUserInputService>();
        }

        public TestHelper SetupAddBook(Book book, string message, string title, string author, int publishYear)
        {

            _bookServiceMock
                .Setup(x => x.AddBook(It.Is<Book>(b =>
                    b.Title == book.Title &&
                    b.Author == book.Author &&
                    b.PublishYear == book.PublishYear
                    )))
                .Returns((true, null));

            _userInputServiceMock
                .SetupSequence(x => x.GetUserInputLoop(It.IsAny<string>()))
                .Returns((null, title))
                .Returns((null, author));

            _userInputServiceMock
                .Setup(x => x.GetUserInputAsIntLoop(It.IsAny<string>()))
                .Returns((null, publishYear));

            return this;
        }

        //public TestHelper SetupRemoveBook(List<string> booksPreRemoval, List<string> booksPostRemoval)
        //{
        //    _fileServiceMock
        //        .Setup(x => x.ReadLinesFromFile(_filePath))
        //        .Returns(booksPreRemoval);

        //    _fileServiceMock
        //        .Setup(x => x.WriteLinesToFile(booksPostRemoval, _filePath))
        //        .Callback<List<string>, string>((booksPostRemoval, filePath) => _booksPostRemoval = booksPostRemoval)
        //        .Returns((true, null));

        //    return this;
        //}

        //public TestHelper SetupUpdateBook(List<string> booksPreUpdate, List<string> booksPostUpdate)
        //{
        //    _fileServiceMock
        //        .Setup(x => x.ReadLinesFromFile(_filePath))
        //        .Returns(booksPreUpdate);

        //    _fileServiceMock
        //        .Setup(x => x.WriteLinesToFile(booksPostUpdate, _filePath))
        //        .Callback<List<string>, string>((booksPostUpdate, filePath) => _booksPostUpdate = booksPostUpdate)
        //        .Returns((true, null));

        //    return this;
        //}

        //public TestHelper SetupReadDatabase(List<string> databaseBooks)
        //{
        //    _fileServiceMock
        //        .Setup(x => x.ReadLinesFromFile(_filePath))
        //        .Returns(databaseBooks);

        //    return this;
        //}

        public InteractionController CreateSut()
        {
            return new InteractionController(_bookServiceMock.Object, _userInputServiceMock.Object);
        }
    }
}