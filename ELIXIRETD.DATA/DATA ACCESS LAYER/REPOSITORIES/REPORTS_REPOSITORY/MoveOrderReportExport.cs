using ClosedXML.Excel;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY
{
    public class MoveOrderReportExport
    {

        public class MoveOrderReportExportCommand : UserParams, IRequest<Unit>
        {
            public string DateTo { get; set; }
            public string DateFrom { get; set; }
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<MoveOrderReportExportCommand, Unit>
        {
            private readonly IUnitOfWork _report;

            public Handler(IUnitOfWork report)
            {
                _report = report;
            }

            public async Task<Unit> Handle(MoveOrderReportExportCommand command, CancellationToken cancellationToken)
            {
                var moveOrder = await _report.Reports.MoveOrderReport(command.DateFrom, command.DateTo, command.Search);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"MoveOrder Report");

                    var headers = new List<string>
                    {
                        "MIRId"
                        ,"Customer Code"
                        ,"Customer Name"
                        //,"Barcode No"
                        ,"Item Code"
                        ,"Item Description"
                        ,"Uom"
                        ,"SOH"
                        ,"Ordered Quantity"
                        ,"Served Order"
                        ,"Unserved Order"
                        ,"Served Percentage"
                        ,"Unserved Remarks"
                        ,"Approved Date"
                        ,"Delivery Date"
                        ,"Status"
                        ,"Asset Tag"
                        ,"Cip #"
                        ,"Helpdesk"
                        ,"Item Remarks"
                        ,"Company Code"
                        ,"Company Name"
                        ,"Department Code"
                        ,"Department Name"
                        ,"Location Code"
                        ,"Location Name"
                        ,"Account Title Code"
                        ,"Account Title"
                        ,"EmpId"
                        ,"FullName"
                        //,"IsRush"

                    };

                    var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                    range.Style.Fill.BackgroundColor = XLColor.Azure;
                    range.Style.Font.Bold = true;
                    range.Style.Font.FontColor = XLColor.Black;
                    range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (var index = 1; index <= headers.Count; index++)
                    {
                        worksheet.Cell(1, index).Value = headers[index - 1];
                    }

                    for (var index = 1; index <= moveOrder.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = moveOrder[index - 1].MIRId;
                        row.Cell(2).Value = moveOrder[index - 1].CustomerCode;
                        row.Cell(3).Value = moveOrder[index - 1].CustomerName;
                        row.Cell(4).Value = moveOrder[index - 1].ItemCode;
                        row.Cell(5).Value = moveOrder[index - 1].ItemDescription;
                        row.Cell(6).Value = moveOrder[index - 1].Uom;
                        row.Cell(7).Value = moveOrder[index - 1].SOH;
                        row.Cell(8).Value = moveOrder[index - 1].OrderedQuantity;
                        row.Cell(9).Value = moveOrder[index - 1].ServedOrder;
                        row.Cell(10).Value = moveOrder[index - 1].UnservedOrder;
                        row.Cell(11).Value = moveOrder[index - 1].ServedPercentage;
                        row.Cell(12).Value = moveOrder[index - 1].Remarks;
                        row.Cell(13).Value = moveOrder[index - 1].ApprovedDate;
                        row.Cell(14).Value = moveOrder[index - 1].DeliveryDate;
                        row.Cell(15).Value = moveOrder[index - 1].Status;
                        row.Cell(16).Value = moveOrder[index - 1].AssetTag;
                        row.Cell(17).Value = moveOrder[index - 1].Cip_No;
                        row.Cell(18).Value = moveOrder[index - 1].HelpdeskNo;
                        row.Cell(19).Value = moveOrder[index - 1].ItemRemarks;
                        row.Cell(20).Value = moveOrder[index - 1].CompanyCode;
                        row.Cell(21).Value = moveOrder[index - 1].CompanyName;
                        row.Cell(22).Value = moveOrder[index - 1].DepartmentCode;
                        row.Cell(23).Value = moveOrder[index - 1].DepartmentName;
                        row.Cell(24).Value = moveOrder[index - 1].LocationCode;
                        row.Cell(25).Value = moveOrder[index - 1].LocationName;
                        row.Cell(26).Value = moveOrder[index - 1].AccountCode;
                        row.Cell(27).Value = moveOrder[index - 1].AccountTitles;
                        row.Cell(28).Value = moveOrder[index - 1].EmpId;
                        row.Cell(29).Value = moveOrder[index - 1].FullName;

                        row.Cell(11).Style.NumberFormat.Format = "0.00%";

                    }
                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"MoveOrderReports {command.DateFrom} - {command.DateTo}.xlsx");
                }

                return Unit.Value;
            }
        }
    }
}
