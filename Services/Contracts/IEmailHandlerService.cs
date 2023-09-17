using System.Threading.Tasks;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.FundDisburse;
using AMS.Models.ServiceModels.FundRequisition;
using AMS.Repositories.UnitOfWork.Contracts;

namespace AMS.Services.Contracts
{
    public interface IEmailHandlerService
    {
        Task<int> SaveNewFundRequisitionEmail(FundRequisitionVM requestDto, IUnitOfWork uow, UserEntity loggedUser);
        Task<int> SaveFundRequisitionRejectEmailEmail(FundRequisitionVM requestDto, IUnitOfWork uow,UserEntity loggedUser);
        Task<int> SaveNewFundDisburseEmail(FundDisburseVM requestDto, IUnitOfWork uow,UserEntity loggedUser);
        Task<int> SaveNewFundReceiveRollbackEmail(FundDisburseVM requestDto, IUnitOfWork uow,UserEntity loggedUser);
        Task<int> SaveSettlementEmail(int settlementId, UserEntity loggedUser);
        Task<int> SaveMemoEmail(int memoId,UserEntity loggedUser);
    }
}
