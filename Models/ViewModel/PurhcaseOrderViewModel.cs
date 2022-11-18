using Microsoft.AspNetCore.Mvc.Rendering;

namespace OberMindTestDemo.Models.ViewModel
{
    public class PurhcaseOrderViewModel
    {

        public PurhcaseOrderViewModel()
        {
            PurcahseOrder = new OrderMaster();
            PurcahseOrderList = new List<OrderMaster>();
            OrderMasterList = new List<OrderDetailViewModel>();
            ItemList = new List<ItemMaster>();
            OrderDetail = new OrderDetailViewModel();
            StatusList = new List<SelectListItem>();
            ItemMaster = new ItemMaster();
        }
        public bool AddFlag { get; set; }
        public List<OrderMaster> PurcahseOrderList { get; set; }
        public List<OrderDetailViewModel> OrderMasterList { get; set; }
        public OrderDetailViewModel OrderDetail { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public OrderMaster PurcahseOrder { get; set; }
        public List<ItemMaster> ItemList { get; set; }
        public ItemMaster ItemMaster { get; set; }
    }
}
