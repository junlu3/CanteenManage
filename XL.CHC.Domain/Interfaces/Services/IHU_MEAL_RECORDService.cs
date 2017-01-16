using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IHU_MEAL_RECORDService
    {
        IList<HU_MEAL_RECORD> GetAll();
        void Delete(HU_MEAL_RECORD entity);
        HU_MEAL_RECORD Single(int id);
        IPagedList<HU_MEAL_RECORD> Search(MealRecordSearchModel searchModel);
        IList<HU_MEAL_RECORD> Download(MealRecordSearchModel searchModel);
        void ExportResult(string filePath, IList<HU_MEAL_RECORD> data);
        void ExportAnalysisResult(string filePath, string term, IList<MealAnalysis> personal, IList<MealAnalysis> depart);
        IPagedList<MealAnalysis> SearchMealAnalysis(MealRecordSearchModel searchModel);
        void ExportComboList(string filePath, List<Combo> data);
        IList<MealAnalysis> Download2(MealRecordSearchModel searchModel);
    }
}
