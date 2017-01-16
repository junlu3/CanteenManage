using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class HU_MEAL_RECORDService : IHU_MEAL_RECORDService
    {
        public readonly IHU_MEAL_RECORDRepository _repository;
        private readonly IImportExportService _importExportService;

        public HU_MEAL_RECORDService(IHU_MEAL_RECORDRepository repository, IImportExportService importExportService)
        {
            _repository = repository;
            _importExportService = importExportService;
        }


        public void Delete(HU_MEAL_RECORD entity)
        {
            _repository.Delete(entity);
        }

        public IList<HU_MEAL_RECORD> Download(MealRecordSearchModel searchModel)
        {
            return _repository.Download(searchModel);
        }



        public IList<HU_MEAL_RECORD> GetAll()
        {
            return _repository.GetAll();
        }

        public IPagedList<HU_MEAL_RECORD> Search(MealRecordSearchModel searchModel)
        {
            return _repository.Search(searchModel);
        }

        public HU_MEAL_RECORD Single(int id)
        {
            return _repository.Single(id);
        }

        public IPagedList<MealAnalysis> SearchMealAnalysis(MealRecordSearchModel searchModel)
        {
            return _repository.SearchMealAnalysis(searchModel);
        }

        public void ExportResult(string filePath, IList<HU_MEAL_RECORD> data)
        {
            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";
            if (data.Count > 0)
            {
                int rowIndex = 3;
                foreach (var entity in data)
                {
                    #region 

                    int colIndex = 1;
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "员工名",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Employee.EMPLOYEE_NAME
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "员工号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Employee.EMPLOYEE_ID
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "部门",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Employee.DEPARTMENT_NAME ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "成本中心",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Employee.COMBO_CODE ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "用餐时间",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = (entity.MEAL_DATETIME != null? entity.MEAL_DATETIME.ToString("yyyy-MM-dd HH:mm:ss") : "")
                    });
                    string meal_class = string.Empty;
                    switch (entity.MEAL_CLASS)
                    {
                        case "Breakfast":
                            meal_class = "早餐";
                            break;
                        case "Lunch":
                            meal_class = "午餐";
                            break;
                        case "Dinner":
                            meal_class = "晚餐";
                            break;
                        case "Supper":
                            meal_class = "夜宵";
                            break;
                        case "Coffee":
                            meal_class = "";
                            break;
                    }
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "时段",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = meal_class
                    });

                    string meal_name = string.Empty;
                    switch (entity.MEAL_TYPE_CODE)
                    {
                        case "chinese food":
                            meal_name = "中餐";
                            break;
                        case "western food":
                            meal_name = "西餐";
                            break;
                        case "special food":
                            meal_name = "特色餐";
                            break;
                        case "supper":
                            meal_name = "夜宵";
                            break;
                        case "coffee":
                            meal_name = "咖啡";
                            break;
                        case "rice soup":
                            meal_name = "粥";
                            break;

                            
                    }
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "餐名",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = meal_name
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "单价",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.MEAL_PRICE.HasValue ? entity.MEAL_PRICE.ToString() : ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "数量",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.QUANTITY.ToString()
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "总价",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.MEAL_AMOUNT.HasValue ? entity.MEAL_AMOUNT.ToString() : ""
                    });
                    #endregion

                    rowIndex++;
                }
            }

            excelModel.Sheets.Add(excelSheet);

            _importExportService.ExportWithTemplate(filePath, excelModel);
        }

        public void ExportAnalysisResult(string filePath,string term,IList<MealAnalysis> personal, IList<MealAnalysis> depart)
        {
            var excelModel = new ExcelModel();

            //个人
            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "个人";
            if (personal.Count > 0)
            {
                int rowIndex = 2;
                foreach (var entity in personal)
                {
                    #region 

                    int colIndex = 1;
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "时间段",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = term
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "员工号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.EMPLOYEE_ID
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "名字",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.EMPLOYEE_NAME
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "部门",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.DEPARTMENT_NAME
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "成本中心",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.COMBO_CODE
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "咖啡消费杯数",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.QUANTITY.ToString()
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "咖啡消费金额",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.AMOUNT.ToString()
                    });
                    #endregion

                    rowIndex++;
                }
            }

            excelModel.Sheets.Add(excelSheet);


            //部门
            var excelSheet2 = new ExcelSheetModel();
            excelSheet2.Name = "部门";
            if (personal.Count > 0)
            {
                int rowIndex = 2;
                foreach (var entity in personal)
                {
                    #region 

                    int colIndex = 1;
                    excelSheet2.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "时间段",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = term
                    });
                    excelSheet2.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "公司号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.COMPANY_ID
                    });
                    excelSheet2.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "员工号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.EMPLOYEE_ID
                    });
                    excelSheet2.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "名字",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.EMPLOYEE_NAME
                    });
                    excelSheet2.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "部门",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.DEPARTMENT_NAME
                    });
                    excelSheet2.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "成本中心",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.COMBO_CODE
                    });
                    excelSheet2.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "咖啡消费杯数",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.QUANTITY.ToString()
                    });
                    excelSheet2.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "咖啡消费金额",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.AMOUNT.ToString()
                    });
                    #endregion

                    rowIndex++;
                }
            }

            excelModel.Sheets.Add(excelSheet2);

            _importExportService.ExportWithTemplate(filePath, excelModel);
        }

        public void ExportComboList(string filePath,List<Combo> data)
        {
            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";
            if (data.Count > 0)
            {
                int rowIndex = 3;
                foreach (var entity in data)
                {
                    #region 
                    decimal total = entity.extra + entity.contractor + entity.depart + entity.traveler;
                    int colIndex = 1;
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "成本中心",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.COMBO_CODE
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "加班餐费用",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.extra.ToString()
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "承包商费用",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.contractor.ToString()
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "出差员工费用",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.traveler.ToString()
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "部门卡费用",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.depart.ToString()
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "用餐申请表费用",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = "0"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "总计",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = total.ToString()
                    });
                    #endregion

                    rowIndex++;
                }
            }

            excelModel.Sheets.Add(excelSheet);

            _importExportService.ExportWithTemplate(filePath, excelModel);
        }

        public IList<MealAnalysis> Download2(MealRecordSearchModel searchModel)
        {
            return _repository.Download2(searchModel);
        }
    }
}
