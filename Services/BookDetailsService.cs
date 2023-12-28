using Bookshelf.Interfaces;
using HtmlAgilityPack;

namespace Bookshelf.Services
{
	public class BookDetailsService : IBookDetailsService
	{
		public async Task<Dictionary<string, string>> GetBookDetails(Book book)
		{
			Dictionary<string, string> dic = new();
			HtmlWeb web = new();
			HtmlDocument doc = new();

			dic["rating"] = "";
			dic["image"] = "";

			try
			{
				string url = $"https://www.goodreads.com/search?q={book.ISBN_13}";
				doc = await web.LoadFromWebAsync(url);

                HtmlNode ratingNode = doc.DocumentNode.SelectSingleNode("//div[@class='RatingStatistics__rating']");
				if (ratingNode == null)
				{
					dic["rating"] = "Rating not found on Goodreads.";
					return dic;
				}

				HtmlNode ratingCountNode = doc.DocumentNode.SelectSingleNode("//span[@data-testid='ratingsCount']");
				if (ratingCountNode == null)
				{
					dic["rating"] = $"{ratingNode.InnerText}/5 stars";
				}

				dic["rating"] = ratingNode.InnerText + "/5 stars - " + ratingCountNode.InnerText;

				HtmlNode image = doc.DocumentNode.SelectSingleNode("//img[@class='ResponsiveImage']");
				if (image == null)
				{
					dic["image"] = "";
					return dic;
				}
				dic["image"] = image.GetAttributeValue("src", "");
			}
			catch
			{
				return dic;
			}

			return dic;
		}
	}
}
