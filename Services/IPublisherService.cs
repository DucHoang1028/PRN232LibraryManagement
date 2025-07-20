using BusinessObjects;

namespace Services
{
    public interface IPublisherService
    {
        IEnumerable<Publisher> GetPublishers();
        Publisher? GetPublisherById(Guid id);
        Publisher? GetPublisherByName(string name);
        Publisher CreatePublisher(Publisher publisher);
        Publisher UpdatePublisher(Guid id, Publisher publisher);
        bool DeletePublisher(Guid id);
    }
} 