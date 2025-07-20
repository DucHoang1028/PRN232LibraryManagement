using BusinessObjects;

namespace Services
{
    public interface IReservationService
    {
        List<Reservation> GetReservations();
        Reservation? GetReservationById(Guid reservationId);
        List<Reservation> GetActiveReservations();
        List<Reservation> GetReservationsByMember(Guid memberId);
        List<Reservation> GetReservationsByBook(Guid bookId);
        Reservation CreateReservation(Guid bookId, Guid memberId);
        bool CancelReservation(Guid reservationId);
        bool FulfillReservation(Guid reservationId);
        void ProcessExpiredReservations();
        List<Reservation> GetReservationsExpiringToday();
        bool IsBookReservedByMember(Guid bookId, Guid memberId);
        void SendReservationNotifications();
    }
} 