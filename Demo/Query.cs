namespace Demo;

public class Query
{
    public BookEntity? GetBookById(int id) => BookRepository.GetBookById(id);

    public IEnumerable<BookEntity> GetBooks() => BookRepository.GetAllBooks();
}
