using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<TripSeat>> LockSeatsAsync(
            long tripId,
            IEnumerable<long> tripSeatIds,
            long customerId);
        Task<Booking?> ConfirmBookingAsync(
           long tripId,
           long customerId,
           string paymentMethod,
           string? transactionId,
           string passengersJson);

        Task<int> ReleaseExpiredSeatLocksAsync();
    }
}
