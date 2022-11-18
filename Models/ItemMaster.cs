namespace OberMindTestDemo.Models
{
    public class ItemMaster
    {
        public long ID { get; set; }
        public string? ItemNumber { get; set; }
        public double? ItemPrice { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
