using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace DataAccessObjects
{
    public class BookDAO
    {
        public static List<Book> GetBooks()
        {
            var listBooks = new List<Book>();
            try
            {
                using var context = new ApplicationDbContext();
                listBooks = context.Books.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listBooks;
        }
    }
}
