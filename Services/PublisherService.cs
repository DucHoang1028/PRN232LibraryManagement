using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class PublisherService : IPublisherService
    {
        private readonly ApplicationDbContext _context;

        public PublisherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Publisher> GetPublishers()
        {
            return _context.Publishers
                .Include(p => p.Books)
                .ToList();
        }

        public Publisher? GetPublisherById(Guid id)
        {
            return _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefault(p => p.PublisherId == id);
        }

        public Publisher? GetPublisherByName(string name)
        {
            return _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        }

        public Publisher CreatePublisher(Publisher publisher)
        {
            if (string.IsNullOrWhiteSpace(publisher.Name))
                throw new ArgumentException("Publisher name is required");

            if (GetPublisherByName(publisher.Name) != null)
                throw new ArgumentException("Publisher with this name already exists");

            publisher.PublisherId = Guid.NewGuid();
            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            return publisher;
        }

        public Publisher UpdatePublisher(Guid id, Publisher publisher)
        {
            var existingPublisher = GetPublisherById(id);
            if (existingPublisher == null)
                throw new ArgumentException("Publisher not found");

            if (string.IsNullOrWhiteSpace(publisher.Name))
                throw new ArgumentException("Publisher name is required");

            var publisherWithSameName = GetPublisherByName(publisher.Name);
            if (publisherWithSameName != null && publisherWithSameName.PublisherId != id)
                throw new ArgumentException("Publisher with this name already exists");

            existingPublisher.Name = publisher.Name;
            existingPublisher.Address = publisher.Address;

            _context.SaveChanges();

            return existingPublisher;
        }

        public bool DeletePublisher(Guid id)
        {
            var publisher = GetPublisherById(id);
            if (publisher == null)
                return false;

            if (publisher.Books.Any())
                throw new InvalidOperationException("Cannot delete publisher with associated books");

            _context.Publishers.Remove(publisher);
            _context.SaveChanges();

            return true;
        }
    }
} 