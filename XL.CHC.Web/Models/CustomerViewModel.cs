using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class CustomerSearchViewModel
    {
        public IPagedList<MSDS_Customer> ViewList { get; set; } 
        public string Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;

    }

    public class CustomerSingleViewModel
    {
        public int ID { get; set; }
        public Guid ROW_ID { get; set; }
        public string EMPLOYEE_CARD { get; set; }
        public string COMPANY_CODE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string EMPLOYEE_NAME_CN { get; set; }
        public string EMPLOYEE_NAME_EN { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string MGR_NAME { get; set; }
        public string LOCATION { get; set; }
        public string COMBO_CODE { get; set; }
        public bool IS_CHINESE_FOOD { get; set; }
        public bool IS_WEST_FOOD { get; set; }
        public bool IS_SPECIAL_FOOD { get; set; }
        public bool IS_COFFEE { get; set; }
        public string EMPLOYEE_PY { get; set; }
        public int CARD_STATUS { get; set; }
        
    }
}