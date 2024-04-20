namespace Demo;

public class Mutation
{
    public BookEntity AddBook(Book book) => BookRepository.AddBook(book);
}