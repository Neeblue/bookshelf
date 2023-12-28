using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Bookshelf.Messages;

public class RemoveBookMessage : ValueChangedMessage<Book>
{
	public RemoveBookMessage(Book value) : base(value) //TODO: Model message only. Delete eventually.
	{
	}
}
