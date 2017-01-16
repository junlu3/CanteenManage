using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Data.Repositories
{
    public class MSDS_CustomerRepository: IMSDS_CustomerRepository
    {
        private readonly CHCContext _context;

        public MSDS_CustomerRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(MSDS_Customer entity)
        {
            if (_context.MSDS_Customer.Any(x=> x.EMPLOYEE_CARD == entity.EMPLOYEE_CARD))
            {
                throw new Exception(string.Format("该工号已被注册"));
            }
            _context.MSDS_Customer.Add(entity);
        }

        public IList<MSDS_Customer> GetAll()
        {
            return _context.MSDS_Customer.Select(x => x).ToList();
        }

        public void Delete(MSDS_Customer entity)
        {
            _context.MSDS_Customer.Remove(entity);
        }

        public MSDS_Customer Single(Guid id)
        {
            return _context.MSDS_Customer.SingleOrDefault(x=>x.ROW_ID == id);
        }

        public IPagedList<MSDS_Customer> Search(CustomerSearchModel searchModel)
        {
            var query = _context.MSDS_Customer.Where(x => (string.IsNullOrEmpty(searchModel.KeyWord)
            || x.EMPLOYEE_CARD.ToLower().Contains(searchModel.KeyWord.ToLower())
            || x.EMPLOYEE_NAME_CN.ToLower().Contains(searchModel.KeyWord.ToLower())
            || x.EMPLOYEE_NAME.ToLower().Contains(searchModel.KeyWord.ToLower())
            || x.EMPLOYEE_ID.ToLower().Contains(searchModel.KeyWord.ToLower())
            || x.DEPARTMENT_NAME.ToLower().Contains(searchModel.KeyWord.ToLower())
            )).OrderByDescending(x => x.EMPLOYEE_CARD);

            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<MSDS_Customer>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }

        public MSDS_Customer Single(string EMPLOYEE_CARD)
        {
            return _context.MSDS_Customer.SingleOrDefault(x => x.EMPLOYEE_CARD == EMPLOYEE_CARD);
        }
    }
}
