namespace OberMindTestDemo.Models.ViewModel
{
    public class OrderDetailViewModel
    {

        public long ID { get; set; }
        public long OrderID { get; set; }
        public long? ItemID { get; set; }
        public string ItemName { get; set; }
        public double? ItemPrice { get; set; }
    }
}
