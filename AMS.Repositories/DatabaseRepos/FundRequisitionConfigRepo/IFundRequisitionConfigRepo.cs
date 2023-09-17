using AMS.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.FundRequisitionConfigRepo
{
   public  interface IFundRequisitionConfigRepo
    {
        Task<FundRequisitionConfig> GetFundRequistionConfigByDepartmentId(int id);
    }
}
