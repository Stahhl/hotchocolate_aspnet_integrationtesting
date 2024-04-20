namespace Demo;

public record BookEntity(int Id, Book Book);

public class BookRepository
{
    private static readonly Dictionary<int, Book> Books = new()
    {
        {
            1, new Book
            {
                Title = "C# in depth.",
                Author = new Author
                {
                    Name = "Jon Skeet"
                }
            }
        }
    };

    public static BookEntity? GetBookById(int id)
    {
        Books.TryGetValue(id, out var book);

        return book is not null ? new BookEntity(id, book) : null;
    }

    public static IEnumerable<BookEntity> GetAllBooks()
    {
        return Books.Select(x => new BookEntity(x.Key, x.Value));
    }

    public static BookEntity AddBook(Book book)
    {
        Books.Add(Books.Count + 1, book);

        return new BookEntity(Books.Count, book);
    }
}