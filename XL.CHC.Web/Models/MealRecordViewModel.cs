using System;
using System.Collections.Generic;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class MealRecordSearchViewModel
    {
        public IPagedList<HU_MEAL_RECORD> ViewList { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public DateTime? STime { get; set; }
        public DateTime? ETime { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_CARD { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
        public bool? Includ_coffee { get; set; }
        public string ActionType { get; set; }
        public bool? IS_Extra_Meal { get; set; }
        public string MEAL_CLASS { get; set; }
        public List<SelectListItem> MealClasses { get; set; }
    }

    public class MealAnalysisSearchViewModel
    {
        public IPagedList<MealAnalysis> ViewList { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public DateTime? STime { get; set; }
        public DateTime? ETime { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_CARD { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
        public bool? Includ_coffee { get; set; }
        public string ActionType { get; set; }
        public bool? AnalysisType { get; set; }
    }

    public class ComboSearchViewModel
    {
        public DateTime? STime { get; set; }
        public DateTime? ETime { get; set; }
        public string ActionType { get; set; }
        public IPagedList<Combo> ViewList { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}