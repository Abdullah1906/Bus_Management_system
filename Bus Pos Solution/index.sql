-- =============================================
-- Bus ticket indexes
-- =============================================

CREATE INDEX IX_BusSeats_BusId
ON BusSeats(BusId);
GO

CREATE INDEX IX_Routes_From_To
ON Routes(FromPlace, ToPlace);
GO

CREATE INDEX IX_Trips_Search
ON Trips(RouteId, TripDate, IsActive);
GO

CREATE INDEX IX_Trips_BusId
ON Trips(BusId);
GO

CREATE UNIQUE INDEX UX_TripSeats_Trip_Seat
ON TripSeats(TripId, BusSeatId);
GO

CREATE INDEX IX_BookingDetails_BookingId
ON BookingDetails(BookingId);
GO

CREATE INDEX IX_Payments_BookingId
ON Payments(BookingId);
GO