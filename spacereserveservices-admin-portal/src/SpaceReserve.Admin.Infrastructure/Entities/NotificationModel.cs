using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceReserve.Infrastructure.Entities
{
    public class NotificationModel
    {
        [Key]
        public int NotificationId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [ForeignKey("SenderId")]
        public virtual User? Sender { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual User? Receiver { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }
    }
}