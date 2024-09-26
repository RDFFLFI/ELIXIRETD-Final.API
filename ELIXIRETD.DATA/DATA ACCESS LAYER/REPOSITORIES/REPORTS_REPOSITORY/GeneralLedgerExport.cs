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
                    .GeneralLedgerReport(command.DateFrom, command.DateTo);

                using (var workbook = new XLWorkbook())
                {

                    var worksheet = workbook.Worksheets.Add($"General Ledger Report");
                    var headers = new List<string>
                    {
                        "SyncId",
                        "Mark",
                        "Mark 2",
                        "Asset /CIP#",
                        "Accounting Tag#",
                        "Transaction Date",
                        "Supplier/Customer",
                        "Account Title Code",
                        "Account Title",
                        "Company Code",
                        "Company",
                        "Division Code",
                        "Division",
                        "Deparment Code",
                        "Department",
                        "Unit Code",
                        "Unit",
                        "Sub-Unit Code",
                        "Sub-Unit Name",
                        "Location Code",
                        "Location",
                        "PO#",
                        "Ref. No.",
                        "Item Code",
                        "Description",
                        "Category",
                        "Qty.",
                        "unit",
                        "Unit Price",
                        "Line Amount",
                        "Voucher No.",
                        "Voucher/GJNO.",
                        "Account Type",
                        "DR/CR",
                        "Asset Code",
                        "Asset",
                        "Service Provide Code",
                        "Service Provider",
                        "BOA",
                        "Allocation",
                        "Account Group",
                        "Account Sub Group",
                        "Financial Statement",
                        "Unit Responsible",
                        "Batch",
                        "Remarks",
                        "Payroll Period",
                        "Position",
                        "Payroll type 1",
                        "Payroll Type 2",
                        "Additional Description for DEPR",
                        "Remaining BV for DEPR",
                        "Useful life",
                        "Month",
                        "Year",
                        "Division",
                        "Particulars",
                        "Month 2",
                        "Farm Type",
                        "Jean Marks",
                        "From",
                        "Change To",
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

                        row.Cell(1).Value = ledger[index - 1].SyncId;
                        row.Cell(4).Value = ledger[index - 1].Asset_Cip;
                        row.Cell(6).Value = ledger[index - 1].Transaction_Date;
                        row.Cell(7).Value = ledger[index - 1].Supplier;
                        row.Cell(8).Value = ledger[index - 1].Account_Title_Code;
                        row.Cell(9).Value = ledger[index - 1].Account_Title_Name;
                        row.Cell(10).Value = ledger[index - 1].Company_Code;
                        row.Cell(11).Value = ledger[index - 1].Company_Name;
                        row.Cell(13).Value = ledger[index - 1].Department_Code;
                        row.Cell(14).Value = ledger[index - 1].Department_Name;
                        row.Cell(20).Value = ledger[index - 1].Location_Code;
                        row.Cell(21).Value = ledger[index - 1].Location;
                        row.Cell(22).Value = ledger[index - 1].Po;
                        row.Cell(23).Value = ledger[index - 1].Reference_No;
                        row.Cell(24).Value = ledger[index - 1].Item_Code;
                        row.Cell(25).Value = ledger[index - 1].Description;
                        row.Cell(26).Value = ledger[index - 1].Category;
                        row.Cell(27).Value = ledger[index - 1].Quantity;
                        row.Cell(28).Value = ledger[index - 1].Uom;
                        row.Cell(29).Value = ledger[index - 1].Unit_Price;
                        row.Cell(30).Value = ledger[index - 1].Line_Amount;
                        row.Cell(34).Value = ledger[index - 1].DR_CR;
                        row.Cell(36).Value = ledger[index - 1].Asset;
                        row.Cell(37).Value = ledger[index - 1].Service_Provider_Code;
                        row.Cell(38).Value = ledger[index - 1].Service_Provider;
                        row.Cell(54).Value = ledger[index - 1].Transaction_Date.Date.Month;
                        row.Cell(55).Value = ledger[index - 1].Transaction_Date.Date.Year;
                        row.Cell(66).Value = ledger[index - 1].System;

                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"GeneralLedgerReports {DateTime.Today.Date.ToString("MM/dd/yyyy")}.xlsx");

                }

                return Unit.Value;

            }
        }
    }
}
