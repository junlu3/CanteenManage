using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{

    public class HU_MEAL_RECORD
    {
        public int ID { get; set; }
        public string CREATED_PROGRAM { get; set; }
        public string GROUP_ID { get; set; }
        public string COMPANY_ID { get; set; }
        /// <summary>
        /// 员工Guid ID
        /// </summary>
        public Guid EMPLOYEE_ID { get; set; }
        public DateTime MEAL_DATETIME { get; set; }
        public string MEAL_TYPE_CODE { get; set; }
        public decimal? MEAL_PRICE { get; set; }
        /// <summary>
        /// 点餐数量
        /// </summary>
        public int QUANTITY { get; set; }
        public string MEAL_CLASS { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public decimal? MEAL_AMOUNT { get; set; }
        public string IMPORT_DATA_ID { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public int TAG { get; set; }
        public bool DELETE_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public string ROW_ID { get; set; }
        public string MEAL_TYPE_NAME { get; set; }
        public string MEAL_TYPE_ID { get; set; }
        public bool EXTRA_MEAL { get; set; }
        public virtual MSDS_Customer Employee { get; set; }
        
    }

    public class MealAnalysis
    {
        public int ID { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string COMBO_CODE { get; set; }
        public int QUANTITY { get; set; }
        public decimal? AMOUNT { get; set; }
        public string COMPANY_ID { get; set; }

    }

    public class MealRecordSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public DateTime? STime { get; set; }
        public DateTime? ETime { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_CARD { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
        public bool Includ_coffee { get; set; }
        public bool AnalysisType { get; set; }
        public bool IS_Extra_Meal { get; set; }
        public string MEAL_CLASS { get; set; }
        public Guid ID { get; set; }
    }

    public class Combo
    {
        public string COMBO_CODE { get; set; }
        public decimal extra { get; set; }
        public decimal contractor { get; set; }
        public decimal traveler { get; set; }
        public decimal depart { get; set; }
        public decimal total { get; set; }
    }
}
