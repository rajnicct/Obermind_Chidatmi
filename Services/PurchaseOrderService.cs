using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OberMindTestDemo.Data;
using OberMindTestDemo.Models;
using OberMindTestDemo.Models.ViewModel;
using System.Security.Claims;

namespace OberMindTestDemo.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {

        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        public PurchaseOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Purchase Order Methods

        public async Task<List<OrderMaster>> GetpurchaseOrderList(HttpContext context)
        {
            try
            {
                var id = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var res = _context.OrderMaster.Where(x => x.CreatedBy == id).ToList();
                return res;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        public async Task<OrderMaster> GetOrderDetailById(HttpContext context, int id)
        {
            try
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var res = _context.OrderMaster.Where(x => x.ID == id && x.CreatedBy == userId).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        public async Task<JsonResponseModel> SubmitOrderMasterData(HttpContext context, int orderId, string orderNumber, int status)
        {
            try
            {
                var response = new JsonResponseModel();
                OrderMaster master = new OrderMaster();
                var id = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var existONumber = DateTime.UtcNow.ToShortDateString() + "-" + orderNumber;
                var SubmittedorderListBydate = _context.OrderMaster.Where(x => x.CreatedOn.Date == DateTime.UtcNow.Date && x.CreatedBy == id && x.Status == 2).ToList();

                var isexist = _context.OrderMaster.Where(x => x.OrderNumber == existONumber && x.CreatedBy == id).FirstOrDefault();

                var data = _context.OrderMaster.Where(x => x.ID == orderId && x.CreatedBy == id).FirstOrDefault();
                var itemListByOrder = _context.OrderDetail.Where(x => x.OrderID == orderId && x.CreatedBy == id).ToList().Count();
                if (SubmittedorderListBydate.Count == 10 && status == 2)
                {
                    response.message = "Maximum 10 order you will be process as Submitted on this date " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ".";
                    response.status = false;
                    response.data = orderId;
                }
                else
                {
                    if (status == 2 && data == null)
                    {
                        response.message = "On Create Time Plz select status as Draft.";
                        response.status = false;
                    }
                    else if (data != null && orderId > 0)
                    {
                        if (itemListByOrder == 0 && status == 2)
                        {
                            response.message = "minimum one item Record will be exists in this Order.";
                            response.status = false;
                            response.data = orderId;

                        }
                        else
                        {
                            data.Status = status;
                            data.UpdatedOn = DateTime.UtcNow;
                            data.UpdatedBy = id;
                            _context.OrderMaster.Update(data);
                            _context.SaveChanges();
                            response.data = orderId;
                            response.message = "Order Update SuccessFully";
                            response.status = true;



                        }
                    }
                    else if (isexist != null)
                    {
                        response.message = "Order allready Exists. plz Enter different Order Number.";
                        response.status = false;
                    }
                    else
                    {
                        master.OrderNumber = DateTime.UtcNow.ToShortDateString() + "-" + orderNumber;
                        master.Status = status;
                        master.Amount = 0;
                        master.CreatedOn = DateTime.UtcNow;
                        master.UpdatedOn = DateTime.UtcNow;
                        master.CreatedBy = id;
                        _context.OrderMaster.Add(master);
                        _context.SaveChanges();
                        response.message = "Order Create SuccessFully";
                        var orderAddId = _context.OrderMaster.Where(x => x.OrderNumber == existONumber && x.CreatedBy == id).FirstOrDefault();
                        response.status = true;
                        response.data = orderAddId.ID;
                    }
                }



                return new JsonResponseModel
                {
                    status = response.status,
                    message = response.message,
                    data = response.data

                };
            }
            catch (Exception ex)
            {
                return new JsonResponseModel
                {
                    status = false,
                    message = ex.Message.ToString(),
                    data = 0
                };
            }

        }

        public async Task<JsonResponseModel> DeleteOrderData(HttpContext context, int id)
        {
            try
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var response = new JsonResponseModel();
                if (id > 0)
                {
                    var data = _context.OrderMaster.Where(x => x.ID == id && x.CreatedBy == userId).FirstOrDefault();
                    var orderTotalData = _context.OrderDetail.Where(x => x.OrderID == id && x.CreatedBy == userId).ToList();
                    response.data = 0;

                    if (data != null)
                    {
                        // Delete  OrdeMAster Table
                        _context.OrderMaster.Remove(data);
                        _context.SaveChanges();
                        if (orderTotalData.Count > 0)
                        {
                            foreach (var details in orderTotalData)
                            {
                                // Delete OrderTotalTable
                                _context.OrderDetail.Remove(details);
                                _context.SaveChanges();
                            }
                        }
                        response.message = "Order Delete SuccessFully";
                        response.status = true;
                    }
                }
                return new JsonResponseModel
                {
                    status = response.status,
                    message = response.message,
                    data = response.data
                };
            }
            catch (Exception ex)
            {
                return new JsonResponseModel
                {
                    status = false,
                    message = ex.Message.ToString()
                };
            }
        }

        #endregion

        #region  Order Details Methods

        public async Task<List<OrderDetailViewModel>> GetOrderMasterList(HttpContext context, int id)
        {
            try
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                List<OrderDetailViewModel> orderslist = new List<OrderDetailViewModel>();
                if (id > 0)
                {
                    var ordersDatalist = _context.OrderDetail.Where(x => x.OrderID == id && x.CreatedBy == userId).ToList();
                    foreach (var data in ordersDatalist)
                    {
                        if (data != null)
                        {
                            var itemData = _context.ItemMaster.Where(x => x.ID == data.ItemID).FirstOrDefault();
                            if (itemData != null)
                            {
                                orderslist.Add(new OrderDetailViewModel
                                {
                                    ID = data.ID,
                                    OrderID = data.OrderID,
                                    ItemID = data.ItemID,
                                    ItemName = itemData.ItemNumber,
                                    ItemPrice = data.ItemPrice

                                });
                            }
                        }
                    }

                }
                else
                {
                    var ordersDatalist = _context.OrderDetail.ToList();
                    foreach (var data in ordersDatalist)
                    {
                        if (data != null)
                        {
                            var itemData = _context.ItemMaster.Where(x => x.ID == data.ItemID).FirstOrDefault();
                            if (itemData != null)
                            {
                                orderslist.Add(new OrderDetailViewModel
                                {
                                    ID = data.ID,
                                    OrderID = data.OrderID,
                                    ItemID = data.ItemID,
                                    ItemName = itemData.ItemNumber,
                                    ItemPrice = data.ItemPrice

                                });
                            }
                        }
                    }
                }

                return orderslist;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        public async Task<OrderDetailViewModel> GetOrderTotalDetailById(HttpContext context, int id)
        {
            try
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                OrderDetailViewModel viewModel = new OrderDetailViewModel();
                var res = _context.OrderDetail.Where(x => x.ID == id && x.CreatedBy == userId).FirstOrDefault();
                if (res != null)
                {
                    var itemName = _context.ItemMaster.Where(x => x.ID == res.ItemID).FirstOrDefault();
                    viewModel.ID = id;
                    viewModel.OrderID = res.OrderID;
                    viewModel.ItemName = itemName.ItemNumber;
                    viewModel.ItemPrice = res.ItemPrice;
                    viewModel.ItemID = res.ItemID;
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        public async Task<JsonResponseModel> SubmitItemDataByOrderId(HttpContext context, int orderId, int itemId, float itemPrice, int flag)
        {
            try
            {
                var response = new JsonResponseModel();
                OrderDetail master = new OrderDetail();

                if (orderId != null)
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var itemListByOrder = _context.OrderDetail.Where(x => x.OrderID == orderId && x.CreatedBy == userId).ToList().Count();
                    var data = _context.OrderDetail.Where(x => x.OrderID == orderId && x.ItemID == itemId && x.CreatedBy == userId).FirstOrDefault();
                    var orderData = _context.OrderMaster.Where(x => x.ID == orderId && x.CreatedBy == userId).FirstOrDefault();
                    if (itemListByOrder < 10)
                    {
                        if (orderData.Amount == null)
                            orderData.Amount = 0;

                        if (data != null)
                        {
                            if (flag == 0)
                            {
                                orderData.Amount = orderData.Amount + itemPrice;
                                data.ItemPrice = data.ItemPrice + itemPrice;
                            }
                            else
                            {
                                orderData.Amount = orderData.Amount - data.ItemPrice + itemPrice;
                                data.ItemPrice = itemPrice;
                            }
                        }
                        else
                        {
                            orderData.Amount = orderData.Amount + itemPrice;
                        }


                        if (orderData.Amount <= 10000)
                        {
                            if (data != null && itemId > 0)
                            {

                                data.OrderID = orderId;
                                data.ItemID = itemId;
                                data.ItemPrice = data.ItemPrice;
                                data.UpdatedDate = DateTime.UtcNow;
                                data.UpdatedBy = userId;

                                // save OrderDetail 
                                _context.OrderDetail.Update(data);
                                _context.SaveChanges();

                                //save OrderMAster Amount
                                _context.OrderMaster.Update(orderData);
                                _context.SaveChanges();

                                response.data = orderId;
                                response.message = "Order Detail Update SuccessFully";
                                response.status = true;
                            }
                            else
                            {

                                master.OrderID = orderId;
                                master.ItemID = itemId;
                                master.ItemPrice = itemPrice;
                                master.CreatedDate = DateTime.UtcNow;
                                master.CreatedBy = userId;

                                //save OrderMAster Amount
                                _context.OrderMaster.Update(orderData);
                                _context.SaveChanges();

                                // save OrderDetail
                                _context.OrderDetail.Add(master);
                                _context.SaveChanges();

                                response.message = "Order Detail Add SuccessFully";
                                response.status = true;
                                response.data = orderId;
                            }
                        }
                        else
                        {
                            response.message = "The total Amount of all the items are maxixmum  10000.";
                            response.status = false;
                            response.data = orderId;
                        }
                    }
                    else
                    {
                        response.message = "Maximum Ten Line item Record will be add on this Order.";
                        response.status = false;
                        response.data = orderId;
                    }
                }

                return new JsonResponseModel
                {
                    status = response.status,
                    message = response.message,
                    data = response.data

                };
            }
            catch (Exception ex)
            {
                return new JsonResponseModel
                {
                    status = false,
                    message = ex.Message.ToString(),
                    data = 0
                };
            }

        }

        public async Task<JsonResponseModel> DeleteOrderDetailData(HttpContext httpContext, int id)
        {
            try
            {
                var response = new JsonResponseModel();
                if (id > 0)
                {
                    var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var data = _context.OrderDetail.Where(x => x.ID == id && x.CreatedBy == userId).FirstOrDefault();
                    var orderData = _context.OrderMaster.Where(x => x.ID == data.OrderID && x.CreatedBy == userId).FirstOrDefault();

                    if (data != null)
                    {
                        orderData.Amount = orderData.Amount - data.ItemPrice;

                        //Update OrderMaster Amount
                        _context.OrderMaster.Update(orderData);
                        _context.SaveChanges();

                        // Delete OrderDetail
                        _context.OrderDetail.Remove(data);
                        _context.SaveChanges();

                        response.status = true;
                        response.data = data.OrderID;
                        response.message = "data removed Succesfully";
                    }
                }
                return new JsonResponseModel
                {
                    status = response.status,
                    message = response.message,
                    data = response.data
                };
            }
            catch (Exception ex)
            {
                return new JsonResponseModel
                {
                    status = false,
                    message = ex.Message.ToString()
                };
            }
        }


        #endregion

        #region Item Details Methods

        public async Task<List<ItemMaster>> GetitemList()
        {
            try
            {
                var res = _context.ItemMaster.ToList();
                return res;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        public async Task<string> GetItemNameById(long? id)
        {
            try
            {
                var res = _context.ItemMaster.Where(x => x.ID == id).FirstOrDefault();
                return res.ItemNumber;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        public async Task<ItemMaster> GetItemDetailById(int id)
        {
            try
            {
                var res = _context.ItemMaster.Where(x => x.ID == id).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        public async Task<List<ItemMaster>> GetItemListById(HttpContext context, int id)
        {
            try
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var list = new List<ItemMaster>();
                var itemIdList = _context.OrderDetail.Where(x => x.OrderID == id && x.CreatedBy == userId).ToList();
                if (itemIdList.Count > 0)
                {
                    foreach (var item in itemIdList)
                    {
                        var listItems = _context.ItemMaster.Where(x => x.ID == item.ItemID).FirstOrDefault();
                        if (listItems != null)
                        {
                            list.Add(listItems);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        #endregion

        #region Other Methods Section

        public async Task<List<SelectListItem>> GetStatusTypeList()
        {
            try
            {
                var list = new List<SelectListItem>
                  {
                      new SelectListItem { Text = "DRAFT", Value = "1" },
                      new SelectListItem { Text = "SUBMITTED",    Value = "2" }
                   };
                return list;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        #endregion


    }
    public interface IPurchaseOrderService
    {
        Task<List<OrderMaster>> GetpurchaseOrderList(HttpContext context);
        Task<List<ItemMaster>> GetitemList();
        Task<List<OrderDetailViewModel>> GetOrderMasterList(HttpContext httpcontext, int id);
        Task<OrderMaster> GetOrderDetailById(HttpContext context, int id);
        Task<OrderDetailViewModel> GetOrderTotalDetailById(HttpContext context, int id);
        Task<ItemMaster> GetItemDetailById(int id);
        Task<List<ItemMaster>> GetItemListById(HttpContext context, int id);
        Task<JsonResponseModel> SubmitOrderMasterData(HttpContext context, int orderId, string orderNumber, int status);
        Task<List<SelectListItem>> GetStatusTypeList();
        Task<JsonResponseModel> DeleteOrderData(HttpContext context, int id);
        Task<string> GetItemNameById(long? id);
        Task<JsonResponseModel> SubmitItemDataByOrderId(HttpContext context, int orderId, int itemId, float itemPrice, int flag);
        Task<JsonResponseModel> DeleteOrderDetailData(HttpContext httpContext, int id);
    }
}
