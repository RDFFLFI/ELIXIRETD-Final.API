using ELIXIRETD.DATA.CORE.API_RESPONSE;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIRETD.DATA.Migrations;
using ELIXIRETD.DATA.SERVICES;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.API.Controllers.ORDERING_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderingController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly StoreContext _context;
        public OrderingController(IUnitOfWork unitOfWork, StoreContext context)
        {
            _unitofwork= unitOfWork;
            _context= context;
        }

        [HttpPost]
        [Route("AddNewOrders")]
        public async Task<IActionResult> AddNewOrders([FromBody] Ordering[] order)
        {
            if (ModelState.IsValid != true )
              return new JsonResult("Something went Wrong!") { StatusCode = 500 };
            {
                
                List<Ordering> DuplicateList = new List<Ordering>();
                List<Ordering> AvailableImport = new List<Ordering>();
                //List<Ordering> CustomerCodeNotExist = new List<Ordering>();
                List<Ordering> CustomerNameNotExist = new List<Ordering>();
                List<Ordering> UomNotExist = new List<Ordering>();
                List<Ordering> ItemCodesExist = new List<Ordering>();
                //List<Ordering> ItemDescriptionNotExist = new List<Ordering>();
                //List<Ordering> QuantityInValid = new List<Ordering>();
                List<Ordering> PreviousDateNeeded = new List<Ordering>();
                //List<Ordering> DepartmentCodeanNameNotExist = new List <Ordering>();
                //List<Ordering> CompanyCodeanNameNotExist = new List<Ordering>();
                //List<Ordering> LocationCodeanNameNotExist = new List<Ordering>();

                foreach (Ordering items in order)
                {

                    //if (items.QuantityOrdered <= 0)
                    //{
                    //    QuantityInValid.Add(items);
                    //

                    if (order.Count(x => x.TrasactId == items.TrasactId && x.ItemCode == items.ItemCode && x.Customercode == items.Customercode && x.CustomerType == items.CustomerType) > 1)
                    {
                        DuplicateList.Add(items);
                        continue;

                    }
                 
                        var validateOrderNoAndItemcode = await _unitofwork.Orders.ValidateExistOrderandItemCode(items.TrasactId, items.ItemCode , items.CustomerType , items.ItemdDescription , items.Customercode);
                        var validateDateNeeded = await _unitofwork.Orders.ValidateDateNeeded(items);
                        //var validateCustomerCode = await _unitofwork.Orders.ValidateCustomerCode(items.Customercode);
                        var validateCustomerName = await _unitofwork.Orders.ValidateCustomerName(items.Customercode , items.CustomerName , items.CustomerType);
                        var validateItemCode = await _unitofwork.Orders.ValidateItemCode(items.ItemCode , items.ItemdDescription, items.Uom);
                        //var validateItemDescription = await _unitofwork.Orders.ValidateItemDescription(items.ItemdDescription);
                        var validateUom = await _unitofwork.Orders.ValidateUom(items.Uom);
                        //var validateQuantity = await _unitofwork.Orders.ValidateQuantity(items.QuantityOrdered);
                        //var validateDepartmentCodeAndName = await _unitofwork.Orders.ValidateDepartment(items.Department);
                        //var validateCompanyCodeAndName = await _unitofwork.Orders.ValidateCompany(items.CompanyCode, items.CompanyName);
                        //var validateLocationCodeAndName = await _unitofwork.Orders.ValidateLocation(items.LocationCode, items.LocationName);

                        if (validateOrderNoAndItemcode == true)
                        {
                            DuplicateList.Add(items);
                        }
                        else if (validateDateNeeded == false)
                        {
                            PreviousDateNeeded.Add(items);
                        }

                        //else if (validateCustomerCode == false)
                        //{
                        //    CustomerCodeNotExist.Add(items);
                        //}

                        else if (validateCustomerName == false)
                        {
                            CustomerNameNotExist.Add(items);
                        }

                        else if (validateItemCode == false)
                        {
                            ItemCodesExist.Add(items);
                        }
                        //else if (validateItemDescription == false)
                        //{
                        //    ItemDescriptionNotExist.Add(items);
                        //}

                        else if (validateUom == false)
                        {
                            UomNotExist.Add(items);
                        }
                       
                        else
                        {

                        items.SyncDate = DateTime.Now;
                        AvailableImport.Add(items);
                        await _unitofwork.Orders.AddNewOrders(items);

                        }
  
                }

                var resultList = new
                {
                   AvailableImport,
                   DuplicateList,
                   ItemCodesExist,
                   UomNotExist,
                   CustomerNameNotExist,
                   PreviousDateNeeded,


                  
                };

                if ( DuplicateList.Count == 0&& CustomerNameNotExist.Count == 0  && ItemCodesExist.Count == 0  && UomNotExist.Count == 0 && PreviousDateNeeded.Count == 0)
                {
                    await _unitofwork.CompleteAsync();
                    return Ok("Successfully Add!");
                }
                else
                {
                    return BadRequest(resultList);
                }
            }
           
        }


        // ===================================== Prepared Schedule ============================================================

       

        [HttpPut]
        [Route("SchedulePreparedOrderedDate")]
        public async Task<IActionResult> SchedulePreparedOrderedDate([FromBody] Ordering[] orders)
        {
            if (orders == null || !orders.Any())
            {
                return BadRequest("Orders not provided.");
            }

            var generate = new GenerateOrderNo();
            generate.Rush = orders.Count(x => x.IsRush == true) > 0;


            generate.IsActive = true;

            if (!await _unitofwork.Orders.GenerateNumber(generate))
            {
                return BadRequest("Failed to generate order number.");
            }

            await _unitofwork.CompleteAsync();


            foreach (Ordering order in orders)
            {
          

                    if (!await _unitofwork.Orders.ValidatePrepareDate(order))
                    {
                        return BadRequest("Date needed must be in the future.");
                    }


                    order.OrderNoPKey = generate.Id;


                    if (!await _unitofwork.Orders.SchedulePreparedDate(order))
                    {
                        return BadRequest("Failed to schedule prepared date");
                    }

                
            }
        
                await _unitofwork.CompleteAsync();
                return new JsonResult("Orders scheduled successfully");

                          
        }
       
        [HttpPut]
        [Route("ReturnCancelledOrders")]
        public async Task<IActionResult> ReturnCancelledOrders([FromBody] Ordering orders)
        {
            var validate = await _unitofwork.Orders.ReturnCancelOrdersInList(orders);

            if (validate == false)
                return BadRequest("Orders is not exist");

            await _unitofwork.CompleteAsync();
            return Ok("Succesfully Return Cancel Orders");
        }

        [HttpGet]
        [Route("GetAllListOfCancelledOrders")]
        public async Task<IActionResult> GetAllListOfCancelledOrders()
        {
            var orders = await _unitofwork.Orders.GetAllListOfCancelOrders();
            return Ok(orders);
        }
        
 //============================================== Prepared Ordering ===============================================================================


        [HttpGet]
        [Route("DetailedListOfOrders")]
        public async Task<IActionResult> DetailedListofOrders([FromQuery]string customer)
        {
            var orders = await _unitofwork.Orders.DetailedListOfOrders(customer);
            return Ok(orders);
        }


        [HttpGet]
        [Route("OrderSummary")]
        public async Task<IActionResult> Ordersummary([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

         
            if (string.IsNullOrEmpty(DateFrom) || string.IsNullOrEmpty(DateTo))
            {
                return BadRequest("Date range is required");
            }

            var orderSummary = await _unitofwork.Orders.OrderSummary(DateFrom, DateTo);

            return Ok(orderSummary);

        }

     

        
        //=================================================================== MIR Ordering Preparation Schedule =======================================================

        [HttpGet]
        [Route("GetAllListofOrdersPagination")]
        public async Task<ActionResult<IEnumerable<GetAllListofOrdersPaginationDto>>> GetAlllistofOrdersPagination([FromQuery] UserParams userParams)
        {
            var orders = await _unitofwork.Orders.GetAllListofOrdersPagination(userParams );

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);
        }



        [HttpGet]
        [Route("GetAllListofOrdersPaginationOrig")]
        public async Task<ActionResult<IEnumerable<GetAllListofOrdersPaginationDto>>> GetAlllistofOrdersPaginationOrig([FromQuery] UserParams userParams , string search)
        {


            if (search == null)

                return await GetAlllistofOrdersPagination(userParams);

            var orders = await _unitofwork.Orders.GetAllListofOrdersPaginationOrig(userParams, search);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);
        }


        [HttpGet]
        [Route("GetAllListOfMirNoSearch")]
        public async Task<ActionResult<IEnumerable<GetAllListOfMirDto>>> GetAllListOfMirNoSearch( [FromQuery]UserParams userParams, [FromQuery] bool status )
        {

            var orders = await _unitofwork.Orders.GetAllListOfMirNoSearch(userParams, status );

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);
        }


        [HttpGet]
        [Route("ViewListOfMirOrdersViewListOfMirOrders")]
        public async Task<IActionResult> ViewListOfMirOrdersViewListOfMirOrders([FromQuery] int id)
        {

            var orders = await _unitofwork.Orders.ViewListOfMirOrders(id);

            return Ok(orders);

        }



        [HttpGet]
        [Route("GetAllListOfMir")]
        public async Task<ActionResult<IEnumerable<GetAllListOfMirDto>>> GetAllListOfMir([FromQuery] UserParams userParams, [FromQuery]  bool status , [FromQuery] string search)
        {
            if(search == null)
            
                return await GetAllListOfMirNoSearch(userParams,  status);

            var orders = await _unitofwork.Orders.GetAllListOfMir(userParams, status, search );

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);
        }


        [HttpGet("GetAllListOfMirOrders")]
        public async Task<IActionResult> GetAllListOfMirOrders(string customer)
        {
            var orders = await _unitofwork.Orders.GetAllListOfMirOrders(customer);
            return Ok(orders);
        }


        [HttpGet("GetAllListOfMirOrdersByMirIds")]
        public async Task<IActionResult> GetAllListOfMirOrdersByMirId([FromQuery] int[] listofMirIds)
        {
            var orders = await _unitofwork.Orders.GetAllListOfMirOrdersbyMirId(listofMirIds);
            return Ok(orders);
        }

        [HttpPut]
        [Route("PreparationOfSchedule")]
        public async Task<IActionResult> PreparationOfSchedule(Ordering[] orderspreparation)
        {


            foreach (Ordering items in orderspreparation)
            {
                
                if (!await _unitofwork.Orders.ValidatePrepareDate(items))
                {
                    return BadRequest("Date needed must be in the future.");
                }


                await _unitofwork.Orders.PreparationOfSchedule(items);
            }

            await _unitofwork.CompleteAsync();

            return Ok(orderspreparation);
            
        }

        [HttpPut]
        [Route("EditOrderQuantity")]
        public async Task<IActionResult> EditOrderQuantity([FromBody] Ordering order)
        {



            await _unitofwork.Orders.EditQuantityOrder(order);
            await _unitofwork.CompleteAsync();
            return new JsonResult("Successfully edit Order Quantity");
        }


        [HttpPut]
        [Route("CancelOrders")]
        public async Task<IActionResult> Cancelorders([FromBody] Ordering orders)
        {

            var existing = await _unitofwork.Orders.CancelOrders(orders);

            if (existing == false)
                return BadRequest("Order Id is not existing");


            await _unitofwork.CompleteAsync();
            return Ok("successfully cancel orders");
        }

        //=================================================================== MIR Ordering For Approval =======================================================

        [HttpGet]
        [Route("GetAllListForApprovalOfSchedule")]
        public async Task<IActionResult> GetAllListforApprovalOfSchedule(bool status)
        {
            var orders = await _unitofwork.Orders.GetAllListForApprovalOfSchedule(status);
            return Ok(orders);
        }

        [HttpGet]
        [Route("GetAllOrdersForScheduleApproval")]

        public async Task<IActionResult> GetallOrdersForScheduleApproval([FromQuery] int id)
        {
            var orders = await _unitofwork.Orders.GetAllOrdersForScheduleApproval(id);

            return Ok(orders);

        }

        [HttpPut]
        [Route("ApprovePreparedDate")]
        public async Task<IActionResult> ApprovedpreparedDate(Ordering[] orders)
        {
            
            foreach(Ordering items  in orders)
            {
                await _unitofwork.Orders.ApprovePreparedDate(items);
                await _unitofwork.CompleteAsync();
            }
          

            return new JsonResult("Successfully approved date!");
        }

        [HttpPut]
        [Route("RejectPreparedDate")]
        public async Task<IActionResult> Rejectdate(Ordering[] orders)
        {

            foreach (Ordering items in orders)
            {
                await _unitofwork.Orders.RejectPreparedDate(items);
                await _unitofwork.CompleteAsync();
            }
           

            return new JsonResult("Successfully reject prepared date!");
        }

        [HttpGet]
        [Route("GetAllApprovedOrdersForCalendar")]
        public async Task<IActionResult> GetallApprovedOrdersforCalendar(bool status)
        {
            var orders = await _unitofwork.Orders.GetAllApprovedOrdersForCalendar(status);
            return Ok(orders);
        }


        //=================================================================== MIR MoveOrder For Preparation =======================================================

        [HttpGet]
        [Route("GetAllListForMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<GetAllListForMoveOrderPaginationDto>>> GetAllListForMoveOrderPagination([FromQuery] UserParams userParams)
        {
            var orders = await _unitofwork.Orders.GetAllListForMoveOrderPagination(userParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);

        }

        [HttpGet]
        [Route("GetAllListForMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<GetAllListForMoveOrderPaginationDto>>> GetAllListForMoveOrderPaginationOrig([FromQuery] UserParams userParams, string search)
        {


            if (search == null)

                return await GetAllListForMoveOrderPagination(userParams/*, status*/);

            var orders = await _unitofwork.Orders.GetAllListForMoveOrderPaginatioOrig(userParams, search/* , status*/);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,

                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);
        }

        [HttpGet]
        [Route("GetAllListOfApprovedPreparedforMoveOrderNoSearch")]
        public async Task<ActionResult<IEnumerable<TotalListOfApprovedPreparedDateDto>>> GetAllListOfApprovedPreparedforMoveOrderNoSearch([FromQuery] UserParams userParams, [FromQuery] bool status)
        {

            var orders = await _unitofwork.Orders.TotalListOfApprovedPreparedDateNoSearch(userParams, status);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,

                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);
        }




        [HttpGet]
        [Route("GetAllListOfApprovedPreparedforMoveOrder")]
        public async Task<ActionResult<IEnumerable<TotalListOfApprovedPreparedDateDto>>> GetAllListOfApprovedPreparedforMoveOrder([FromQuery] UserParams userParams,[FromQuery] bool status , [FromQuery] string search)
        {

            if (search == null)

                return await GetAllListOfApprovedPreparedforMoveOrderNoSearch(userParams,status);

            var orders = await _unitofwork.Orders.TotalListOfApprovedPreparedDate(userParams, status, search);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,

                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return Ok(orderResult);
        }


        [HttpGet]
        [Route("GetAllListOfOrdersForMoveOrder")]
        public async Task<IActionResult> GetAllListOfOrdersForMoveOrder([FromQuery] int id)
        {
            var orders = await _unitofwork.Orders.ListOfOrdersForMoveOrder(id);
            return Ok(orders);
        }



        [HttpGet]
        [Route("GetAvailableStockFromWarehouse")]
        public async Task<IActionResult> GetAvailableStockFromWarehouse([FromQuery] int id, [FromQuery] string itemcode)
        {
            var orders = await _unitofwork.Orders.GetActualItemQuantityInWarehouse(id, itemcode);

            var getFirstrecieve = await _unitofwork.Orders.GetFirstNeeded(itemcode);

            var validate = _unitofwork.Orders.ValidateWarehouseId(id, itemcode);

            if (!await validate)
                return BadRequest("No id or itemcode existing");


            var resultList = new
            {
                orders,
                getFirstrecieve.warehouseId

            };

            return Ok(resultList);
        }


        [HttpGet]
        [Route("GetAllOutOfStockByItemCodeAndOrderDate")]
        public async Task<IActionResult> GetAllOutOfStockByItemCodeAndOrderDate([FromQuery] string itemcode, [FromQuery] string orderdate)
        {
            var orders = await _unitofwork.Orders.GetAllOutOfStockByItemCodeAndOrderDate(itemcode, orderdate);

            return Ok(orders);

        }


        [HttpPost]
        [Route("PrepareItemForMoveOrder")]
        public async Task<IActionResult> PrepareItemforMoveOrder([FromBody] MoveOrder order)
        {
            var details = await _unitofwork.Orders.GetMoveOrderDetailsForMoveOrder(order.OrderNoPkey);


            order.OrderNoPkey = details.Id;
            order.OrderNo = details.MIRId;
            order.OrderDate = Convert.ToDateTime(details.OrderDate);
            order.DateNeeded = Convert.ToDateTime(details.DateNeeded);
            order.PreparedDate = Convert.ToDateTime(details.PrepareDate);
            order.DepartmentName = details.Department;

            order.DepartmentCode = details.DepartmentCode;
            order.CompanyCode = details.CompanyCode;
            order.CompanyName = details.CompanyName;
            order.LocationCode = details.LocationCode;
            order.LocationName = details.LocationName;

            order.CustomerName = details.CustomerName;
            order.Customercode = details.CustomerCode;
            order.AddressOrder = details.Address;
            order.ItemCode = details.ItemCode;
            order.ItemDescription = details.ItemDescription;
            order.Uom = details.Uom;
            order.Category = details.Category;
            order.IsActive = true;
            order.IsPrepared = true;
            order.Rush = details.Rush;
            order.Remarks = details.Remarks;

            order.CustomerType = details.CustomerType;

            order.ItemRemarks = details.ItemRemarks;

            order.Cip_No = details.Cip_no;


            order.PreparedBy = User.Identity.Name;

            await _unitofwork.Orders.PrepareItemForMoveOrder(order);
            await _unitofwork.CompleteAsync();

            return Ok(order);
        }


        [HttpPut]
        [Route("CancelOrdersInMoveOrder")]
        public async Task<IActionResult> CancelOrdersInMoveOrder([FromBody] Ordering[] order)
        {
           foreach(Ordering items in order)
            {
                await _unitofwork.Orders.CancelControlInMoveOrder(items);

                await _unitofwork.CompleteAsync();
            }

            return Ok("Successfully cancel orders");

        }


        [HttpGet]
        [Route("ListOfPreparedItemsForMoveOrder")]
        public async Task<IActionResult> ListOfPreparedItemsForMoveOrder([FromQuery] int id)
        {

            var orders = await _unitofwork.Orders.ListOfPreparedItemsForMoveOrder(id);

            return Ok(orders);

        }


        [HttpPut]
        [Route("AddSavePreparedMoveOrder")]
        public async Task<IActionResult> AddSavePreparedMoveOrder([FromBody] MoveOrder[] orders)
        {

            foreach (MoveOrder items in orders)
            {
                if (!await _unitofwork.Orders.SavePreparedMoveOrder(items))
                    return BadRequest("No order no exist");

            }
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully added!");
        }


        [HttpPut]
        [Route("CancelPreparedItems")]
        public async Task<IActionResult> CancelPreparedItems([FromBody] MoveOrder moveorder)
        {
            var order = await _unitofwork.Orders.CancelMoveOrder(moveorder);

            if (order == false)
                return BadRequest("No existing Prepared Items");


            await _unitofwork.CompleteAsync();
            return new JsonResult("Successfully cancel prepared moverorder date!");


        }

        [HttpGet]
        [Route("ForApprovalMoveOrderPagination")]
        public async Task<IActionResult> ForApprovalMoveOrderPagination([FromQuery] bool status)
        {

            var orders = await _unitofwork.Orders.ForApprovalMoveOrderPagination(status);

            return Ok(orders);

        }

        [HttpGet]
        [Route("ForApprovalMoveOrderPaginationOrig")]
        public async Task<IActionResult> ForApprovalMoveOrderPaginationOrig([FromQuery] string search , [FromQuery] bool status )
        {

            if (search == null)
                return await ForApprovalMoveOrderPagination( status);

            var orders = await _unitofwork.Orders.ForApprovalMoveOrderPaginationOrig(search , status);

            return Ok(orders);

        }


        [HttpGet]
        [Route("ViewMoveOrderForApproval")]
        public async Task<IActionResult> ViewMoveOrderForApproval([FromQuery] int id)
        {
            var orders = await _unitofwork.Orders.ViewMoveOrderForApproval(id);
            return Ok(orders);
        }

        [HttpPut]
        [Route("ApproveListOfMoveOrder")]
        public async Task<IActionResult> ApprovalListofMoveOrder([FromBody] MoveOrder[] moveOrder)
        {
            foreach(MoveOrder items in moveOrder)
            {
                await _unitofwork.Orders.ApprovalForMoveOrders(items);
                await _unitofwork.CompleteAsync();
            }    

            return new JsonResult("Successfully Approved List for move order!");

        }


        [HttpPut]
        [Route("UpdatePrintStatus")]
        public async Task<IActionResult> UpdatePrintStatus([FromBody] MoveOrder[] moveorder)
        {

            foreach( MoveOrder items in moveorder)
            {
                await _unitofwork.Orders.UpdatePrintStatus(items);
                await _unitofwork.CompleteAsync();
            }
           

            return Ok(moveorder);
        }

        [HttpPut]
        [Route("RejectListOfMoveOrder")]
        public async Task<IActionResult> RejectListOfMoveOrder([FromBody] MoveOrder[] moveorder)
        {


            foreach(MoveOrder items in moveorder)
            {
                await _unitofwork.Orders.RejectForMoveOrder(items);
            }
      
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully reject list for move order!");
        }



        //=================================================================== MIR MoveOrder Approve =======================================================


        [HttpGet]
        [Route("ApprovedMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<ApprovedMoveOrderPaginationDto>>> ApprovedMoveOrderPagination([FromQuery] UserParams userParams , [FromQuery] bool status)
        {
            var moveorder = await _unitofwork.Orders.ApprovedMoveOrderPagination(userParams,status);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);

        }


        [HttpGet]
        [Route("ApprovedMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<ApprovedMoveOrderPaginationDto>>> ApprovedMoveOrderPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search , [FromQuery] bool status)
        {

            if (search == null)

                return await ApprovedMoveOrderPagination(userParams, status);

            var moveorder = await _unitofwork.Orders.ApprovedMoveOrderPaginationOrig(userParams, search, status);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("GetAllApprovedMoveOrder")]
        public async Task<IActionResult> GetAllApprovedMoveOrder([FromQuery] int id)
        {
            var orders = await _unitofwork.Orders.GetAllApprovedMoveOrder(id);


            return Ok(orders);
        }



        [HttpPut]
        [Route("RejectApproveListOfMoveOrder")]
        public async Task<IActionResult> RejectApproveListOfMoveOrder([FromBody] MoveOrder moveorder)
        {
            await _unitofwork.Orders.RejectApproveMoveOrder(moveorder);
            await _unitofwork.CompleteAsync();
            return new JsonResult("Successfully reject approved list for move order!");
        }



        [HttpGet]
        [Route("RejectedMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<RejectedMoveOrderPaginationDto>>> RejectedMoveOrderPagination([FromQuery] UserParams userParams , bool status)
        {
            var moveorder = await _unitofwork.Orders.RejectedMoveOrderPagination(userParams , status);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }



        [HttpGet]
        [Route("RejectedMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<RejectedMoveOrderPaginationDto>>> RejectedMoveOrderPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] bool status)
        {

            if (search == null)

                return await RejectedMoveOrderPagination(userParams, status);

            var moveorder = await _unitofwork.Orders.RejectedMoveOrderPaginationOrig(userParams, search, status);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }


        [HttpPut]
        [Route("ReturnMoveOrderForApproval")]
        public async Task<IActionResult> ReturnMoveOrderForApproval([FromBody] MoveOrder moveorder)
        {

            await _unitofwork.Orders.ReturnMoveOrderForApproval(moveorder);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully return list for move order!");
        }


        //=================================================================== MIR Transact MoveOrder =======================================================

        [HttpGet]
        [Route("GetTotalListForMoveOrder")]
        public async Task<IActionResult> GetTotalListForMoveOrder([FromQuery] bool status)
        {

            var orders = await _unitofwork.Orders.TotalListForTransactMoveOrder(status);

            return Ok(orders);

        }


        [HttpGet]
        [Route("ListOfMoveOrdersForTransact")]
        public async Task<IActionResult> ListOfMoveOrdersForTransact([FromQuery] int orderid)
        {

            var orders = await _unitofwork.Orders.ListOfMoveOrdersForTransact(orderid);

            return Ok(orders);

        }

        [HttpPost]
        [Route("TransactListOfMoveOrders")]
        public async Task<IActionResult> TransactListsOfMoveOrders([FromBody] TransactMoveOrder[] transact)
        {


            foreach (TransactMoveOrder items in transact)
            {
                items.IsTransact = true;
                items.IsActive = true;
                items.PreparedDate = DateTime.Now;
                items.PreparedBy = User.Identity.Name;

                if (!await _unitofwork.Orders.TransanctListOfMoveOrders(items))
                    return BadRequest("no order exist");

            }

            await _unitofwork.CompleteAsync();

            return Ok(transact);

        }

        [HttpGet]
        [Route("TrackingofOrderingTransaction")]
        public async Task<IActionResult> TrackingofOrderingTransaction()
        {
            var orders = await _unitofwork.Orders.TrackingofOrderingTransaction();

            return Ok(orders);
        }

        [HttpGet("ListofServedDto")]
        public async Task<IActionResult> ListofServedDto()
        {
            var order = await _unitofwork.Orders.ListofServedDto();

            return Ok(order);
        }

    }
}
