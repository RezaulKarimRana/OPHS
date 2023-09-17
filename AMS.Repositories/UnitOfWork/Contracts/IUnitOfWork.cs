using System;
using AMS.Repositories.DatabaseRepos.ConfigurationRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DashboardRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DepartmentRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DistRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DivRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Contract;
using AMS.Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateApproverFeedbackRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateApproverRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateChangeHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateDetailsRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateLinkedRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateMemo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateTypeRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimationReference.Contracts;
using AMS.Repositories.DatabaseRepos.EstimationRepo.Contracts;
using AMS.Repositories.DatabaseRepos.FundDisburseRepo;
using AMS.Repositories.DatabaseRepos.FundRequisition;
using AMS.Repositories.DatabaseRepos.FundRequisitionConfigRepo;
using AMS.Repositories.DatabaseRepos.ItemCategoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ItemRepo.Contracts;
using AMS.Repositories.DatabaseRepos.MemoApproverRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ProcurementApproval.Contracts;
using AMS.Repositories.DatabaseRepos.ProjectRepo.Contracts;
using AMS.Repositories.DatabaseRepos.RolePriorityRepo.Contracts;
using AMS.Repositories.DatabaseRepos.SessionRepo.Contracts;
using AMS.Repositories.DatabaseRepos.SettlementItemRepo;
using AMS.Repositories.DatabaseRepos.SettlementRepo;
using AMS.Repositories.DatabaseRepos.SettlementDepartmentWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.SettlementParticularWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ThanaRepo.Contracts;
using AMS.Repositories.DatabaseRepos.UnitRepo.Contracts;
using AMS.Repositories.DatabaseRepos.UserRepo.Contracts;
using AMS.Repositories.DatabaseRepos.AdminSupportRepo.Contracts;

namespace AMS.Repositories.UnitOfWork.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IConfigurationRepo ConfigurationRepo { get; }

        ISessionRepo SessionRepo { get; }

        IUserRepo UserRepo { get; }

        IDashboardRepo DashboardRepo { get; }

        IParticularRepo ParticularRepo { get; }

        IItemCategoryRepo ItemCategoryRepo { get; }

        IItemRepo ItemRepo { get; }

        IProjectRepo ProjectRepo { get; }

        IUnitRepo UnitRepo { get; }

        IDivRepo DivRepo { get; }

        IDistRepo DistRepo { get; }

        IThanaRepo ThanaRepo { get; }

        IDepartmentRepo DepartmentRepo { get; }

        IEmailContentsRepo EmailRepo { get; }

        IEstimateTypeRepo EstimateTyeRepo { get; }

        IEstimationRepo EstimationRepo { get; }

        IEstimateLinkedRepo EstimateLinkedRepo { get; }

        IEstimateDetailsRepo EstimateDetailsRepo { get; }

        IEstimateApproverRepo EstimateApproverRepo { get; }
        IEstimateApproverFeedbackRepo EstimateApproverFeedbackRepo { get; }

        IRolePriorityRepo RolePriorityRepo { get; }

        IEstimationAttachmentRepo EstimationAttachment { get; }

        IEstimateChangeHistoryRepo EstimateChangeHistoryRepo { get; }

        IEstimateDetailsChangeHistoryRepo EstimateDetailsChangeHistoryRepo { get; }

        IEstimateApproverChangeHistoryRepo EstimateApproverChangeHistoryRepo { get; }

        IDepartmentWiseSummaryRepo DepartmentWiseSummaryRepo { get; }

        IDepartmentWiseSummaryHistoryRepo DepartmentWiseSummaryHistoryRepo { get; }

        IParticularWiseSummaryRepo ParticularWiseSummaryRepo { get; }

        IParticularWiseSummaryHistoryRepo ParticularWiseSummaryHistoryRepo { get; }

        IProcurementApprovalRepo ProcurementApprovalRepo { get; }
        IFundRequisitionRepo FundRequisitionRepo  { get; }
        IFundRequisitionConfigRepo FundRequisitionConfigRepo { get; }
        IFundDisburseRepo FundDisburseRepo { get; }

        ISettlementRepo SettlementRepo { get; }
        ISettlementItemRepo SettlementItemRepo { get; }

        //Memo
        IEstimateMemoEntityRepo EstimateMemoEntityRepo { get; }
        IEstimationReferenceRepo EstimationReferenceRepo { get; }
        ISettlementDepartmentWiseSummaryRepo SettlementDepartmentWiseSummaryRepo { get; }
        ISettlementParticularWiseSummaryRepo SettlementParticularWiseSummaryRepo { get; }
        IMemoApproverRepo MemoApproverRepo { get; }
        IEstimateMemoAttachmentsRepo EstimateMemoAttachmentsRepo { get; }
        IAdminSupportRepo AdminSupportRepo { get; }
        bool Commit();
    }
}
