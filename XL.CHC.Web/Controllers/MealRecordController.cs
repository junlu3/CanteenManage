using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;

namespace XL.CHC.Web.Controllers
{
    public class MealRecordController : BaseController
    {

        #region Fields
        private readonly IHU_MEAL_RECORDService _service;
        #endregion


        public MealRecordController(IHU_MEAL_RECORDService service)
        {
            _service = service;
        }

        // GET: MealRecord
        public ActionResult Index()
        {
            try
            {
                var model = new MealRecordSearchViewModel();
                SearchOrders(model);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult Index(MealRecordSearchViewModel searchModel)
        {
            try
            {
                switch (searchModel.ActionType)
                {
                    case "Download":
                        return Download(searchModel);
                    case "Search":
                    default:
                        SearchOrders(searchModel);
                        return View(searchModel);
                }
            }
            catch (Exception ex)
            {
                var model = new MealRecordSearchViewModel();
                SearchOrders(model);
                ErrorNotification(ex);
                return View(model);
            }
        }

        private void SearchOrders(MealRecordSearchViewModel model)
        {
            if (model.STime.HasValue)
            {
                string tempD = model.STime.Value.ToString("yyyy-MM-dd 00:00:00");
                model.STime = DateTime.Parse(tempD);
            }
            else
            {
                string tempD = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd 00:00:00");
                model.STime = DateTime.Parse(tempD);
            }
            if (model.ETime.HasValue)
            {
                string tempD = model.ETime.Value.ToString("yyyy-MM-dd 23:59:59");
                model.ETime = DateTime.Parse(tempD);
            }
            else
            {
                string tempD = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                model.ETime = DateTime.Parse(tempD);
            }
            MealRecordSearchModel searchModel = new MealRecordSearchModel
            {
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                STime = model.STime,
                ETime = model.ETime,
                EMPLOYEE_CARD = string.IsNullOrEmpty(model.EMPLOYEE_CARD) ? string.Empty : model.EMPLOYEE_CARD.Trim(),
                EMPLOYEE_ID = string.IsNullOrEmpty(model.EMPLOYEE_ID) ? string.Empty : model.EMPLOYEE_ID.Trim(),
                DEPARTMENT_NAME = string.IsNullOrEmpty(model.DEPARTMENT_NAME) ? string.Empty : model.DEPARTMENT_NAME.Trim(),
                COMPANY_CODE = string.IsNullOrEmpty(model.COMPANY_CODE) ? string.Empty: model.COMPANY_CODE.Trim(),
                EMPLOYEE_NAME = string.IsNullOrEmpty(model.EMPLOYEE_NAME) ? string.Empty : model.EMPLOYEE_NAME.Trim(),
                Includ_coffee = model.Includ_coffee.HasValue ? model.Includ_coffee.Value : false,
                IS_Extra_Meal = model.IS_Extra_Meal.HasValue ? model.IS_Extra_Meal.Value : false,
                MEAL_CLASS = model.MEAL_CLASS
                
            };

            model.ViewList = _service.Search(searchModel);

            List<SelectListItem> selList = new List<SelectListItem>();
            selList.Add(new SelectListItem { Text = "全部", Value = "", Selected = string.IsNullOrEmpty(model.MEAL_CLASS) });
            selList.Add(new SelectListItem { Text = "早餐", Value = "Breakfast", Selected = (!string.IsNullOrEmpty(model.MEAL_CLASS) && model.MEAL_CLASS.Equals("Breakfast",StringComparison.OrdinalIgnoreCase)) });
            selList.Add(new SelectListItem { Text = "午餐", Value = "Lunch", Selected = (!string.IsNullOrEmpty(model.MEAL_CLASS) && model.MEAL_CLASS.Equals("Lunch", StringComparison.OrdinalIgnoreCase)) });
            selList.Add(new SelectListItem { Text = "晚餐", Value = "Dinner", Selected = (!string.IsNullOrEmpty(model.MEAL_CLASS) && model.MEAL_CLASS.Equals("Dinner", StringComparison.OrdinalIgnoreCase)) });
            selList.Add(new SelectListItem { Text = "夜宵", Value = "Supper", Selected = (!string.IsNullOrEmpty(model.MEAL_CLASS) && model.MEAL_CLASS.Equals("Supper", StringComparison.OrdinalIgnoreCase)) });
            model.MealClasses = selList;

        }


        public ActionResult MealAnalysis()
        {
            try
            {
                var model = new MealAnalysisSearchViewModel();
                SearchOrdersMealAnalysis(model);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult MealAnalysis(MealAnalysisSearchViewModel searchModel)
        {
            try
            {
                switch (searchModel.ActionType)
                {
                    case "Download":
                        return Download2(searchModel);
                    case "Search":
                    default:
                        SearchOrdersMealAnalysis(searchModel);
                        return View(searchModel);
                }
            }
            catch (Exception ex)
            {
                var model = new MealAnalysisSearchViewModel();
                SearchOrdersMealAnalysis(model);
                ErrorNotification(ex);
                return View(model);
            }
        }

        private void SearchOrdersMealAnalysis(MealAnalysisSearchViewModel model)
        {
            if (model.STime.HasValue)
            {
                string tempD = model.STime.Value.ToString("yyyy-MM-dd 00:00:00");
                model.STime = DateTime.Parse(tempD);
            }
            else
            {
                string tempD = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd 00:00:00");
                model.STime = DateTime.Parse(tempD);
            }
            if (model.ETime.HasValue)
            {
                string tempD = model.ETime.Value.ToString("yyyy-MM-dd 23:59:59");
                model.ETime = DateTime.Parse(tempD);
            }
            else
            {
                string tempD = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                model.ETime = DateTime.Parse(tempD);
            }
            MealRecordSearchModel searchModel = new MealRecordSearchModel
            {
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                STime = model.STime,
                ETime = model.ETime,
                EMPLOYEE_CARD = string.IsNullOrEmpty(model.EMPLOYEE_CARD) ? string.Empty : model.EMPLOYEE_CARD.Trim(),
                EMPLOYEE_ID = string.IsNullOrEmpty(model.EMPLOYEE_ID) ? string.Empty : model.EMPLOYEE_ID.Trim(),
                DEPARTMENT_NAME = string.IsNullOrEmpty(model.DEPARTMENT_NAME) ? string.Empty : model.DEPARTMENT_NAME.Trim(),
                COMPANY_CODE = string.IsNullOrEmpty(model.COMPANY_CODE) ? string.Empty : model.COMPANY_CODE.Trim(),
                EMPLOYEE_NAME = string.IsNullOrEmpty(model.EMPLOYEE_NAME) ? string.Empty : model.EMPLOYEE_NAME.Trim(),
                Includ_coffee = true,
                AnalysisType = model.AnalysisType.HasValue? model.AnalysisType.Value : true
            };

            model.ViewList = _service.SearchMealAnalysis(searchModel);

        }

        public ActionResult Download(MealRecordSearchViewModel model)
        {
            if (model.STime.HasValue)
            {
                string tempD = model.STime.Value.ToString("yyyy-MM-dd 00:00:00");
                model.STime = DateTime.Parse(tempD);
            }
            else
            {
                string tempD = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd 00:00:00");
                model.STime = DateTime.Parse(tempD);
            }
            if (model.ETime.HasValue)
            {
                string tempD = model.ETime.Value.ToString("yyyy-MM-dd 23:59:59");
                model.ETime = DateTime.Parse(tempD);
            }
            else
            {
                string tempD = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                model.ETime = DateTime.Parse(tempD);
            }
            MealRecordSearchModel searchModel = new MealRecordSearchModel
            {
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                STime = model.STime,
                ETime = model.ETime,
                EMPLOYEE_CARD = string.IsNullOrEmpty(model.EMPLOYEE_CARD) ? string.Empty : model.EMPLOYEE_CARD.Trim(),
                EMPLOYEE_ID = string.IsNullOrEmpty(model.EMPLOYEE_ID) ? string.Empty : model.EMPLOYEE_ID.Trim(),
                DEPARTMENT_NAME = string.IsNullOrEmpty(model.DEPARTMENT_NAME) ? string.Empty : model.DEPARTMENT_NAME.Trim(),
                COMPANY_CODE = string.IsNullOrEmpty(model.COMPANY_CODE) ? string.Empty : model.COMPANY_CODE.Trim(),
                EMPLOYEE_NAME = string.IsNullOrEmpty(model.EMPLOYEE_NAME) ? string.Empty : model.EMPLOYEE_NAME.Trim(),
                Includ_coffee = model.Includ_coffee.HasValue ? model.Includ_coffee.Value : false,
                IS_Extra_Meal = model.IS_Extra_Meal.HasValue ? model.IS_Extra_Meal.Value : false,
                MEAL_CLASS = model.MEAL_CLASS
            };

            IList<HU_MEAL_RECORD> list = _service.Download(searchModel);

            try
            {
                string suffix = DateTime.Now.ToString("yyyyMMddhhmmss");
                var fileName = "(不含加班餐)用餐明细" + suffix + ".xlsx";
                if (model.IS_Extra_Meal.HasValue && model.IS_Extra_Meal.Value)
                {
                    fileName = "(加班餐)用餐明细" + suffix + ".xlsx";
                }
                
                var filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                System.IO.File.Copy(Server.MapPath("~/Content/Templates/用餐明细模板(导出).xlsx"), filePath);

                //_specificationCheckService.ExportSpecificationResult(filePath, model.ViewList.ToList());
                _service.ExportResult(filePath,list);
                return File(filePath, "text/xls", fileName);
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return RedirectToAction("Index", new { @model = model });
            }
            
        }

        public ActionResult Download2(MealAnalysisSearchViewModel model)
        {
            string startTime = "";
            string endTime = "";
            if (model.STime.HasValue)
            {
                string tempD = model.STime.Value.ToString("yyyy-MM-dd 00:00:00");
                startTime = model.STime.Value.ToString("yyyy-MM-dd");
                model.STime = DateTime.Parse(tempD);
            }
            else
            {
                string tempD = new DateTime(DateTime.Now.Year, DateTime.Now.Month,1).ToString("yyyy-MM-dd 00:00:00");
                startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
                model.STime = DateTime.Parse(tempD);
            }
            if (model.ETime.HasValue)
            {
                string tempD = model.ETime.Value.ToString("yyyy-MM-dd 23:59:59");
                endTime = model.ETime.Value.ToString("yyyy-MM-dd");
                model.ETime = DateTime.Parse(tempD);
            }
            else
            {
                string tempD = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                endTime = DateTime.Now.ToString("yyyy-MM-dd");
                model.ETime = DateTime.Parse(tempD);
            }
            MealRecordSearchModel searchModel1 = new MealRecordSearchModel
            {
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                STime = model.STime,
                ETime = model.ETime,
                EMPLOYEE_CARD = string.IsNullOrEmpty(model.EMPLOYEE_CARD) ? string.Empty : model.EMPLOYEE_CARD.Trim(),
                EMPLOYEE_ID = string.IsNullOrEmpty(model.EMPLOYEE_ID) ? string.Empty : model.EMPLOYEE_ID.Trim(),
                DEPARTMENT_NAME = string.IsNullOrEmpty(model.DEPARTMENT_NAME) ? string.Empty : model.DEPARTMENT_NAME.Trim(),
                COMPANY_CODE = string.IsNullOrEmpty(model.COMPANY_CODE) ? string.Empty : model.COMPANY_CODE.Trim(),
                EMPLOYEE_NAME = string.IsNullOrEmpty(model.EMPLOYEE_NAME) ? string.Empty : model.EMPLOYEE_NAME.Trim(),
                Includ_coffee = true,
                AnalysisType = true

            };

            MealRecordSearchModel searchModel2 = new MealRecordSearchModel
            {
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                STime = model.STime,
                ETime = model.ETime,
                EMPLOYEE_CARD = string.IsNullOrEmpty(model.EMPLOYEE_CARD) ? string.Empty : model.EMPLOYEE_CARD.Trim(),
                EMPLOYEE_ID = string.IsNullOrEmpty(model.EMPLOYEE_ID) ? string.Empty : model.EMPLOYEE_ID.Trim(),
                DEPARTMENT_NAME = string.IsNullOrEmpty(model.DEPARTMENT_NAME) ? string.Empty : model.DEPARTMENT_NAME.Trim(),
                COMPANY_CODE = string.IsNullOrEmpty(model.COMPANY_CODE) ? string.Empty : model.COMPANY_CODE.Trim(),
                EMPLOYEE_NAME = string.IsNullOrEmpty(model.EMPLOYEE_NAME) ? string.Empty : model.EMPLOYEE_NAME.Trim(),
                Includ_coffee = true,
                AnalysisType = false

            };

            IList<MealAnalysis> list1 = _service.Download2(searchModel1);

            IList<MealAnalysis> list2 = _service.Download2(searchModel2);
            try
            {
                string suffix = DateTime.Now.ToString("yyyyMMddhhmmss");
                var fileName = "咖啡清单" + suffix + ".xlsx";
                var filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                System.IO.File.Copy(Server.MapPath("~/Content/Templates/咖啡清单模板(导出).xlsx"), filePath);
                string term = startTime + "~" + endTime;
                _service.ExportAnalysisResult(filePath, term, list1, list2);
                return File(filePath, "text/xls", fileName);
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return RedirectToAction("Index", new { @model = model });
            }

        }


        public ActionResult ComboSearch()
        {
            try
            {
                var model = new ComboSearchViewModel();
                model.ViewList = GetComboListFromDB(model);
                return View(model);
            }
            catch (Exception ex)
            {
                var model = new ComboSearchViewModel();
                ErrorNotification(ex);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult ComboSearch(ComboSearchViewModel model)
        {
            try
            {
                switch (model.ActionType)
                {
                    case "Download":
                        return Download3(model);
                    case "Search":
                    default:
                        model.ViewList = GetComboListFromDB(model);
                        return View(model);
                }

            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View(model);
            }
        }

        public ActionResult Download3(ComboSearchViewModel model)
        {
            try
            {
                List<Combo> list =  GetComboListFromDB2(model);
 
                string suffix = DateTime.Now.ToString("yyyyMMddhhmmss");
                var fileName = "成本中心结算清单" + suffix + ".xlsx";

                var filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                System.IO.File.Copy(Server.MapPath("~/Content/Templates/成本中心结算清单.xlsx"), filePath);

                //_specificationCheckService.ExportSpecificationResult(filePath, model.ViewList.ToList());
                _service.ExportComboList(filePath, list);
                return File(filePath, "text/xls", fileName);
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return RedirectToAction("Index", new { @model = model });
            }
        }

        private IPagedList<Combo> GetComboListFromDB(ComboSearchViewModel model)
        {
            string startTime = "";
            string endTime = "";
            if (model.STime.HasValue)
            {
                string tempD = model.STime.Value.ToString("yyyy-MM-dd 00:00:00");
                startTime = model.STime.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                string tempD = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd 00:00:00");
                startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            }
            if (model.ETime.HasValue)
            {
                string tempD = model.ETime.Value.ToString("yyyy-MM-dd 23:59:59");
                endTime = model.ETime.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                string tempD = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                endTime = DateTime.Now.ToString("yyyy-MM-dd");
            }

            string connString = ConfigurationManager.ConnectionStrings["CHCContext"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText =string.Format( @"SELECT B.COMBO_CODE, SUM(CASE WHEN EXTRA_MEAL = 1 THEN A.MEAL_AMOUNT ELSE 0 END) AS extra,
SUM(CASE WHEN B.EMPLOYEE_ID = '999999' AND EXTRA_MEAL = 0 THEN A.MEAL_AMOUNT ELSE 0 END) AS contractor,
SUM(CASE WHEN B.EMPLOYEE_ID = '999999T' AND EXTRA_MEAL = 0  THEN A.MEAL_AMOUNT ELSE 0 END) AS traveler,
SUM(CASE WHEN B.EMPLOYEE_ID = '999999D'  AND EXTRA_MEAL = 0 THEN A.MEAL_AMOUNT ELSE 0 END) AS depart
 FROM HU_MEAL_RECORD A
INNER JOIN MSDS_Customer B ON A.EMPLOYEE_ID = B.ROW_ID
WHERE A.MEAL_DATETIME BETWEEN '{0}' AND '{1}'
GROUP BY B.COMBO_CODE", startTime, endTime);
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Combo> list = new List<Combo>();
                    

                    while (reader != null && reader.Read())
                    {
                        Combo combo = new Combo();
                        combo.COMBO_CODE = reader[0] is DBNull ? "": reader.GetString(0);
                        combo.extra = reader[1] is DBNull ? 0 : reader.GetDecimal(1);
                        combo.contractor = reader[2] is DBNull ? 0 : reader.GetDecimal(2);
                        combo.traveler = reader[3] is DBNull ? 0 : reader.GetDecimal(3);
                        combo.depart = reader[4] is DBNull ? 0 : reader.GetDecimal(4);
                        combo.total = combo.extra + combo.contractor + combo.depart;
                        list.Add(combo);
                    }
                    reader.Close();
                    int count = list.Count;
                    var temp = list.OrderByDescending(x => x.total);
                    var result = temp.Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return new PagedList<Combo>(result, model.PageIndex, model.PageSize, count);
                }
            }
        }


        private List<Combo> GetComboListFromDB2(ComboSearchViewModel model)
        {
            string startTime = "";
            string endTime = "";
            if (model.STime.HasValue)
            {
                string tempD = model.STime.Value.ToString("yyyy-MM-dd 00:00:00");
                startTime = model.STime.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                string tempD = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd 00:00:00");
                startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            }
            if (model.ETime.HasValue)
            {
                string tempD = model.ETime.Value.ToString("yyyy-MM-dd 23:59:59");
                endTime = model.ETime.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                string tempD = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                endTime = DateTime.Now.ToString("yyyy-MM-dd");
            }

            string connString = ConfigurationManager.ConnectionStrings["CHCContext"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = string.Format(@"SELECT B.COMBO_CODE, SUM(CASE WHEN EXTRA_MEAL = 1 THEN A.MEAL_AMOUNT ELSE 0 END) AS extra,
SUM(CASE WHEN B.EMPLOYEE_ID = '999999' AND EXTRA_MEAL = 0 THEN A.MEAL_AMOUNT ELSE 0 END) AS contractor,
SUM(CASE WHEN B.EMPLOYEE_ID = '999999T' AND EXTRA_MEAL = 0  THEN A.MEAL_AMOUNT ELSE 0 END) AS traveler,
SUM(CASE WHEN B.EMPLOYEE_ID = '999999D'  AND EXTRA_MEAL = 0 THEN A.MEAL_AMOUNT ELSE 0 END) AS depart
 FROM HU_MEAL_RECORD A
INNER JOIN MSDS_Customer B ON A.EMPLOYEE_ID = B.ROW_ID
WHERE A.MEAL_DATETIME BETWEEN '{0}' AND '{1}'
GROUP BY B.COMBO_CODE", startTime, endTime);
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Combo> list = new List<Combo>();


                    while (reader != null && reader.Read())
                    {
                        Combo combo = new Combo();
                        combo.COMBO_CODE = reader[0] is DBNull ? "" : reader.GetString(0);
                        combo.extra = reader[1] is DBNull ? 0 : reader.GetDecimal(1);
                        combo.contractor = reader[2] is DBNull ? 0 : reader.GetDecimal(2);
                        combo.traveler = reader[3] is DBNull ? 0 : reader.GetDecimal(3);
                        combo.depart = reader[4] is DBNull ? 0 : reader.GetDecimal(4);
                        combo.total = combo.extra + combo.contractor + combo.depart;
                        list.Add(combo);
                    }
                    reader.Close();

                    return list;
                }
            }
        }
    }
}