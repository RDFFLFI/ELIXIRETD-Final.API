﻿using ELIXIRETD.DATA.CORE.API_RESPONSE;
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
using Microsoft.Extensions.Logging.Abstractions;

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
        public async Task<IActionResult> AddNewOrders([FromBody] Ordering[] order, CancellationToken cancellation)
        {
            if (ModelState.IsValid != true )
              return new JsonResult("Something went Wrong!") { StatusCode = 500 };
            {
                
                List<Ordering> DuplicateList = new List<Ordering>();
                List<Ordering> AvailableImport = new List<Ordering>();
                List<Ordering> CustomerNameNotExist = new List<Ordering>();
                List<Ordering> ItemCodesExist = new List<Ordering>();
                List<Ordering> PreviousDateNeeded = new List<Ordering>();
                List<Ordering> AccountCodeEmpty = new List<Ordering>();
                List<Ordering> AccountTitleEmpty = new List<Ordering>();

                foreach (Ordering items in order)
                {

                    if (order.Count(x => x.TrasactId == items.TrasactId && x.ItemCode == items.ItemCode && x.Customercode == items.Customercode && x.CustomerType == items.CustomerType) > 1)
                    {
                        DuplicateList.Add(items);
                        continue;

                    }
                 
                        var validateOrderNoAndItemcode = await _unitofwork.Orders.ValidateExistOrderandItemCode(items.TrasactId, items.ItemCode , items.CustomerType , items.ItemdDescription , items.Customercode);
                        var validateDateNeeded = await _unitofwork.Orders.ValidateDateNeeded(items);
                        var validateCustomerName = await _unitofwork.Orders.ValidateCustomerName(items.Customercode , items.CustomerName , items.CustomerType);
                        var validateItemCode = await _context.Materials
                        .Include(x => x.Uom)
                        .FirstOrDefaultAsync(x => x.ItemCode == items.ItemCode && x.IsActive);
                        

                    if (validateOrderNoAndItemcode == true)
                    {
                        DuplicateList.Add(items);
                    }
                    else if (validateDateNeeded == false)
                    {
                        PreviousDateNeeded.Add(items);
                    }

                    else if (validateCustomerName == false)
                    {
                        CustomerNameNotExist.Add(items);
                    }

                    else if (validateItemCode is null)
                    {
                        ItemCodesExist.Add(items);
                    }
                    else if (string.IsNullOrEmpty(items.AccountCode))
                    {
                        AccountCodeEmpty.Add(items);
                    }

                    else if (string.IsNullOrEmpty(items.AccountTitles))
                    {
                        AccountTitleEmpty.Add(items);
                    }

                    else
                    {
                        items.ItemdDescription = validateItemCode.ItemDescription;
                        items.Uom = validateItemCode.Uom.UomCode;
                        items.SyncDate = DateTime.Now;
                        AvailableImport.Add(items);
                        await _unitofwork.Orders.AddNewOrders(items, cancellation);

                    }
                }

                var resultList = new
                {
                   AvailableImport,
                   DuplicateList,
                   ItemCodesExist,
                   CustomerNameNotExist,
                   PreviousDateNeeded,
                   AccountCodeEmpty,
                   AccountTitleEmpty
                };

                if ( DuplicateList.Count == 0&& CustomerNameNotExist.Count == 0  && ItemCodesExist.Count == 0  && PreviousDateNeeded.Count == 0 
                    && AccountTitleEmpty.Count == 0 && AccountCodeEmpty.Count == 0)
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
        [Route("ViewListOfMirOrders")]
        public async Task<IActionResult> ViewListOfMirOrders([FromQuery] int id)
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

            order.Modified_By = User.Identity.Name;

            await _unitofwork.Orders.EditQuantityOrder(order);
            await _unitofwork.CompleteAsync();
            return new JsonResult("Successfully edit Order Quantity");
        }


        [HttpPut]
        [Route("CancelOrders")]
        public async Task<IActionResult> Cancelorders([FromBody] Ordering orders)
        {
           
            //orders.Modified_By = User.Identity.Name;
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


            if(orders == null)
            {
                return Ok("[]");
            }

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
            order.OrderNoGenus = details.OrderNoGenus;
            order.OrderDate = Convert.ToDateTime(details.OrderDate);
            order.DateNeeded = Convert.ToDateTime(details.DateNeeded);
            order.PreparedDate = Convert.ToDateTime(details.PrepareDate);
            order.DepartmentName = details.Department;
            order.DepartmentCode = details.DepartmentCode;
            order.CompanyCode = details.CompanyCode;
            order.CompanyName = details.CompanyName;
            order.LocationCode = details.LocationCode;
            order.LocationName = details.LocationName;
            order.AccountCode = details.AccountCode;
            order.AccountTitles = details.AccountTitles;
            order.EmpId = details.EmpId;
            order.FullName = details.FullName;

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

            order.AssetTag = details.AssetTag;

            order.HelpdeskNo = details.HelpDeskNo;

            order.Requestor = details.Requestor;
            order.Approver = details.Approver;
            order.DateApproved = Convert.ToDateTime(details.DateApproved);


            if(order.QuantityOrdered < 0 )
            {
                return BadRequest("Quantity order invalid!");
            }

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

            foreach (var items in orders)
            {
                if (!await _unitofwork.Orders.SavePreparedMoveOrder(items))
                    return BadRequest("No order no exist");

                items.PreparedBy =  User.Identity.Name;

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
        public async Task<IActionResult> GetTotalListForMoveOrder([FromQuery] bool status , [FromQuery] string search)
        {

            var orders = await _unitofwork.Orders.TotalListForTransactMoveOrder(status , search);

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


        [HttpGet("MoveOrderAssetTag")]
        public async Task<ActionResult<IEnumerable<DtoMoveOrderAssetTag>>> MoveOrderAssetTag([FromQuery] UserParams userParams)
        {
            var assetTag = await _unitofwork.Orders.MoveOrderAssetTag(userParams);

            Response.AddPaginationHeader(assetTag.CurrentPage, assetTag.PageSize, assetTag.TotalCount, assetTag.TotalPages, assetTag.HasNextPage, assetTag.HasPreviousPage);

            var assetTagResult = new
            {
                assetTag,
                assetTag.CurrentPage,
                assetTag.PageSize,
                assetTag.TotalCount,
                assetTag.TotalPages,
                assetTag.HasNextPage,
                assetTag.HasPreviousPage
            };

            return Ok(assetTagResult);

        }

    }
}
