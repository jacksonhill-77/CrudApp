using System.Collections.Generic;
using CrudApp.models;
using CrudApp.services;
using CrudApp.services.data;
using FluentAssertions;
using Moq;
using Xunit;

namespace DefaultNamespace;
/// <summary>
/// SUT = Subject Under Test
/// </summary>
public class BookServiceTests
{
    [Fact]
    public void CanAddBook_WithSuccess()
    {
        // Setup
        var testHelper = new TestHelper();
        var newBook = new Book()
        {
            Id = 1,
            Title = "Test Title",
            Author = "Test Author",
        };

        var sut = testHelper
            .SetupGetBookByTitle(newBook.Title, false)
            .SetupAddBook(newBook)
            .CreateSut();

        // Act
        var response = sut.AddBook(newBook); 

        // Assert
        response.Should().Be((true, null)!);
    }
    
    [Fact]
    public void CanAddBook_ReportsDuplicate()
    {
        // Setup
        var testHelper = new TestHelper();
        var newBook = new Book()
        {
            Title = "Existing title",
        };
        var sut = testHelper
            .SetupGetBookByTitle(newBook.Title, true)
            .CreateSut();

        // Act
        var response = sut.AddBook(newBook); 

        // Assert
        response.Should().Be((false, "Already exists"), "Because the business checks for this exact response string");
    }

    [Fact]

    public void CanFetchBooks_WithSuccess()
    {
        // Setup
        var testHelper = new TestHelper();
        var newBook1 = new Book()
        {
            Title = "Existing title",
            Author = "Existing author",
            PublishYear = 1901,
        };
        var newBook2 = new Book()
        {
            Title = "Existing title 2",
            Author = "Existing author 2",
            PublishYear = 1902,
        };

        var expectedBooks = new List<Book> { newBook1, newBook2 };

        var sut = testHelper
            .SetupReadDatabase(expectedBooks)
            .CreateSut();

        // Act
        var response = sut.FetchBooks();

        // Assert 
        response.Should().Be((true, null, expectedBooks));
    }

    [Fact]

    public void CanUpdateBook_WithSuccess()
    {
        // Setup
        var testHelper = new TestHelper();
        var newBook = new Book()
        {
            Title = "Existing title",
            Author = "Existing author",
            PublishYear = 1901,
        };
        var updatedBook = new Book() 
        {
            Title = "Updated title",
            Author = "Updated author",
            PublishYear = 1902,
        };

        var sut = testHelper
            .SetupUpdateBook(newBook.Title, updatedBook)
            .CreateSut();

        // Act
        var response = sut.UpdateBook(newBook.Title, updatedBook);

        // Assert 
        response.Should().Be((true, null, updatedBook));
    }

    [Fact]

    public void CanRemoveBook_WithSuccess()
    {
        // Setup
        var testHelper = new TestHelper();
        var book = new Book()
        {
            Title = "Existing title",
            Author = "Existing author",
            PublishYear = 1901,
        };

        var sut = testHelper
            .SetupRemoveBook(book.Title)
            .CreateSut();

        // Act
        var response = sut.RemoveBook(book.Title);

        // Assert 
        response.Should().Be((true, null, updatedBook));
    }




    class TestHelper
    {
        private readonly MockRepository _mockRepository = new MockRepository(MockBehavior.Strict);
        readonly Mock<IBookRepository> _bookRepositoryMock;
        
        public TestHelper()
        {
            _bookRepositoryMock = _mockRepository.Create<IBookRepository>();
        }

        public TestHelper SetupGetBookByTitle(string expectedTitle, bool doesExist)
        {
            _bookRepositoryMock
                .Setup(x => x.GetBookByTitle(expectedTitle))
                .Returns(doesExist
                    ? (new Book()
                    {
                        Title = expectedTitle, 
                    }, null, null )
                    : (null, null, "Could not find book title"));
            return this;
        }

        public TestHelper SetupReadDatabase(List<Book> expectedBooks)
        {
            _bookRepositoryMock
                .Setup(x => x.ReadDatabase())
                .Returns(expectedBooks);

            return this;
        }

        public TestHelper SetupAddBook(Book book)
        {
            _bookRepositoryMock
                .Setup(x => x.AddBook(book))
                .Returns((true, null));
            
            return this;
        }

        public TestHelper SetupUpdateBook(string titleOfBookToUpdate, Book updatedBook)
        {
            _bookRepositoryMock
                .Setup(x => x.UpdateBook(titleOfBookToUpdate, updatedBook))
                .Returns((true, "Book added successfully"));

            return this;
        }

        public TestHelper SetupRemoveBook(string titleOfBookToRemove)
        {
            _bookRepositoryMock
                .Setup(x => x.RemoveBook(titleOfBookToRemove))
                .Returns((true, "Book added successfully"));

            return this;
        }

        public BookService CreateSut()
        {
            return new BookService(_bookRepositoryMock.Object);
        }
    }
}