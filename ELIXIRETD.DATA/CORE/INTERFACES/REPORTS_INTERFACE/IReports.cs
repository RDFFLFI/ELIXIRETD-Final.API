﻿using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE
{
    public interface IReports
    {

        Task<IReadOnlyList<DtoWarehouseReceivingReports>> WarehouseReceivingReports(string DateFrom , string DateTo);

        Task<IReadOnlyList<DtoMoveOrderReports>> WarehouseMoveOrderReports(string DateFrom , string DateTo);

        Task<IReadOnlyList<DtoMiscReports>> MiscReports(string DateFrom, string DateTo);

        Task<IReadOnlyList<DtoMiscIssue>> MiscIssue (string DateFrom, string DateTo);

        Task<IReadOnlyList<DtoBorrowedAndReturned>> ReturnBorrowedReports (string DateFrom, string DateTo);

        Task<IReadOnlyList<DtoReturnedReports>> ReturnedReports(string DateFrom, string DateTo);

        Task<IReadOnlyList<DtoCancelledReports>> CancelledReports(string DateFrom , string DateTo);
        Task<IReadOnlyList<DtoInventoryMovement>>InventoryMovementReports (string DateFrom , string DateTo , string PlusOne);



    }
}