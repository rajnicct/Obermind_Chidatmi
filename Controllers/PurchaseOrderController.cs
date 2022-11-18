using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OberMindTestDemo.Data;
using OberMindTestDemo.Models;
using OberMindTestDemo.Models.ViewModel;
using OberMindTestDemo.Services;
using static System.Net.Mime.MediaTypeNames;

namespace OberMindTestDemo.Controllers
{
    [Authorize]
    public class PurchaseOrderController : Controller
    {

        private IPurchaseOrderService _purchaseOrderService;
        private IHttpContextAccessor _HttpContext;
        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IHttpContextAccessor httpContext)
        {
            _purchaseOrderService = purchaseOrderService;
            _HttpContext = httpContext;
        }


        #region Purchase Order Api

        [HttpGet]
        public async Task<IActionResult> PurchaseOrderList()
        {
            PurhcaseOrderViewModel viewModel = new PurhcaseOrderViewModel();
            var PurcahseOrderList = await _purchaseOrderService.GetpurchaseOrderList(_HttpContext.HttpContext);
            var itemList = await _purchaseOrderService.GetitemList();
            var masterList = await _purchaseOrderService.GetOrderMasterList(_HttpContext.HttpContext, 0);
            viewModel.StatusList = await _purchaseOrderService.GetStatusTypeList();
            if (PurcahseOrderList != null)
                viewModel.PurcahseOrderList = PurcahseOrderList;
            if (itemList != null)
                viewModel.ItemList = itemList;
            if (masterList != null)
                viewModel.OrderMasterList = masterList;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> GetItemData(int id)
        {
            List<ItemMaster> ItemList = new List<ItemMaster>();


            ItemList = await _purchaseOrderService.GetitemList();


            return Json(ItemList);
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrderData(int id, string orderName, int statusId)
        {

            var response = await _purchaseOrderService.SubmitOrderMasterData(_HttpContext.HttpContext, id, orderName, statusId);
            return Json(response);

        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrderData(int id)
        {

            var response = await _purchaseOrderService.DeleteOrderData(_HttpContext.HttpContext, id);
            return Json(response);

        }


        #endregion

        #region  Order Details Api

        [HttpGet]
        public async Task<IActionResult> SaveOrderDetail(int id = 0, int status = 1)
        {
            //Get id wise data
            PurhcaseOrderViewModel model = new PurhcaseOrderViewModel();

            if (id < 1)
            {
                model.AddFlag = false;
                model.StatusList = await _purchaseOrderService.GetStatusTypeList();
                model.ItemList = await _purchaseOrderService.GetitemList();
                model.PurcahseOrder.Status = 0;

            }
            else
            {
                model.AddFlag = true;
                model.PurcahseOrder = await _purchaseOrderService.GetOrderDetailById(_HttpContext.HttpContext, id);

                if (status == 1 && model.PurcahseOrder.Status == 1)
                    model.PurcahseOrder.Status = status;
                model.ItemList = await _purchaseOrderService.GetitemList();
                model.OrderMasterList = await _purchaseOrderService.GetOrderMasterList(_HttpContext.HttpContext, id);
                model.StatusList = await _purchaseOrderService.GetStatusTypeList();

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderdetails(int id)
        {
            OrderDetailViewModel orderDetailView = new OrderDetailViewModel();

            if (id > 0)
            {
                orderDetailView = await _purchaseOrderService.GetOrderTotalDetailById(_HttpContext.HttpContext, id);

            }
            return PartialView("_SaveItemDetail", orderDetailView); ;
        }

        [HttpPost]
        public async Task<IActionResult> SaveItemsDatabyOrderId(int orderId, int id, float itemPrice, int flag)
        {
            var response = await _purchaseOrderService.SubmitItemDataByOrderId(_HttpContext.HttpContext, orderId, id, itemPrice, flag);
            return Json(response);

        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrderDetailData(int id)
        {

            var response = await _purchaseOrderService.DeleteOrderDetailData(_HttpContext.HttpContext, id);
            return Json(response);

        }

        #endregion

        #region Other Api

        [HttpGet]
        public async Task<IActionResult> GetItemListData(int id)
        {
            List<ItemMaster> ItemList = new List<ItemMaster>();

            if (id > 0)
            {
                ItemList = await _purchaseOrderService.GetItemListById(_HttpContext.HttpContext, id);

            }
            return PartialView("_ItemList", ItemList); ;
        }

        #endregion





    }
}
