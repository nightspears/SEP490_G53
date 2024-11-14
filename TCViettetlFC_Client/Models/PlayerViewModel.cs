namespace TCViettetlFC_Client.Models
{
    public class PlayerViewModel
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; }
        public int? ShirtNumber { get; set; }
        public string Position { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? OutDate { get; set; }
        public string Description { get; set; }
        public string BackShirtImage { get; set; }
        public string Status { get; set; }
    }
}
