using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Bookings
{
    public class ConfirmBookingResult
    {
        public Booking Booking { get; set; } = new();

        public List<ConfirmedPassengerDto> Passengers { get; set; } = new();
    }
}
