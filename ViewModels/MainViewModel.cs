using Bookshelf.Interfaces;
using Bookshelf.Models;
using Microsoft.Maui.Networking;
using Newtonsoft.Json;

namespace Bookshelf.ViewModels;

public partial class MainViewModel : BaseViewModel
{
	public ObservableCollection<Book> Books { get; } = new();
	IConnectivity _connectivity;
	IBookService _bookService;
	IBookDetailsService _bookDetailsService;

	[ObservableProperty]
	bool isRefreshing;

	public MainViewModel(IConnectivity connectivity, IBookService bookService, IBookDetailsService bookDetailsService)
	{
		Title = "Bookshelf";
		_connectivity = connectivity;
		_bookService = bookService;
		_bookDetailsService = bookDetailsService;

		GetBooksAsync();
	}

	[RelayCommand]
	async Task GoToDetails(Book book)
	{
		if (IsBusy)
			return;

		if (book == null)
			return;

		try
		{
			if (_connectivity.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("No connectivity!",
					$"Please check internet and try again.", "OK");
				return;
			}

			IsBusy = true;

			await Shell.Current.GoToAsync(nameof(BookDetailsView), true, new Dictionary<string, object>
			{
				{"Book", book }
			});
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to get details: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}

	[RelayCommand]
	async Task GetBooksAsync()
	{
		if (IsBusy || Books.Count > 0)
			return;

		try
		{
			if (_connectivity.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("No connectivity!",
					$"Please check internet and try again.", "OK");
				return;
			}

			IsBusy = true;

			List<Book> newBooks = await _bookService.GetAllBooks();
			foreach (Book book in newBooks)
				Books.Add(book);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to get books: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}

	[RelayCommand]
	async Task AddBookAsync()
	{
		if (IsBusy)
			return;

		try
		{
			if (_connectivity.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("No connectivity!",
					$"Please check internet and try again.", "OK");
				return;
			}

			IsBusy = true;
			IsRefreshing = true;

			string title = await Shell.Current.DisplayPromptAsync("Add a Book", "Enter a book title:", "Search", "Cancel", "Book title...", 30);
			if (title == "")
			{
				await Shell.Current.DisplayAlert("Error!", "Enter a book name", "OK");
				return;
			}

			Book book = await _bookService.GetBookFromTitle(title);

			if (Books.Contains(book))
			{
                await Shell.Current.DisplayAlert("Error!", "Book already added!", "OK");
                return;
			}

            Dictionary<string, string> results = await _bookDetailsService.GetBookDetails(book);
			book.Rating = results["rating"];
			book.ImageHighQuality = results["image"];

			bool result = await _bookService.AddBookToDatabase(book);
			if (result)
				Books.Add(book);

		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to add the book: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}

	[RelayCommand]
	void DeleteBook(Book book)
	{
		Books.Remove(book);
		_bookService.RemoveBook(book);
	}
}
