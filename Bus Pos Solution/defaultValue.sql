--For Roles

INSERT INTO Roles (Name, Description)
VALUES
('Admin', 'System administrator'),
('Customer', 'Customer'),
('Employee', 'System employee');

INSERT INTO Permissions
(
    Code,
    Name,
    Description,
    Module
)
VALUES
-- Dashboard
('dashboard.view', 'View Dashboard', 'Can view dashboard', 'Dashboard'),

-- Places (আপনার রাউটে places আছে কিন্তু আগের পারমিশনে ছিল না)
('place.view', 'View Place', 'Can view places', 'Place'),
('place.create', 'Create Place', 'Can create place', 'Place'),
('place.edit', 'Edit Place', 'Can edit place', 'Place'),
('place.delete', 'Delete Place', 'Can delete place', 'Place'),

-- Trips (General trips)
('trip.view', 'View Trip', 'Can view trips', 'Trip'),
('trip.create', 'Create Trip', 'Can create trip', 'Trip'),
('trip.edit', 'Edit Trip', 'Can edit trip', 'Trip'),
('trip.delete', 'Delete Trip', 'Can delete trip', 'Trip'),

-- Buses (Admin Buses)
('bus.view', 'View Bus', 'Can view buses', 'Bus'),
('bus.create', 'Create Bus', 'Can create bus', 'Bus'),
('bus.edit', 'Edit Bus', 'Can edit bus', 'Bus'),
('bus.delete', 'Delete Bus', 'Can delete bus', 'Bus'),
('bus.seats.manage', 'Manage Bus Seats', 'Can manage seat layouts for buses', 'Bus'),

-- Bus Seat Sub-permissions (API Specific)
('bus.seat.view', 'View Bus Seats', 'Can view bus seats', 'Bus'),
('bus.seat.create', 'Create Bus Seat', 'Can create bus seats', 'Bus'),
('bus.seat.edit', 'Edit Bus Seat', 'Can edit bus seats', 'Bus'),
('bus.seat.delete', 'Delete Bus Seat', 'Can delete bus seats', 'Bus'),
('bus.seat.status.update', 'Update Bus Seat Status', 'Can update bus seat status', 'Bus'),

-- Routes (Admin Routes)
('route.view', 'View Route', 'Can view routes', 'Route'),
('route.create', 'Create Route', 'Can create route', 'Route'),
('route.edit', 'Edit Route', 'Can edit route', 'Route'),
('route.delete', 'Delete Route', 'Can delete route', 'Route'),

-- Trip Schedules (Admin Trip Schedules)
('schedule.view', 'View Trip Schedule', 'Can view trip schedules', 'Schedule'),
('schedule.create', 'Create Trip Schedule', 'Can create trip schedules', 'Schedule'),
('schedule.edit', 'Edit Trip Schedule', 'Can edit trip schedules', 'Schedule'),
('schedule.delete', 'Delete Trip Schedule', 'Can delete trip schedules', 'Schedule'),

-- Reports
('report.view', 'View Report', 'Can view reports', 'Report'),
('report.export', 'Export Report', 'Can export reports', 'Report'),

-- Search & Booking (Search, Seat Map, Passenger, Payment, Ticket)
('trip.search', 'Search Trip', 'Can search trips', 'Booking'),
('booking.create', 'Create Booking', 'Can create booking', 'Booking'),
('booking.view', 'View Booking', 'Can view booking', 'Booking'),
('booking.cancel', 'Cancel Booking', 'Can cancel booking', 'Booking');


-- customer permission
INSERT INTO RolePermissions
(
    RoleId,
    PermissionId
)
SELECT
    2,
    Id
FROM Permissions
WHERE Code IN
(
    'dashboard.view',
    'trip.search',
    'booking.create',
    'booking.view',
    'booking.cancel'
);

--- admin all permissions
INSERT INTO RolePermissions
(
    RoleId,
    PermissionId
)
SELECT
    1,
    Id
FROM Permissions;


-- delete table data
Truncate table RolePermissions;
DELETE FROM Permissions;
DELETE FROM Roles;

-- then reset id
DBCC CHECKIDENT ('Roles', RESEED, 0);
DBCC CHECKIDENT ('Permissions', RESEED, 0);