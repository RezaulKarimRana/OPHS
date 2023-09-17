using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Services.Budget.Contracts
{
    public interface IBudgetReportGenarator
    {
        Task ExcelReport(IList<AddBudgetEstimation> requestObjList, string fileName);
    }
}
