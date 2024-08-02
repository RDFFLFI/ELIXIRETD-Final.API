using ClosedXML.Excel;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.IMPORT_REPOSITORY
{
    public  class ExportMaterial
    {
        public class ExportMaterialCommand : IRequest<Unit>
        {
        }

        public class Handler : IRequestHandler<ExportMaterialCommand, Unit>
        {
            private readonly IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(ExportMaterialCommand command, CancellationToken cancellationToken)
            {
                var materialList = await _unitOfWork.Materials.GetAllActiveMaterials();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"MaterialList");

                    var headers = new List<string>
                    {
                        "ItemCode",
                        "UomCode"
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

                    for (var index = 1; index <= materialList.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);
                        row.Cell(1).Value = materialList[index - 1].ItemCode;
                        row.Cell(2).Value = materialList[index - 1].Uom;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"MaterialList.xlsx");

                    return Unit.Value;
                }
            }
        }

    }
}
