using my_books.Data.Models;
using my_books.Data.Paging;
using my_books.Data.ViewModels;
using my_books.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace my_books.Data.Services
{
    public class PublishersService
    {
        private AppDbContext _context;

        public PublishersService(AppDbContext context)
        {
            _context = context;
        }

        public List<Publisher> GetAllPublishers(string sortBy, string searchString, int? pageNumber, int? pageSize) 
        {
            var _allPublishers = _context.Publishers.OrderBy(n => n.Name).ToList(); // Ascending order by default.

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_descending":
                        _allPublishers = _allPublishers.OrderByDescending(n => n.Name).ToList();
                        break;
                    case "id_ascending":
                        _allPublishers = _allPublishers.OrderBy(n => n.Id).ToList();
                        break;
                    case "id_descending":
                        _allPublishers = _allPublishers.OrderByDescending(n => n.Id).ToList();
                        break;
                    default:
                        break;
                }
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                _allPublishers=_allPublishers.Where(n=>n.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            // Paging
            if(pageSize < 3)
            {
                pageSize = 5;
            }
            _allPublishers = PaginatedList<Publisher>.Create(_allPublishers.AsQueryable(), pageNumber ?? 1, pageSize?? 5);

            return _allPublishers;
        }  

        public Publisher AddPublisher(PublisherVM publisher)
        {
            if (PublisherNameStartsWithNumber(publisher.Name)) throw new PublisherNameException("Name starts with number", publisher.Name);
            var _publisher = new Publisher()
            {
                Name = publisher.Name
            };
            _context.Publishers.Add(_publisher);
            _context.SaveChanges();

            return _publisher;
        }

        public Publisher GetPublisherById(int id) => _context.Publishers.FirstOrDefault(n => n.Id == id);

        public PublisherWithBooksAndAuthorsVM GetPublisherData(int publisherId)
        {
            var _publisherData = _context.Publishers.Where(n => n.Id == publisherId)
                .Select(n => new PublisherWithBooksAndAuthorsVM()
                {
                    Name = n.Name,
                    BookAuthors = n.Books.Select(n => new BookAuthorVM()
                    {
                        BookName = n.Title,
                        BookAuthors = n.Book_Authors.Select(n => n.Author.FullName).ToList()
                    }).ToList()
                }).FirstOrDefault();

            return _publisherData;
        }

        public void DeletePublisherById(int id)
        {
            var _publisher = _context.Publishers.FirstOrDefault(n => n.Id == id);
            if (_publisher != null)
            {
                _context.Publishers.Remove(_publisher);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($"The publisher with the id: {id} does not exsist.");
            }
        }

        private bool PublisherNameStartsWithNumber(string name)
        {
            if (Regex.IsMatch(name, @"^\d")) 
                return true;
            return false;
        }
    }
}
