namespace SpaceReserve.Utility.Resources;

public static class EmailConstants
{
    public const string AdminSubject = "Request Received for Unassigned Seat";
    public const string UserSubject = "Request Received for Your Assigned Seat";
    public const string DearAdminGreeting = "Dear Admin,";
    public const string AdminActionMessage = "Please take the required action from the <strong>Booking History</strong>.";
    public const string UserActionMessage = "Please take the required action from the <strong>Booking Request</strong>.";
    public const string AdminRequestMessage="A seat request has been submitted for an unassigned seat.";
    public const string UserRequestMessage="A user has requested access to your assigned seat.";
    public const string DearUserGreeting = "Dear {0} {1},";
}