namespace Bookshelf.Views;

public partial class BookDetailsView : ContentPage
{
	public BookDetailsView(BookDetailsViewModel bookDetailsViewModel)
	{
		InitializeComponent();
		BindingContext = bookDetailsViewModel;
	}
}