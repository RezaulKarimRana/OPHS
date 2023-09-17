using System.Threading.Tasks;
using AMS.Models.ServiceModels.FundDisburse;
using AMS.Models.ServiceModels.FundRequisition;

namespace AMS.Services.Contracts
{
    public interface IHtmlGeneratorService
    {
        Task<string> getSettlementHtmlBodyWithEstimationForPdfReport(int settlementId);
        string getRejectFundRequisitionEmailBody(FundRequisitionVM fundRequisitionVm);
        string getNewFundRequisitionEmailBody(FundRequisitionVM fundRequisitionVm);
        string getNewFundDisburseEmailBody(FundDisburseVM fundDisburseVm);
        string getNewFundReceivedOrRollbackEmailBody(FundDisburseVM fundDisburseVm);
        Task<string> getNewSettlementInitEmailBody(int settlementId, string message);
        Task<string> getMemoInitEmailBody(int memoId, string message);
        Task<string> getMemoHtmlBodyWithEstimationForPdfReport(int memoId);
        Task<string> getNewEstimateInitEmailBody(int estimateId, string message);
        Task<string> getPasswordResetEmailBody(string password);
        Task<string> getEstimateHtmlBodyWithEstimationForPdfReportV2(int estimationId);

    }
}
