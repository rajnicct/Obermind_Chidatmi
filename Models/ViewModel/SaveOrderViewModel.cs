using Microsoft.AspNetCore.Mvc.Rendering;

namespace OberMindTestDemo.Models.ViewModel
{
    public class SaveOrderViewModel
    {
        public SaveOrderViewModel()
        {
            PurcahseOrder = new OrderMaster();
            ItemList = new List<ItemMaster>();
            OrderDetail = new OrderDetail();
            StatusList = new List<SelectListItem>();
            ItemMaster = new ItemMaster();
        }
        public bool AddFlag { get; set; }
        public OrderDetail OrderDetail { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public OrderMaster PurcahseOrder { get; set; }
        public List<ItemMaster> ItemList { get; set; }
        public ItemMaster ItemMaster { get; set; }
    }
}
