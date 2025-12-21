namespace SpaceReserve.Admin.Utility.Resources;

public class CommonResources
{
    public const string UserNotFound = "User ID not found.";
    public const string SeatCount = "Maximum limit to add seat on one Table is 15 and capacity of one floor is 150 seats.";
    public const string Addseat ="Seat Added Successfully";
    public const string SeatSequence ="Invalid Seat Sequence";
    public const string SeatNumber ="Seatnumber is already exists";
    public const string AdminNotFound = "Admin not found.";
    public const string SeatStatusChanged = "Seat status has been changed to ";
    public const string BadRequestError = "Seat status update DTO cannot be null.";
    public const string ServerError = "Internal server error occurred";
    public const string SeatConfiguration = "Seat configuration not found.";
    public const string SeatStatusNotChanged = "Seat Status has not changed";
    public const string BookingTypeForUnAssignedSeat = "Booking";
    public const string NotAvailable = "N/A";

    public const string UnAvailable = "Unavailable";
    public const string UnAssigned = "Unassigned";

    public const string unavailable = "unavailable";
    public const string unassigned = "unassigned";
    public enum BookingStatus : byte
    {
        All = 0,
        Pending = 1,
        Accepted = 2,
        Rejected = 3,
        Cancelled = 4,
    }
    public enum BookingFilterEnum
    {
        All = 0,
        Past = 5,
        Upcoming = 6
    }
    public enum ReservedDesignations : byte
    {
        INFRA = 3,
        DEVOPS = 4,
        ADMINISTRATOR = 6,
        HR = 9,
        ACCOUNTS = 10
    }
}

public static class RegisteredUserCommonResources
{
    public const string BadRequestError = "Invalid sort value. Sort value must be between 0 and 4";
    public const string ServerError = "Internal server error occurred";
    public const string UpdateUserStatus = "The changes have been successfully saved";

}
