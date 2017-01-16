using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class MSDS_CustomerMapping: EntityTypeConfiguration<MSDS_Customer>
    {
        public MSDS_CustomerMapping()
        {
            HasKey(x => x.ROW_ID);
            HasMany(x => x.MealRecords)
                .WithRequired(x => x.Employee)
                .HasForeignKey(x => x.EMPLOYEE_ID);
        }
    }
}
