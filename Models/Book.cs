namespace Bookshelf.Models;

public class Book
{
	public string Title { get; set; }
	public string Subtitle { get; set; }
	public string Description { get; set; }
	public List<string> Authors { get; set; }
	public string Image { get; set; }
	public string ImageHighQuality { get; set; }
	public int PageCount { get; set; }
	public string Publisher { get; set; }
	public string PublishedDate { get; set; }
	public long ISBN_10 { get; set; }
	public long ISBN_13 { get; set; }
	public string PreviewLink { get; set; }
	public string ThumbnailLink { get; set; }
	public string ImageLink { get; set; }
	public string Rating { get; set; }
	public string FirebaseId { get; set; }

	public override bool Equals(object obj)
	{
		return obj is Book book && 
			book.ISBN_13 != 0 && 
			ISBN_13 == book.ISBN_13;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(ISBN_13);
	}
}