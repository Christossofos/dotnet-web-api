using my_books.Data.Models;
using my_books.Data.Paging;
using my_books.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_books.Data.Services
{
    public class AuthorsService
    {
        private AppDbContext _context;

        public AuthorsService(AppDbContext context)
        {
            _context = context;
        }

        public List<Author> GetAllAuthors(string orderBy, string searchString, int? pageNumber, int? pageSize)
        {
            var _allAuthors = _context.Authors.OrderBy(n => n.FullName).ToList();
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy)
                {
                    case "name_descending":
                        _allAuthors = _allAuthors.OrderByDescending(n => n.FullName).ToList();
                        break;
                    case "id_ascending":
                        _allAuthors = _allAuthors.OrderBy(n => n.Id).ToList();
                        break;
                    case "id_descending":
                        _allAuthors = _allAuthors.OrderByDescending(n => n.Id).ToList();
                        break;
                    default:
                        break;
                }
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                _allAuthors = _allAuthors.Where(n => n.FullName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }

            // Paging
            if (pageSize < 3)
            {
                pageSize = 5;
            }
            _allAuthors = PaginatedList<Author>.Create(_allAuthors.AsQueryable(), pageNumber ?? 1, pageSize ?? 5);

            return _allAuthors;
        }

        public void AddAuthor(AuthorVM author)
        {
            var _author = new Author()
            {
                FullName = author.FullName
            };
            _context.Authors.Add(_author);
            _context.SaveChanges();
        }

        public AuthorWithBooksVM GetAuthorWithBooks(int authorId)
        {
            var _author = _context.Authors.Where(n => n.Id == authorId).Select(n => new AuthorWithBooksVM()
            {
                FullName = n.FullName,
                BookTitles = n.Book_Authors.Select(n => n.Book.Title).ToList()
            }).FirstOrDefault();

            return _author;
        }

        public AuthorWithBooksVM GetAuthorById(int authorId)
        {
            var _author = _context.Authors.Where(n => n.Id == authorId).Select(n => new AuthorWithBooksVM()
            {
                FullName = n.FullName
            }).FirstOrDefault();

            return _author;
        }
    }
}
