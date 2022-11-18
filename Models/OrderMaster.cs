namespace OberMindTestDemo.Models
{
    public class OrderMaster
    {
        public long ID { get; set; }
        public string? OrderNumber { get; set; }
        public int? Status { get; set; }
        public double? Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
    }

}
