using Bookshelf.Interfaces;
using Bookshelf.Models;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Net;
using System.Text.Json.Serialization;

namespace Bookshelf.Services
{
	public class BookService : IBookService
	{
		private IFirebaseClient _client;
		private string DatabasePath { get => "Bookshelf/"; }

		public BookService()
		{
			FireBaseInitialize();			
		}
		private void FireBaseInitialize()
		{
			IFirebaseConfig config = new FirebaseConfig
			{
				AuthSecret = Security.Keys.AuthSecret,
				BasePath = "https://bookshelf-maui-default-rtdb.firebaseio.com"
			};
			_client = new FirebaseClient(config);
		}

		/// <summary>
		/// Removes the book from the Firebase database
		/// </summary>
		/// <param name="book">The book that is to be removed</param>
		/// <returns>True if the book was removed successfully</returns>
		public async Task<bool> RemoveBook(Book book)
		{
			if (String.IsNullOrEmpty(book.FirebaseId) == false)
			{
				_client.Delete($"Bookshelf/{book.FirebaseId}");
			}
			else //If there is no Firebase Id then the entry must be searched for manually
			{
                List<Book> books = new List<Book>();
                FirebaseResponse response = await _client.GetAsync(DatabasePath);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Dictionary<string, Book> bookDict = JsonSerializer.Deserialize<Dictionary<string, Book>>(response.Body);

                    List<string> matchingIds = new List<string>();
                    foreach (var dictionaryBook in bookDict)
                    {
                        if (dictionaryBook.Value.Title == book.Title)
                        {
                            matchingIds.Add(dictionaryBook.Key);
                        }
                    }
                    FirebaseResponse deleteResponse = null;
                    foreach (string Id in matchingIds)
                    {
                        deleteResponse = await _client.DeleteAsync($"Bookshelf/{Id}");
                    }
                    if (deleteResponse.StatusCode == HttpStatusCode.OK)
                        return true;
                }
            }
			
			return false;
		}
		public async Task<List<Book>> GetAllBooks()
		{
			List<Book> books = new List<Book>();
			FirebaseResponse response = await _client.GetAsync(DatabasePath);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				Dictionary<string, Book> bookDict = JsonSerializer.Deserialize<Dictionary<string, Book>>(response.Body);

				foreach (var dicBook in bookDict)
				{
					dicBook.Value.FirebaseId = dicBook.Key;
					books.Add(dicBook.Value);
				}
			}
			return books;
		}
		public async Task<Book> GetBookFromTitle(string title)
		{
			HttpClient httpClient = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage();  
			request.RequestUri = new Uri($"https://www.googleapis.com/books/v1/volumes?langRestrict=en&q={title}&printType=books&orderBy=relevance&projection=full&maxResults=1");
			request.Method = HttpMethod.Get;
			request.Headers.Add("api_key", "AIzaSyA_Cd0wKit5p9Oe8MReeW3Z2yvtNHhtoV4");

			HttpResponseMessage response = await httpClient.SendAsync(request);
			string json = await response.Content.ReadAsStringAsync();

			Json jsonObject = JsonSerializer.Deserialize<Json>(json);
			Book book = ObjectToBook(jsonObject);
			return book;
		}
		public async Task<bool> AddBookToDatabase(Book book)
		{
			var response = await _client.PushAsync(DatabasePath, book);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				return true;
			}
			return false;
		}
		private Book ObjectToBook(Json item)
		{
			string ISBN_10 = "";
            string ISBN_13 = "";
            if (item.items[0].volumeInfo.industryIdentifiers != null)
			{
				foreach (var id in item.items[0].volumeInfo.industryIdentifiers)
				{
					if (id.type == "ISBN_10")
						ISBN_10 = id.identifier;
                    if (id.type == "ISBN_13")
                        ISBN_13 = id.identifier;
                }
			}

			string Image = "";
			if (item.items[0].volumeInfo.imageLinks != null)
				Image = item.items[0].volumeInfo.imageLinks.thumbnail;

			Book book = new()
            {
				Title = item.items[0].volumeInfo.title,
				Description = item.items[0].volumeInfo.description,
				Subtitle = item.items[0].volumeInfo.subtitle,
				Authors = item.items[0].volumeInfo.authors.ToList(),
				Image = Image,
				PageCount = Convert.ToInt32(item.items[0].volumeInfo.pageCount),
				Publisher = item.items[0].volumeInfo.publisher,
				PublishedDate = item.items[0].volumeInfo.publishedDate,
                ISBN_10 = Convert.ToInt64(ISBN_10),
                ISBN_13 = Convert.ToInt64(ISBN_13)				
			};

			return book;
		}
	}
}
