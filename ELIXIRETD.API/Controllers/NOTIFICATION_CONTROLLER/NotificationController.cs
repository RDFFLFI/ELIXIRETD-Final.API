using ELIXIRETD.DATA.CORE.ICONFIGURATION;
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

            var ForApprovalListNotif = await _unitOfWork.Orders.GetForApprovalMoveOrdersNotification();
            var ForApprovalListNotifNotRush = await _unitOfWork.Orders.GetForApprovalMoveOrdersNotificationNotRush();

            var rejectlistNotif = await _unitOfWork.Orders.GetRejectMoveOrderNotification();

            //borrowed
            var ForBorrowedApproval = await _unitOfWork.Borrowed.GetNotificationForBorrowedApproval();
            var ForReturnedApproval = await _unitOfWork.Borrowed.GetNotificationForReturnedApproval();
           

            var posummarycount = PoSummaryNotif.Count();
            //var cancelledcount = CancelledPONotif.Count();

            var orderingnotifcount = OrderingNotif.Count();
            var orderingnotifcountNotRush = OrderingNotifNotRush.Count();

            var orderingapprovalcount = OrderingApprovalNotif.Count();
            var orderingapprovalcountNotRush = OrderingApprovalNotifNotRush.Count();

            var moveorderlistcount = MoveorderlistNotif.Count();
            var moveorderlistcountNotRush = MoveorderlistNotifNotRush.Count();

            var transactmoveordercount = TransactmoveorderNotif.Count();

            var forapprovalmoveordercount = ForApprovalListNotif.Count();
            var forapprovalmoveordercountNotRush = ForApprovalListNotifNotRush.Count();

            var rejectlistcount = rejectlistNotif.Count();

            var forborrowedApprovalcount = ForBorrowedApproval.Count();
            var forreturnedApprovalcount = ForReturnedApproval.Count();

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
                Ordering = new
                {
                    orderingnotifcount
                },
                OrderingNotRush = new
                {
                    orderingnotifcountNotRush
                },

                OrderingApproval = new
                {
                    orderingapprovalcount
                },
                OrderingApprovalNotRush = new
                {
                    orderingapprovalcountNotRush
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
                ForApprovalMoveOrder = new
                {
                    forapprovalmoveordercount
                },
                ForApprovalMoveOrderNotRush = new
                {
                    forapprovalmoveordercountNotRush
                },
                Rejectlist = new
                {
                    rejectlistcount
                },
                BorrowedApproval = new
                {
                    forborrowedApprovalcount
                },
                ReturnedApproval = new
                {
                    forreturnedApprovalcount
                }

            };

            return Ok(countlist);

            
            
        }


    }
}
