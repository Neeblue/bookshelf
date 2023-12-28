namespace Bookshelf.ViewModels;

[QueryProperty(nameof(Book), "Book")]
public partial class BookDetailsViewModel : BaseViewModel
{
	[ObservableProperty]
	Book book;

	[ObservableProperty]
	int width;

    partial void OnWidthChanged(int value)
    {
		Shell.Current.DisplayAlert(value.ToString(), "", "cancel");
    }
}