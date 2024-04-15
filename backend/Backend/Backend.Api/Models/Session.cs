namespace Backend.Api.Models
{
    public class Session : ModelBase
    {
        public string AccountId { get; set; }

        public bool IsExpired { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? ExpiredOn { get; set; }
    }
}
