namespace SpaceReserve.Admin.Utility.Resources;

public static class EmailHelper
{
    public const string UnAssignedUser = "Your Seat Has Been Unassigned by Admin";
    public const string UnAvailableUser = "Your Seat Has Been Unavailable by Admin";
    public const string CancelledUser = "Your Seat Reservation Has Been Cancelled by Admin";
    public const string UnAvailable = "Unavailable";
    public const string UnAssigned = "Unassigned";
    public const string Rejected = "Rejected By : ";
    public const string Approved = "Approved By : ";
    public const string AutoReject = "Auto Rejected";
    public const string SeatRequestRejectSubject = "Your Seat Request Has Been Rejected";
    public const string SeatRequestRejectStatus = "Your seat booking request has been rejected.";
    public const string SeatRequestRejectAutoMessage = "You may submit a new request or select another seat from the Home Page.";
    public const string SeatRequestApproveSubject = "Your Seat Request Has Been Approved";
    public const string SeatRequestApproveStatus = "Your seat booking request has been Approved.";
    public const string SeatRequestApproveAutoMessage = "You may view or cancel your reservation from your Booking History.";
    public const string UserOtherSeatsRequestRejectSubject = "Your Other Seat Request Was Rejected";
    public const string UserOtherSeatsRequestRejectStatus = "Your other pending seat request has been automatically rejected.";
    public const string UserOtherSeatsRequestRejectAutoMessage = "You may choose a new seat from the Home Page.";
    public const string OtherUsersRequestOfSeatRejectSubject = "Your Seat Request Has Been Rejected";
    public const string OtherUsersRequestOfSeatRejectStatus = "Your seat booking request has been rejected.";
    public const string OtherUsersRequestOfSeatRejectAutoMessage = "You may submit a new request or select another seat from the Home Page.";
    public static string UnAssignedUserEmail(string firstName, string lastName, DateTime? deleteDate, string city, string floor, string seatNo, string message,string messageBy)
    {
        return $@"
                    <html>
                    <head>
                        <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                        .container {{ margin: 20px; }}
                        .footer {{ margin-top: 30px; font-size: 14px; color: #555; }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <p>Dear {firstName + " " + lastName},</p>
                            <p>Your assigned seat has been {messageBy} by the Admin.</p>
                            <p><strong>Details:</strong></p>
                            <ul>
                                <li><strong>Date:</strong> {deleteDate}</li>
                                <li><strong>City:</strong> {city}</li>
                                <li><strong>Floor:</strong> {floor}</li>
                                <li><strong>Seat:</strong> {seatNo}</li>
                                <li><strong>{message} By:</strong>Admin</li>
                            </ul>
                            <div class=""footer"">
                                <p>Thank you for using Space Reserve.</p>
                                <p>Regards,<br>Space Reserve Team</p>
                            </div>
                        </div>
                    </body>
                    </html>";
    }
    public static string CancelledUserEmail(string firstName, string lastName, DateTime? modifiedDate, string city, string floor, string seatNo, string message)
    {
        return $@"
                        <html>
                        <head>
                            <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                            .container {{ margin: 20px; }}
                            .footer {{ margin-top: 30px; font-size: 14px; color: #555; }}
                            </style>
                        </head>
                        <body>
                            <div class=""container"">
                                <p>Dear {firstName + " " + lastName},</p>
                                <p>Your seat reservation for the date mentioned below has been cancelled because the Admin {message} it from the seat owner.</p>
                                <p><strong>Details:</strong></p>
                                <ul>
                                    <li><strong>Date:</strong> {modifiedDate}</li>
                                    <li><strong>City:</strong> {city}</li>
                                    <li><strong>Floor:</strong> {floor}</li>
                                    <li><strong>Seat:</strong> {seatNo}</li>
                                    <li><strong>Cancelled By:</strong>Admin</li>
                                </ul>
                                <p>You may book another seat from the Home Page or view this update in your Booking History.</p>
                                <div class=""footer"">
                                    <p>Thank you for using Space Reserve.</p>
                                    <p>Regards,<br>Space Reserve Team</p>
                                </div>
                            </div>
                        </body>
                        </html>";
    }
    public static string GenerateSeatRequestEmailBody(string dear, string heading, string status, string autoMessage, DateOnly date, string? city, string? floor, string? seatNumber, string endMessage)
    {
        return $@"
            <html>
            <head>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                .container {{ margin: 20px; }}
                .footer {{ margin-top: 30px; font-size: 14px; color: #555; }}
            </style>
            </head>
            <body>
            <div class=""container"">
                <p>Dear {dear},</p>

                <p>{heading}</p>

                <p><strong>Details:</strong></p>
                <ul>
                <li><strong>{status}</strong>{autoMessage}</li>
                <li><strong>Date:</strong> {date}</li>
                <li><strong>City:</strong> {city}</li>
                <li><strong>Floor:</strong> {floor}</li>
                <li><strong>Seat:</strong> {seatNumber}</li>
                </ul>

                <p>{endMessage}</p>

                <div class=""footer"">
                <p>Thank you for using Space Reserve.</p>
                <p>Regards,<br>Space Reserve Team</p>
                </div>
            </div>
            </body>
            </html>";
    }
    public static string EmailForCancelBooking(string dear)
    {
        return $@"
            <html>
            <head>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                .container {{ margin: 20px; }}
                .footer {{ margin-top: 30px; font-size: 14px; color: #555; }}
            </style>
            </head>
            <body>
            <div class=""container"">
                <p>Dear {dear},</p>

                <p>Your access to the Space Reserve application has been deactivated by the administrator. As a result, you will no longer be able to submit seat requests, make bookings, or access the platform.</p>

                <p>For further information, please contact your administrator or facility team.</p>

                <div class=""footer"">
                <p>Thank you for using Space Reserve.</p>
                <p>Regards,<br>Space Reserve Team</p>
                </div>
            </div>
            </body>
            </html>";
    }

}
