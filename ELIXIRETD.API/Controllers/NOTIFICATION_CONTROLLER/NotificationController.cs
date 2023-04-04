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
            var OrderingApprovalNotif = await _unitOfWork.Orders.GetAllListForApprovalOfSchedule();
            var MoveorderlistNotif = await _unitOfWork.Orders.GetMoveOrdersForNotification();
            var TransactmoveorderNotif = await _unitOfWork.Orders.GetAllForTransactMoveOrderNotification();
            var ForApprovalListNotif = await _unitOfWork.Orders.GetForApprovalMoveOrdersNotification();
            var rejectlistNotif = await _unitOfWork.Orders.GetRejectMoveOrderNotification();


            var posummarycount = PoSummaryNotif.Count();
            //var cancelledcount = CancelledPONotif.Count();

            var orderingnotifcount = OrderingNotif.Count();
            var orderingapprovalcount = OrderingApprovalNotif.Count();
            var moveorderlistcount = MoveorderlistNotif.Count();
            var transactmoveordercount = TransactmoveorderNotif.Count();
            var forapprovalmoveordercount = ForApprovalListNotif.Count();
            var rejectlistcount = rejectlistNotif.Count();

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
                OrderingApproval = new
                {
                    orderingapprovalcount
                },
                MoveOrderlist = new
                {
                    moveorderlistcount
                },
                Transactmoveorder = new
                {
                    transactmoveordercount
                },
                ForApprovalMoveOrder = new
                {
                    forapprovalmoveordercount
                },
                Rejectlist = new
                {
                    rejectlistcount
                }

            };

            return Ok(countlist);

            
            
        }


    }
}
