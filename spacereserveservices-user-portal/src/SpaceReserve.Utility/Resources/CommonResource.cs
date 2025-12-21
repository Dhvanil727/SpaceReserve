namespace SpaceReserve.Utility.Resources;

public static class CommonResource
{
    public const string isBooking = "Booking";
    public const string isRequest = "Request";
    public const int BookingPending = 1;
    public const string AllBooking = "All Booking";
    public const string Rejected = "Rejected By : ";
    public const string Admin = "Admin";
    public const string AutoRejected = "Auto Rejected";
    public const string Approved = "Approved By : ";
    public const string UserNotFound = "User Profile Not Found";
    public const string ServerError = "Internal Server Error";
    public const string CheckColumnId = "Invalid column Id. It must be a number between 1 to 10";
    public const string CheckCityId = "Invalid city Id. It must be a number between 1 or 2";
    public const string OneRequestPending = "There is currently one request pending for this seat. Two additional requests can be accommodated.";
    public const string TwoRequestPending = "There are currently two requests pending for this seat. One additional request can be accommodated.";
    public const string ThreeRequestPending = "This seat is already requested by Three employee, please choose a different available seat for your booking.";
    public const string Seat = "Seat not found.";
    public const string SeatNotAvailable = "This seat cannot be booked";
    public const string RegularUser = "Regular user cannot book a seat";
    public const string SeatAssign = "You have already a seat assigned on this date";
    public const string SeatAlloted = "You have already a seat alloted on this date";
    public const string MaximumLimitUser = "Maximum limit for request seats is reached, up to 3 seats can be requested for a Day";
    public const string MaximumLimit = "This seat is already requested by Three employees, please choose a different available seat for your booking";
    public const string SeatRequested = "You already requested this Seat.";
    public const string SeatBooked = "Your seat request has been submitted";
    public const string messageUnassigned = "You have selected Unassigned seat. Please proceed with clicking on Request";
    public const string messageAvailable = "You have selected available seat. Please proceed with clicking on Request";
}
