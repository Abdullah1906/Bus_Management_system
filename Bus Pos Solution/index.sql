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

CREATE UNIQUE INDEX UX_Trips_Bus_Date_Time
ON Trips(BusId, RouteId, TripDate, DepartureTime);

-- for place name

CREATE INDEX IX_Places_PlaceName
ON Places (PlaceName);
GO

-- for search on trips
CREATE INDEX IX_TripRecords_IsActive_TripDate_Id
ON TripRecords
(
    IsActive,
    TripDate DESC,
    Id DESC
)
INCLUDE
(
    PlaceId,
    TipStatus,
    TipAmount,
    Price
);
GO