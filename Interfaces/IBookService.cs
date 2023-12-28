namespace Bookshelf.Interfaces
{
    public interface IBookService
    {
        Task<bool> AddBookToDatabase(Book book);
        Task<bool> RemoveBook(Book book);
		Task<List<Book>> GetAllBooks();
        Task<Book> GetBookFromTitle(string title);
	}
}