using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                    .GeneralLedgerReport(command.DateFrom, command.DateTo);


                using (var workbook = new XLWorkbook())
                {

                    var worksheet = workbook.Worksheets.Add($"General Ledger Report");
                    worksheet.Protect();
                    var headers = new List<string>
                    {
                        "Sync Id",
                        "Mark",
                        "Mark 2",
                        "Asset/CIP#",
                        "Accounting Tag",
                        "Transaction Date",
                        "Supplier / Customer",
                        "Account Title Code",
                        "Account Title",
                        "Company Code",
                        "Company",
                        "Division Code",
                        "Division",
                        "Department Code",
                        "Department",
                        "Unit Code",
                        "Unit",
                        "Sub Unit Code",
                        "Sub Unit",
                        "Location Code",
                        "Location",
                        "PO#",
                        "Ref.No.",
                        "Item Code",
                        "Description",
                        "Quantity",
                        "unit",
                        "Unit Price",
                        "Line Amount",
                        "Voucher/GJNO.",
                        "Account Type",
                        "DR / CR",
                        "Asset Code",
                        "Asset",
                        "Service Provider Code",
                        "Service Provider",
                        "BOA",
                        "Allocation",
                        "Account Group",
                        "Account SubGroup",
                        "Financial Statement",
                        "Unit Responsible",
                        "Batch",
                        "Remarks",
                        "Payroll Period",
                        "Position",
                        "PayrollType1",
                        "Payroll Type 2",
                        "Additional Description for DEPR",
                        "Remaining BV for DEPR",
                        "Useful Life",
                        "Month",
                        "Year",
                        "Particulars",
                        "Month 2",
                        "Farm Type",
                        "Jean Remarks",
                        "From",
                        "Changed To",
                        "Reason",
                        "Checking Remarks",
                        "BOA 2",
                        "System",
                        "Books"



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

                        //row.Cell(1).Style.Protection.Locked =false;
                        row.Cell(1).Value = ledger[index - 1].SyncId;
                        row.Cell(4).Value = ledger[index - 1].Asset_Cip;
                        row.Cell(4).Style.Protection.SetLocked(false);
                        row.Cell(6).Value = ledger[index - 1].Transaction_Date;
                        row.Cell(6).Style.Protection.SetLocked(false);
                        row.Cell(7).Value = ledger[index - 1].Supplier;
                        row.Cell(7).Style.Protection.SetLocked(false);
                        row.Cell(8).Value = ledger[index - 1].Account_Title_Code;
                        row.Cell(8).Style.Protection.SetLocked(false);
                        row.Cell(9).Value = ledger[index - 1].Account_Title_Name;
                        row.Cell(9).Style.Protection.SetLocked(false);
                        row.Cell(10).Value = ledger[index - 1].Company_Code;
                        row.Cell(10).Style.Protection.SetLocked(false);
                        row.Cell(11).Value = ledger[index - 1].Company_Name;
                        row.Cell(11).Style.Protection.SetLocked(false);
                        row.Cell(14).Value = ledger[index - 1].Department_Code;
                        row.Cell(14).Style.Protection.SetLocked(false);
                        row.Cell(15).Value = ledger[index - 1].Department_Name;
                        row.Cell(15).Style.Protection.SetLocked(false);

                        row.Cell(20).Value = ledger[index - 1].Location_Code;
                        row.Cell(20).Style.Protection.SetLocked(false);
                        row.Cell(21).Value = ledger[index - 1].Location;
                        row.Cell(21).Style.Protection.SetLocked(false);
                        row.Cell(22).Value = ledger[index - 1].Po;
                        row.Cell(22).Style.Protection.SetLocked(false);
                        row.Cell(23).Value = ledger[index - 1].Reference_No;
                        row.Cell(23).Style.Protection.SetLocked(false);
                        row.Cell(24).Value = ledger[index - 1].Item_Code;
                        row.Cell(24).Style.Protection.SetLocked(false);
                        row.Cell(25).Value = ledger[index - 1].Description;
                        row.Cell(25).Style.Protection.SetLocked(false);
                        row.Cell(26).Value = ledger[index - 1].Quantity;
                        row.Cell(26).Style.Protection.SetLocked(false);
                        row.Cell(27).Value = ledger[index - 1].Uom;
                        row.Cell(27).Style.Protection.SetLocked(false);
                        row.Cell(28).Value = ledger[index - 1].Unit_Price;
                        row.Cell(28).Style.Protection.SetLocked(false);
                        row.Cell(29).Value = ledger[index - 1].Line_Amount;
                        row.Cell(29).Style.Protection.SetLocked(false);
                        row.Cell(32).Value = ledger[index - 1].DR_CR;
                        row.Cell(32).Style.Protection.SetLocked(false);
                        row.Cell(34).Value = ledger[index - 1].Asset;
                        row.Cell(34).Style.Protection.SetLocked(false);
                        row.Cell(35).Value = ledger[index - 1].Service_Provider_Code;
                        row.Cell(35).Style.Protection.SetLocked(false);
                        row.Cell(36).Value = ledger[index - 1].Service_Provider;
                        row.Cell(36).Style.Protection.SetLocked(false);
                        row.Cell(52).Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(ledger[index - 1].Transaction_Date.Date.Month);
                        row.Cell(52).Style.Protection.SetLocked(false);
                        row.Cell(53).Value = ledger[index - 1].Transaction_Date.Date.Year.ToString();
                        row.Cell(53).Style.Protection.SetLocked(false);
                        row.Cell(63).Value = ledger[index - 1].System;
                        row.Cell(63).Style.Protection.SetLocked(false);

                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"GeneralLedgerReports {DateTime.Today.Date.ToString("MM/dd/yyyy")}.xlsx");

                }

                return Unit.Value;

            }
        }
    }
}
