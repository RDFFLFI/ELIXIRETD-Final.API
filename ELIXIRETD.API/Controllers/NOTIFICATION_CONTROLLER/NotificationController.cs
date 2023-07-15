using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.Notification_Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.NOTIFICATION_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("GetNotification")]
        public async Task<IActionResult> GetNotification()
        {

            //Receiving
            var PoSummaryNotif = await _unitOfWork.Receives.PoSummaryForWarehouseNotif();

            //var CancelledPONotif = await _unitOfWork.Receives.CancelledPoSummaryNotif();

            //Ordering 

            var OrderingNotif = await _unitOfWork.Orders.GetOrdersForNotification();
            var OrderingNotifNotRush = await _unitOfWork.Orders.GetOrdersForNotificationNotRush();

            var OrderingApprovalNotif = await _unitOfWork.Orders.GetAllListForApprovalOfSchedule();
            var OrderingApprovalNotifNotRush = await _unitOfWork.Orders.GetAllListForApprovalOfScheduleNotRush();

            var MoveorderlistNotif = await _unitOfWork.Orders.GetMoveOrdersForNotification();
            var MoveorderlistNotifNotRush = await _unitOfWork.Orders.GetMoveOrdersForNotificationNotRush();

            var TransactmoveorderNotif = await _unitOfWork.Orders.GetAllForTransactMoveOrderNotification();

            var RejectMoveOrderNotif = await _unitOfWork.Orders.GetRejectMoveOrderNotification();

            var ForApprovalListNotif = await _unitOfWork.Orders.GetForApprovalMoveOrdersNotification();
            var ForApprovalListNotifNotRush = await _unitOfWork.Orders.GetForApprovalMoveOrdersNotificationNotRush();

            // All

            var OrderingNotifAll = await _unitOfWork.Orders.GetOrdersForNotificationAll();
            var OrderingApprovalNotifAll = await _unitOfWork.Orders.GetAllListForApprovalOfScheduleAll();
            var MoveorderlistNotifAll = await _unitOfWork.Orders.GetMoveOrdersForNotificationAll();
            var ForApprovalListNotifAll = await _unitOfWork.Orders.GetForApprovalMoveOrdersNotificationAll();

            //borrowed
            var ForBorrowedApproval = await _unitOfWork.Borrowed.GetNotificationForBorrowedApproval();
            var ForReturnedApproval = await _unitOfWork.Borrowed.GetNotificationForReturnedApproval();

            var ForGetAllBorrowedNoParameters = await _unitOfWork.Borrowed.GetNotificationAllBorrowedNoParameters();
            var rejectlistNotifs = await _unitOfWork.Borrowed.RejectBorrowedNotification();






            var posummarycount = PoSummaryNotif.Count();
            //var cancelledcount = CancelledPONotif.Count();

            var orderingnotifcount = OrderingNotif.Count();
            var orderingnotifcountNotRush = OrderingNotifNotRush.Count();

            var orderingapprovalcount = OrderingApprovalNotif.Count();
            var orderingapprovalcountNotRush = OrderingApprovalNotifNotRush.Count();

            var moveorderlistcount = MoveorderlistNotif.Count();
            var moveorderlistcountNotRush = MoveorderlistNotifNotRush.Count();

            var transactmoveordercount = TransactmoveorderNotif.Count();
            var rejectmoveordercount = RejectMoveOrderNotif.Count();

            var forapprovalmoveordercount = ForApprovalListNotif.Count();
            var forapprovalmoveordercountNotRush = ForApprovalListNotifNotRush.Count();

            // all

            var orderingnotifallcount = OrderingNotifAll.Count();
            var orderingapprovalallcount = OrderingApprovalNotifAll.Count();
            var moveorderlistallcount = MoveorderlistNotifAll.Count();
            var forapprovalmoveorderallcount = ForApprovalListNotifAll.Count();

            //Borrowed

            var rejectlistscount = rejectlistNotifs.Count();

            var forborrowedApprovalcount = ForBorrowedApproval.Count();
            var forreturnedApprovalcount = ForReturnedApproval.Count();

            var GetAllBorrowed = ForGetAllBorrowedNoParameters.Count();


            var countlist = new
            {

                PoSummary = new
                {
                    posummarycount
                },
                //CancelledPosummary = new
                //{
                //    cancelledcount
                //},

                OrderingAll = new
                {
                    orderingnotifallcount
                },
                Ordering = new
                {
                    orderingnotifcount
                },
                OrderingNotRush = new
                {
                    orderingnotifcountNotRush
                },
                OrderingApprovalAll = new
                {
                    orderingapprovalallcount
                },

                OrderingApproval = new
                {
                    orderingapprovalcount
                },
                OrderingApprovalNotRush = new
                {
                    orderingapprovalcountNotRush
                },
                MoveOrderlistAll = new
                {
                    moveorderlistallcount
                },
                MoveOrderlist = new
                {
                    moveorderlistcount
                },
                MoveOrderlistNotRush = new
                {
                    moveorderlistcountNotRush
                },
                Transactmoveorder = new
                {
                    transactmoveordercount
                },
                Rejectmoveorder = new
                {
                    rejectmoveordercount
                },
                ForApprovalMoveOrderAll = new
                {
                    forapprovalmoveorderallcount
                },

                ForApprovalMoveOrder = new
                {
                    forapprovalmoveordercount
                },
                ForApprovalMoveOrderNotRush = new
                {
                    forapprovalmoveordercountNotRush
                },
               
                BorrowedApproval = new
                {
                    forborrowedApprovalcount
                },
                ReturnedApproval = new
                {
                    forreturnedApprovalcount
                },
                Rejectlist = new
                {
                    rejectlistscount
                },
                GetAllBorrowed = new
                {
                    GetAllBorrowed
                }

            };

            return Ok(countlist);

            
            
        }




        [HttpGet]
        [Route("GetNotificationWithParameters")]
        public async Task<IActionResult> GetNotificationWithEmpId(int empid)
        {

            //Receiving
            var ApprovedBorrowedNotif = await _unitOfWork.Borrowed.GetNotificationBorrowedApprove(empid);
            var ApproveReturnedNotif = await _unitOfWork.Borrowed.GetNotificationReturnedApprove(empid);
            var RejectNotif = await _unitOfWork.Borrowed.RejectBorrowedNotificationWithParameter(empid);
            var AllBorrowed = await _unitOfWork.Borrowed.GetNotificationAllBorrowed(empid);


            var borrowedApprovecount = ApprovedBorrowedNotif.Count();
            var returnedApprovecount = ApproveReturnedNotif.Count();    
            var RejectNotifcount = RejectNotif.Count();
            var allBorrowed =  AllBorrowed.Count();


            var countlist = new
            {

                BorrowedApproved = new
                {
                    borrowedApprovecount
                },
                ReturnedApproved = new
                {
                   returnedApprovecount
                },
                RejectNotification = new
                {
                    RejectNotifcount
                },
                AllBorrowedCount = new
                {
                    allBorrowed
                }



            };

            return Ok(countlist);



        }





    }
}
