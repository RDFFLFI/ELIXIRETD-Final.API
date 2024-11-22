using ClosedXML.Excel;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE;
using ELIXIRETD.DATA.SERVICES;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY.ConsolidateFinanceExport;
namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY
{
    public class ConsolidateFinanceExport
    {
        public class ConsolidateFinanceExportCommand : IRequest<Unit>
        {
            public string DateTo { get; set; }
            public string DateFrom { get; set; }

            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<ConsolidateFinanceExportCommand, Unit>
        {
            private readonly IUnitOfWork _report;

            public Handler(IUnitOfWork report)
            {
                _report = report;
            }

            public async Task<Unit> Handle(ConsolidateFinanceExportCommand command, CancellationToken cancellationToken)
            {

                var consolidate = await _report.Reports.ConsolidateFinanceReport(command.DateFrom, command.DateTo, command.Search);


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Consolidated Report");

                    var headers = new List<string>
                    {
                        "Id","Transaction Date","Item Code","Item Description","Uom","Category","Quantity","Unit Cost","Line Amount",
                        "Source","Transaction Type","Reason","Reference","Supplier Name","Encoded By","Company Code","Company Name","Department Code","Department Name",
                        "Location Code","Location Name","Account Title Code","Account Title","EmpId","FullName","Asset Tag","Cip #", "Helpdesk","Rush"

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


                    for (var index = 1; index <= consolidate.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = consolidate[index - 1].Id;
                        row.Cell(2).Value = consolidate[index - 1].TransactionDate;
                        row.Cell(3).Value = consolidate[index - 1].ItemCode;
                        row.Cell(4).Value = consolidate[index - 1].ItemDescription;
                        row.Cell(5).Value = consolidate[index - 1].Uom;
                        row.Cell(6).Value = consolidate[index - 1].Category;
                        if (consolidate[index - 1].TransactionType == "Move Order" || consolidate[index - 1].TransactionType == "Miscellaneous Issue" || consolidate[index - 1].TransactionType == "Borrow")
                        {
                            row.Cell(7).Value = "-" + consolidate[index - 1].Quantity;
                            row.Cell(9).Value = "-" + consolidate[index - 1].LineAmount;
                        }
                        else
                        {
                            row.Cell(7).Value = consolidate[index - 1].Quantity;
                            row.Cell(9).Value = consolidate[index - 1].LineAmount;
                        }
                        //row.Cell(7).Value = consolidate[index - 1].Quantity;
                        row.Cell(8).Value = consolidate[index - 1].UnitCost;
                        //row.Cell(9).Value = consolidate[index - 1].LineAmount;
                        row.Cell(10).Value = consolidate[index - 1].Source;
                        row.Cell(11).Value = consolidate[index - 1].TransactionType;
                        row.Cell(12).Value = consolidate[index - 1].Reason;
                        row.Cell(13).Value = consolidate[index - 1].Reference;
                        row.Cell(14).Value = consolidate[index - 1].SupplierName;
                        row.Cell(15).Value = consolidate[index - 1].EncodedBy;
                        row.Cell(16).Value = consolidate[index - 1].CompanyCode;
                        row.Cell(17).Value = consolidate[index - 1].CompanyName;
                        row.Cell(18).Value = consolidate[index - 1].DepartmentCode;
                        row.Cell(19).Value = consolidate[index - 1].DepartmentName;
                        row.Cell(20).Value = consolidate[index - 1].LocationCode;
                        row.Cell(21).Value = consolidate[index - 1].LocationName;
                        row.Cell(22).Value = consolidate[index - 1].AccountTitleCode;
                        row.Cell(23).Value = consolidate[index - 1].AccountTitle;
                        row.Cell(24).Value = consolidate[index - 1].EmpId;
                        row.Cell(25).Value = consolidate[index - 1].Fullname;
                        row.Cell(26).Value = consolidate[index - 1].AssetTag;
                        row.Cell(27).Value = consolidate[index - 1].CIPNo;
                        row.Cell(28).Value = consolidate[index - 1].Helpdesk;
                        row.Cell(29).Value = consolidate[index - 1].Rush;

                    }
                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"ConsolidatedReports {command.DateFrom} - {command.DateTo}.xlsx");
                }

                return Unit.Value;
            }

        }

    }
}




