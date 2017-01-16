using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IHU_MEAL_RECORDRepository
    {
        IList<HU_MEAL_RECORD> GetAll();
        void Delete(HU_MEAL_RECORD entity);
        HU_MEAL_RECORD Single(int id);
        IPagedList<HU_MEAL_RECORD> Search(MealRecordSearchModel searchModel);

        IList<HU_MEAL_RECORD> Download(MealRecordSearchModel searchModel);
        IPagedList<MealAnalysis> SearchMealAnalysis(MealRecordSearchModel searchModel);
        IList<MealAnalysis> Download2(MealRecordSearchModel searchModel);
    }
}
