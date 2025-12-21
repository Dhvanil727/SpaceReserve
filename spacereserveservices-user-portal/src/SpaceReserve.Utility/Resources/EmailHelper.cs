namespace SpaceReserve.Utility.Resources;

public static class EmailHelper
{
     public static string GenerateSeatRequestEmailBody(string name, DateOnly date, string? city, string? floor, string? seatNumber,string greeting,string message,string actionMessage)
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
                        <p>{greeting}</p>
                        <p>{message}</p>
                        <p><strong>Details:</strong></p>
                        <ul>
                            <li><strong>Name:</strong> {name}</li>
                            <li><strong>Date:</strong> {date}</li>
                            <li><strong>City:</strong> {city}</li>
                            <li><strong>Floor:</strong> {floor}</li>
                            <li><strong>Seat:</strong> {seatNumber}</li>
                        </ul>
                        <p>{actionMessage}</p>
                        <div class=""footer"">
                            <p>Thank you for using Space Reserve.</p>
                            <p>Regards,<br>Space Reserve Team</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
}
