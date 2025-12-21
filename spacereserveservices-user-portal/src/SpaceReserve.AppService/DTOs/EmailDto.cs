namespace SpaceReserve.AppService.DTOs
{
    public class EmailDto
    {
        public List<string> CcList { get; set; }
        public List<string> BccList { get; set; }
        public List<int> ToUserId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SenderSubjectId { get; set; }
        private int _senderId;
        public int SenderId
        {
            get => _senderId;
            set
            {
                _senderId = value;
                CreatedBy = value;
            }
        }
        public int ReceiverId { get; set; }
        public int CreatedBy { get; private set; }
        public EmailDto()
        {
            CcList = new List<string> { };
            BccList = new List<string> { };
            ToUserId = new List<int> { };
        }

    }
}