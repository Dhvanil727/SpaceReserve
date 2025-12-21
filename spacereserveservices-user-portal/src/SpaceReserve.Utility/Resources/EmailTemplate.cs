namespace SpaceReserve.Utility.Resources;

public static class EmailTemplate
{
    public static string GetEmailTemplate(string dear, string heading, string status, string rejectedBy, DateOnly bookingDate, string city, string floor, string seat, string endMessage)
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
                <li><strong>{status} </strong> {rejectedBy}</li>
                <li><strong>Date:</strong> {bookingDate}</li>
                <li><strong>City:</strong> {city}</li>
                <li><strong>Floor:</strong> {floor}</li>
                <li><strong>Seat:</strong> {seat}</li>

                
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
    public static string CancelBookingEmail(string dear, string heading, string name, DateOnly bookingDate, string city, string floor, string seat, string endMessage)
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
                <li><strong>Name:</strong> {name}</li>
                <li><strong>Date:</strong> {bookingDate}</li>
                <li><strong>City:</strong> {city}</li>
                <li><strong>Floor:</strong> {floor}</li>
                <li><strong>Seat:</strong> {seat}</li>
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
}
