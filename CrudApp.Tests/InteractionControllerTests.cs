using System.Collections.Generic;
using CrudApp.models;
using CrudApp.services;
using CrudApp.services.data;
using FluentAssertions;
using Moq;
using Xunit;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Security.Policy;
using System;

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

    [Fact]
    public void CanRemoveBook_WithSuccess()
    {
        // Setup
        var testHelper = new TestHelper();
        var title = "New Title";
        var message = "Book removed successfully";

        var sut = testHelper
            .SetupRemoveBook(title, message)
            .CreateSut();

        // Act
        var response = sut.RemoveBook();

        // Assert
        response.Should().Be((true, message));
    }

    [Fact]
    public void CanUpdateBook_WithSuccess()
    {
        // Setup
        var testHelper = new TestHelper();
        var title = "Existing title";
        var message = "Book updated successfully";
        var updatedBook = new Book()
        {
            Id = 0,
            Title = "Updated Title",
            Author = "Updated Author",
            PublishYear = 1900,
        };

        var sut = testHelper
            .SetupUpdateBook(title, updatedBook, message)
            .CreateSut();

        // Act
        var response = sut.UpdateBook();

        // Assert
        response.Should().Be((true, message));
    }

    //[Fact]
    //public void CanDisplayBooks_WithSuccess()
    //{
    //    // Setup
    //    var testHelper = new TestHelper();
    //    var title = "New Title";
    //    var message = "Book removed successfully";

    //    var sut = testHelper
    //        .SetupRemoveBook(title, message)
    //        .CreateSut();

    //    // Act
    //    var response = sut.RemoveBook();

    //    // Assert
    //    response.Should().Be((true, message));
    //}

    class TestHelper
    {
        private readonly MockRepository _mockRepository = new MockRepository(MockBehavior.Strict);
        public readonly Mock<IBookService> _bookServiceMock;
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

        public TestHelper SetupRemoveBook(string title, string message)
        {
            _userInputServiceMock
                .Setup(x => x.GetUserInput(It.IsAny<string>()))
                .Returns((null, title));

            _bookServiceMock
                .Setup(x => x.RemoveBook(title))
                .Returns((true, message));

            return this;
        }

        public TestHelper SetupUpdateBook(string title, Book updatedBook, string message)
        {

            _userInputServiceMock
                .Setup(x => x.GetUserInput(It.IsAny<string>()))
                .Returns((null, title));

            _userInputServiceMock
                .SetupSequence(x => x.GetUserInputLoop(It.IsAny<string>()))
                .Returns((null, updatedBook.Title))
                .Returns((null, updatedBook.Author));

            _userInputServiceMock
                .Setup(x => x.GetUserInputAsIntLoop(It.IsAny<string>()))
                .Returns((null, updatedBook.PublishYear));

            _bookServiceMock
                .Setup(x => x.UpdateBook(
                    It.IsAny<string>(),
                    It.Is<Book>(b =>
                        b.Title == updatedBook.Title &&
                        b.Author == updatedBook.Author &&
                        b.PublishYear == updatedBook.PublishYear
                    )))
                .Returns((true, message));

            return this;
        }

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