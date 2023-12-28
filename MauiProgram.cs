using Bookshelf.Interfaces;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace Bookshelf;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		//Services
		builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
		builder.Services.AddTransient<IBookDetailsService, BookDetailsService>();
		builder.Services.AddTransient<IBookService, BookService>();
		//ViewModels
		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddTransient<BookDetailsViewModel>();
		//Views
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<BookDetailsView>();


		return builder.Build();
	}
}
