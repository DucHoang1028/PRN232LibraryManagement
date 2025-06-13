using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class BookRepository : IBookRepository
    {
        public List<Book> GetBooks() => BookDAO.GetBooks();
    }
}
