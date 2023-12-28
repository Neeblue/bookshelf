using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshelf.Interfaces
{
	public interface IBookDetailsService
	{
		Task<Dictionary<string, string>> GetBookDetails(Book book);
	}
}
