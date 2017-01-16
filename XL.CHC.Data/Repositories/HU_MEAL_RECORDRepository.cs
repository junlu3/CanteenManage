using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Data.Repositories
{
    public class HU_MEAL_RECORDRepository : IHU_MEAL_RECORDRepository
    {
        private readonly CHCContext _context;

        public HU_MEAL_RECORDRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Delete(HU_MEAL_RECORD entity)
        {
            _context.HU_MEAL_RECORD.Remove(entity);
        }

        public IList<HU_MEAL_RECORD> GetAll()
        {
            return _context.HU_MEAL_RECORD.Select(x => x).ToList();
        }
        public HU_MEAL_RECORD Single(int id)
        {
            return _context.HU_MEAL_RECORD.SingleOrDefault(x => x.ID == id);
        }

        public IPagedList<HU_MEAL_RECORD> Search(MealRecordSearchModel searchModel)
        {

            var query = _context.HU_MEAL_RECORD.Where(x => (searchModel.STime == null || x.MEAL_DATETIME >= searchModel.STime.Value)
                        && (searchModel.ETime == null || x.MEAL_DATETIME <= searchModel.ETime.Value)
                        && (string.IsNullOrEmpty(searchModel.EMPLOYEE_CARD) || x.Employee.EMPLOYEE_CARD.Contains(searchModel.EMPLOYEE_CARD))
                        && (string.IsNullOrEmpty(searchModel.EMPLOYEE_ID) || x.Employee.EMPLOYEE_ID.Contains(searchModel.EMPLOYEE_ID))
                        && (string.IsNullOrEmpty(searchModel.DEPARTMENT_NAME) || x.Employee.DEPARTMENT_NAME.Contains(searchModel.DEPARTMENT_NAME))
                        && (string.IsNullOrEmpty(searchModel.COMPANY_CODE) || x.Employee.COMPANY_CODE.Contains(searchModel.COMPANY_CODE))
                        && (string.IsNullOrEmpty(searchModel.EMPLOYEE_NAME) || x.Employee.EMPLOYEE_NAME.Contains(searchModel.EMPLOYEE_NAME))
                        && (string.IsNullOrEmpty(searchModel.MEAL_CLASS) || x.MEAL_CLASS == searchModel.MEAL_CLASS )
                        && x.EXTRA_MEAL == searchModel.IS_Extra_Meal
                        ).OrderByDescending(x => x.MEAL_DATETIME);

            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<HU_MEAL_RECORD>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }

        public IList<HU_MEAL_RECORD> Download(MealRecordSearchModel searchModel)
        {
            var query = _context.HU_MEAL_RECORD.Where(x => (searchModel.STime == null || x.MEAL_DATETIME >= searchModel.STime.Value)
            && (searchModel.ETime == null || x.MEAL_DATETIME <= searchModel.ETime.Value)
            && (string.IsNullOrEmpty(searchModel.EMPLOYEE_CARD) || x.Employee.EMPLOYEE_CARD.Contains(searchModel.EMPLOYEE_CARD))
            && (string.IsNullOrEmpty(searchModel.EMPLOYEE_ID) || x.Employee.EMPLOYEE_ID.Contains(searchModel.EMPLOYEE_ID))
            && (string.IsNullOrEmpty(searchModel.DEPARTMENT_NAME) || x.Employee.DEPARTMENT_NAME.Contains(searchModel.DEPARTMENT_NAME))
            && (string.IsNullOrEmpty(searchModel.COMPANY_CODE) || x.Employee.COMPANY_CODE.Contains(searchModel.COMPANY_CODE))
            && (string.IsNullOrEmpty(searchModel.EMPLOYEE_NAME) || x.Employee.EMPLOYEE_NAME.Contains(searchModel.EMPLOYEE_NAME))
            && (string.IsNullOrEmpty(searchModel.MEAL_CLASS) || x.MEAL_CLASS == searchModel.MEAL_CLASS)
            && x.EXTRA_MEAL == searchModel.IS_Extra_Meal
            ).OrderByDescending(x => x.MEAL_DATETIME);

            return query.ToList();
        }

        public IPagedList<MealAnalysis> SearchMealAnalysis(MealRecordSearchModel searchModel)
        {
            if (!searchModel.AnalysisType)
            {
                //部门统计
                var query = _context.HU_MEAL_RECORD.Where(x => (searchModel.STime == null || x.MEAL_DATETIME >= searchModel.STime.Value)
                && (searchModel.ETime == null || x.MEAL_DATETIME <= searchModel.ETime.Value)
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_CARD) || x.Employee.EMPLOYEE_CARD.Contains(searchModel.EMPLOYEE_CARD))
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_ID) || x.Employee.EMPLOYEE_ID.Contains(searchModel.EMPLOYEE_ID))
                && (string.IsNullOrEmpty(searchModel.DEPARTMENT_NAME) || x.Employee.DEPARTMENT_NAME.Contains(searchModel.DEPARTMENT_NAME))
                && (string.IsNullOrEmpty(searchModel.COMPANY_CODE) || x.Employee.COMPANY_CODE.Contains(searchModel.COMPANY_CODE))
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_NAME) || x.Employee.EMPLOYEE_NAME.Contains(searchModel.EMPLOYEE_NAME))
                && (searchModel.Includ_coffee ? (x.MEAL_CLASS == "Coffee") : (x.MEAL_CLASS != "Coffee"))
                && (x.Employee.EMPLOYEE_ID.Contains("999999"))
                ).GroupBy(x => x.Employee.EMPLOYEE_ID, (i, o) => new { EMPLOYEE_ID = i, list = o });

                var count = query.Count();
                List<MealAnalysis> meal = new List<MealAnalysis>();
                var k = 1;
                query.ToList().ForEach(x => {
                    var obj = x.list.First();
                    var obj_emp = obj.Employee;
                    meal.Add(new MealAnalysis
                    {
                        ID = k,
                        EMPLOYEE_ID = x.EMPLOYEE_ID,
                        COMBO_CODE = obj_emp.COMBO_CODE,
                        DEPARTMENT_NAME = obj_emp.DEPARTMENT_NAME,
                        EMPLOYEE_NAME = obj_emp.EMPLOYEE_NAME,
                        QUANTITY = x.list.Sum(j => j.QUANTITY),
                        AMOUNT = x.list.Sum(j => j.MEAL_AMOUNT),
                        COMPANY_ID = obj.COMPANY_ID
                    });
                    k++;
                }
                );

                var newquery = meal.OrderByDescending(x => x.AMOUNT);
                var result = newquery.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
                return new PagedList<MealAnalysis>(result, searchModel.PageIndex, searchModel.PageSize, count);
            }
            else
            {
                //个人统计
                var query = _context.HU_MEAL_RECORD.Where(x => (searchModel.STime == null || x.MEAL_DATETIME >= searchModel.STime.Value)
                && (searchModel.ETime == null || x.MEAL_DATETIME <= searchModel.ETime.Value)
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_CARD) || x.Employee.EMPLOYEE_CARD.Contains(searchModel.EMPLOYEE_CARD))
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_ID) || x.Employee.EMPLOYEE_ID.Contains(searchModel.EMPLOYEE_ID))
                && (string.IsNullOrEmpty(searchModel.DEPARTMENT_NAME) || x.Employee.DEPARTMENT_NAME.Contains(searchModel.DEPARTMENT_NAME))
                && (string.IsNullOrEmpty(searchModel.COMPANY_CODE) || x.Employee.COMPANY_CODE.Contains(searchModel.COMPANY_CODE))
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_NAME) || x.Employee.EMPLOYEE_NAME.Contains(searchModel.EMPLOYEE_NAME))
                && (searchModel.Includ_coffee ? (x.MEAL_CLASS == "Coffee") : (x.MEAL_CLASS != "Coffee"))
                && !x.Employee.EMPLOYEE_ID.Contains("999999")
                ).GroupBy(x => x.Employee.EMPLOYEE_ID, (i, o) => new { EMPLOYEE_ID = i, list = o });

                var count = query.Count();
                List<MealAnalysis> meal = new List<MealAnalysis>();
                var k = 1;
                query.ToList().ForEach(x => {
                    var obj = x.list.First();
                    var obj_emp = obj.Employee;
                    meal.Add(new MealAnalysis
                    {
                        ID = k,
                        EMPLOYEE_ID = x.EMPLOYEE_ID,
                        COMBO_CODE = obj_emp.COMBO_CODE,
                        DEPARTMENT_NAME = obj_emp.DEPARTMENT_NAME,
                        EMPLOYEE_NAME = obj_emp.EMPLOYEE_NAME,
                        QUANTITY = x.list.Sum(j => j.QUANTITY),
                        AMOUNT = x.list.Sum(j => j.MEAL_AMOUNT),
                        COMPANY_ID = obj.COMPANY_ID
                    });
                    k++;
                }
                );

                var newquery = meal.OrderByDescending(x => x.AMOUNT);
                var result = newquery.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
                return new PagedList<MealAnalysis>(result, searchModel.PageIndex, searchModel.PageSize, count);
            }
        }

        public IList<MealAnalysis> Download2(MealRecordSearchModel searchModel)
        {
            if (!searchModel.AnalysisType)
            {
                //部门统计
                var query = _context.HU_MEAL_RECORD.Where(x => (searchModel.STime == null || x.MEAL_DATETIME >= searchModel.STime.Value)
                && (searchModel.ETime == null || x.MEAL_DATETIME <= searchModel.ETime.Value)
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_CARD) || x.Employee.EMPLOYEE_CARD.Contains(searchModel.EMPLOYEE_CARD))
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_ID) || x.Employee.EMPLOYEE_ID.Contains(searchModel.EMPLOYEE_ID))
                && (string.IsNullOrEmpty(searchModel.DEPARTMENT_NAME) || x.Employee.DEPARTMENT_NAME.Contains(searchModel.DEPARTMENT_NAME))
                && (string.IsNullOrEmpty(searchModel.COMPANY_CODE) || x.Employee.COMPANY_CODE.Contains(searchModel.COMPANY_CODE))
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_NAME) || x.Employee.EMPLOYEE_NAME.Contains(searchModel.EMPLOYEE_NAME))
                && (searchModel.Includ_coffee ? (x.MEAL_CLASS == "Coffee") : (x.MEAL_CLASS != "Coffee"))
                && (x.Employee.EMPLOYEE_ID.Contains("999999"))
                ).GroupBy(x => x.Employee.EMPLOYEE_ID, (i, o) => new { EMPLOYEE_ID = i, list = o });

                var count = query.Count();
                List<MealAnalysis> meal = new List<MealAnalysis>();
                var k = 1;
                query.ToList().ForEach(x => {
                    var obj = x.list.First();
                    var obj_emp = obj.Employee;
                    meal.Add(new MealAnalysis
                    {
                        ID = k,
                        EMPLOYEE_ID = x.EMPLOYEE_ID,
                        COMBO_CODE = obj_emp.COMBO_CODE,
                        DEPARTMENT_NAME = obj_emp.DEPARTMENT_NAME,
                        EMPLOYEE_NAME = obj_emp.EMPLOYEE_NAME,
                        QUANTITY = x.list.Sum(j => j.QUANTITY),
                        AMOUNT = x.list.Sum(j => j.MEAL_AMOUNT),
                        COMPANY_ID = obj.COMPANY_ID
                    });
                    k++;
                }
                );

                var newquery = meal.OrderByDescending(x => x.AMOUNT).ToList();
                return newquery;
            }
            else
            {
                //个人统计
                var query = _context.HU_MEAL_RECORD.Where(x => (searchModel.STime == null || x.MEAL_DATETIME >= searchModel.STime.Value)
                && (searchModel.ETime == null || x.MEAL_DATETIME <= searchModel.ETime.Value)
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_CARD) || x.Employee.EMPLOYEE_CARD.Contains(searchModel.EMPLOYEE_CARD))
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_ID) || x.Employee.EMPLOYEE_ID.Contains(searchModel.EMPLOYEE_ID))
                && (string.IsNullOrEmpty(searchModel.DEPARTMENT_NAME) || x.Employee.DEPARTMENT_NAME.Contains(searchModel.DEPARTMENT_NAME))
                && (string.IsNullOrEmpty(searchModel.COMPANY_CODE) || x.Employee.COMPANY_CODE.Contains(searchModel.COMPANY_CODE))
                && (string.IsNullOrEmpty(searchModel.EMPLOYEE_NAME) || x.Employee.EMPLOYEE_NAME.Contains(searchModel.EMPLOYEE_NAME))
                && (searchModel.Includ_coffee ? (x.MEAL_CLASS == "Coffee") : (x.MEAL_CLASS != "Coffee"))
                && !x.Employee.EMPLOYEE_ID.Contains("999999")
                ).GroupBy(x => x.Employee.EMPLOYEE_ID, (i, o) => new { EMPLOYEE_ID = i, list = o });

                var count = query.Count();
                List<MealAnalysis> meal = new List<MealAnalysis>();
                var k = 1;
                query.ToList().ForEach(x => {
                    var obj = x.list.First();
                    var obj_emp = obj.Employee;
                    meal.Add(new MealAnalysis
                    {
                        ID = k,
                        EMPLOYEE_ID = x.EMPLOYEE_ID,
                        COMBO_CODE = obj_emp.COMBO_CODE,
                        DEPARTMENT_NAME = obj_emp.DEPARTMENT_NAME,
                        EMPLOYEE_NAME = obj_emp.EMPLOYEE_NAME,
                        QUANTITY = x.list.Sum(j => j.QUANTITY),
                        AMOUNT = x.list.Sum(j => j.MEAL_AMOUNT),
                        COMPANY_ID = obj.COMPANY_ID
                    });
                    k++;
                }
                );
                var newquery = meal.OrderByDescending(x => x.AMOUNT).ToList();
                return newquery;
            }
        }
    }
}
