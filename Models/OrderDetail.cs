namespace OberMindTestDemo.Models
{
    public class OrderDetail
    {
        public long ID { get; set; }
        public long OrderID { get; set; }
        public long? ItemID { get; set; }
        public double? ItemPrice { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
