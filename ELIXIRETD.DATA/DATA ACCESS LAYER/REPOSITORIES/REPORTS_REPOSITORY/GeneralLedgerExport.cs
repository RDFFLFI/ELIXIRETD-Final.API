using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY
{
    public class GeneralLedgerExport
    {
        public class GeneralLedgerExportCommand : IRequest<Unit>
        {
            public string DateTo { get; set; }
            public string DateFrom { get; set; }
            public string Search {  get; set; }      

        }

        public class Handler : IRequestHandler<GeneralLedgerExportCommand, Unit>
        {
            private readonly IUnitOfWork _report;

            public Handler(IUnitOfWork report)
            {
                _report = report;
            }

            public async Task<Unit> Handle(GeneralLedgerExportCommand command, CancellationToken cancellationToken)
            {
                var ledger = await _report.Reports
                    .ConsolidateFinanceReport(command.DateFrom, command.DateTo, command.Search);

                using (var workbook = new XLWorkbook())
                {

                    var worksheet = workbook.Worksheets.Add($"General Ledger Report");
                    var headers = new List<string>
                    {
                        "Mark",
                        "Mark 2",
                        "CIP#",
                        "Accounting Tag#",
                        "Transaction Date",
                        "Supplier/Creditor",
                        "Company Code",
                        "Company",
                        "Business Unit Code",
                        "Business Unit",
                        "Deparment Code",
                        "Department",
                        "Unit Code",
                        "Unit",
                        "Sub-Unit Code",
                        "Sub-Unit Name",
                        "Location Code",
                        "Location",
                        "Account Title Code",
                        "Account Title",
                        "Ref. No.",
                        "PO",
                        "Item Code",
                        "Description",
                        "Category",
                        "Qty.",
                        "Uom",
                        "Unit Price",
                        "Line Amount",
                        "Voucher No.",
                        "Transaction Number",
                        "Transaction Type",
                        "DR/CR",
                        "Asset Code",
                        "Asset",
                        "CIP",
                        "Helpdesk Number",
                        "Service Provider Code",
                        "Service Provider",
                        "BOA",
                        "Allocation",
                        "Batch",
                        "Reason",
                        "Remarks",
                        "Month",
                        "Year",
                        "Division"
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
                    for (var index = 1; index <= ledger.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = "";
                        row.Cell(2).Value = "";
                        row.Cell(3).Value = ledger[index - 1].CIPNo;
                        row.Cell(4).Value = "";
                        row.Cell(5).Value = ledger[index - 1].TransactionDate;
                        row.Cell(6).Value = ledger[index - 1].SupplierName;
                        row.Cell(7).Value = ledger[index - 1].CompanyCode;
                        row.Cell(8).Value = ledger[index - 1].CompanyName;
                        row.Cell(9).Value = "";
                        row.Cell(10).Value = "";
                        row.Cell(11).Value = ledger[index - 1].DepartmentCode;
                        row.Cell(12).Value = ledger[index - 1].DepartmentName;
                        row.Cell(13).Value = "";
                        row.Cell(14).Value = "";
                        row.Cell(15).Value = "";
                        row.Cell(16).Value = "";
                        row.Cell(17).Value = ledger[index - 1].LocationCode;
                        row.Cell(18).Value = ledger[index - 1].LocationName;
                        row.Cell(19).Value = ledger[index - 1].AccountTitleCode;
                        row.Cell(20).Value = ledger[index - 1].AccountTitle;
                        row.Cell(21).Value = ledger[index - 1].Reference;
                        row.Cell(22).Value = ledger[index - 1].Source;
                        row.Cell(23).Value = ledger[index - 1].ItemCode;
                        row.Cell(24).Value = ledger[index - 1].ItemDescription;
                        row.Cell(25).Value = ledger[index - 1].Category;
                        row.Cell(26).Value = ledger[index - 1].Quantity;
                        row.Cell(27).Value = ledger[index - 1].Uom;
                        row.Cell(28).Value = ledger[index - 1].UnitCost;
                        row.Cell(29).Value = ledger[index - 1].LineAmount;
                        row.Cell(30).Value = "";
                        row.Cell(31).Value = ledger[index - 1].Source;
                        row.Cell(32).Value = ledger[index - 1].TransactionType;
                        row.Cell(33).Value = "";
                        row.Cell(34).Value = "";
                        row.Cell(35).Value = ledger[index - 1].AssetTag;
                        row.Cell(36).Value = ledger[index - 1].CIPNo;
                        row.Cell(37).Value = ledger[index - 1].Helpdesk;
                        row.Cell(38).Value = "";
                        row.Cell(39).Value = "";
                        row.Cell(40).Value = "Elixir ETD";
                        row.Cell(41).Value = "";
                        row.Cell(42).Value = "";
                        row.Cell(43).Value = ledger[index - 1].Reason;
                        row.Cell(44).Value = "";
                        row.Cell(45).Value = ledger[index - 1].TransactionDate.Date.Month;
                        row.Cell(46).Value = ledger[index - 1].TransactionDate.Date.Year;
                        row.Cell(7).Value = "";


                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"GeneralLedgerReports {DateTime.Today.Date.ToString("MM/dd/yyyy")}.xlsx");

                }

                return Unit.Value;

            }
        }
    }
}
