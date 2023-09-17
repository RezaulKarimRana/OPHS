using System;
using System.Data;
using AMS.Infrastructure.Configuration.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Repositories.DatabaseRepos.ConfigurationRepo.Contracts;
using AMS.Repositories.DatabaseRepos.UserRepo.Contracts;
using AMS.Repositories.DatabaseRepos.SessionRepo.Contracts;
using AMS.Repositories.DatabaseRepos.UserRepo;
using AMS.Repositories.DatabaseRepos.ConfigurationRepo;
using AMS.Repositories.DatabaseRepos.SessionRepo;
using AMS.Repositories.DatabaseRepos.DashboardRepo;
using AMS.Repositories.DatabaseRepos.DashboardRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ItemCategoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ItemRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ProjectRepo.Contracts;
using AMS.Repositories.DatabaseRepos.UnitRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DivRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DistRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ThanaRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DepartmentRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateTypeRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimationRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateLinkedRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateDetailsRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateApproverRepo.Contracts;
using AMS.Repositories.DatabaseRepos.RolePriorityRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateChangeHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularRepo;
using AMS.Repositories.DatabaseRepos.ItemCategoryRepo;
using AMS.Repositories.DatabaseRepos.ItemRepo;
using AMS.Repositories.DatabaseRepos.ProjectRepo;
using AMS.Repositories.DatabaseRepos.UnitRepo;
using AMS.Repositories.DatabaseRepos.DivRepo;
using AMS.Repositories.DatabaseRepos.DistRepo;
using AMS.Repositories.DatabaseRepos.ThanaRepo;
using AMS.Repositories.DatabaseRepos.DepartmentRepo;
using AMS.Repositories.DatabaseRepos.EstimationRepo;
using AMS.Repositories.DatabaseRepos.EstimateTypeRepo;
using AMS.Repositories.DatabaseRepos.EstimateLinkedRepo;
using AMS.Repositories.DatabaseRepos.EstimateDetailsRepo;
using AMS.Repositories.DatabaseRepos.EstimateApproverRepo;
using AMS.Repositories.DatabaseRepos.RolePriorityRepo;
using AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo;
using AMS.Repositories.DatabaseRepos.EstimateChangeHistoryRepo;
using AMS.Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo;
using AMS.Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryRepo;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo;
using AMS.Repositories.DatabaseRepos.EstimateApproverFeedbackRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateApproverFeedbackRepo;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Contract;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo;
using AMS.Repositories.DatabaseRepos.FundDisburseRepo;
using AMS.Repositories.DatabaseRepos.ProcurementApproval.Contracts;
using AMS.Repositories.DatabaseRepos.ProcurementApproval;
using AMS.Repositories.DatabaseRepos.FundRequisition;
using AMS.Repositories.DatabaseRepos.FundRequisitionConfigRepo;
using AMS.Repositories.DatabaseRepos.EstimateMemo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateMemo;
using AMS.Repositories.DatabaseRepos.EstimationReference.Contracts;
using AMS.Repositories.DatabaseRepos.EstimationReference;
using AMS.Repositories.DatabaseRepos.SettlementDepartmentWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.SettlementDepartmentWiseSummaryRepo;
using AMS.Repositories.DatabaseRepos.SettlementParticularWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.SettlementParticularWiseSummaryRepo;
using AMS.Repositories.DatabaseRepos.MemoApproverRepo.Contracts;
using AMS.Repositories.DatabaseRepos.MemoApproverRepo;
using AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo;
using AMS.Repositories.DatabaseRepos.SettlementItemRepo;
using AMS.Repositories.DatabaseRepos.SettlementRepo;
using AMS.Repositories.DatabaseRepos.AdminSupportRepo.Contracts;
using AMS.Repositories.DatabaseRepos.AdminSupportRepo;

namespace AMS.Repositories.UnitOfWork
{
    public class UnitOfWork : BaseUow, IUnitOfWork
    {
        #region Instance Fields

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        private IConfigurationRepo _configurationRepo;
        private ISessionRepo _sessionRepo;
        private IUserRepo _userRepo;
        private IDashboardRepo _dashboardRepo;
        private IParticularRepo _particularRepo;
        private IItemCategoryRepo _itemCategoryRepo;
        private IItemRepo _itemRepo;
        private IUnitRepo _unitRepo;
        private IProjectRepo _projectRepo;
        private IDivRepo _divRepo;
        private IDistRepo _distRepo;
        private IThanaRepo _thanaRepo;
        private IDepartmentRepo _departmentRepo;
        private IEmailContentsRepo _emailRepo;
        private IEstimateTypeRepo _estimatetypeRepo;
        private IEstimationRepo _estimationRepo;
        private IEstimateLinkedRepo _estimateLinkedRepo;
        private IEstimateDetailsRepo _estimateDetailsRepo;
        private IEstimateApproverRepo _estimateApproverRepo;
        private IEstimationAttachmentRepo _estimationAttachmentRepo;
        private IRolePriorityRepo _rolePriorityRepo;
        private IEstimateChangeHistoryRepo _estimateChangeHistoryRepo;
        private IEstimateDetailsChangeHistoryRepo _estimateDetailsChangeHisotryRepo;
        private IEstimateApproverChangeHistoryRepo _estimateApproverChangeHistory;
        private IDepartmentWiseSummaryRepo _departmentWiseSummaryRepo;
        private IDepartmentWiseSummaryHistoryRepo _departmentWiseChangeHistoryRepo;
        private IParticularWiseSummaryRepo _particularWiseSummaryRepo;
        private IParticularWiseSummaryHistoryRepo _particularWiseSummaryHistoryRepo;
        private IEstimateApproverFeedbackRepo _estimateApproverFeedbackRepo;
        private IProcurementApprovalRepo _procurementApprovalRepo;
        private IFundRequisitionRepo _fundRequisitionRepo;
        private IFundRequisitionConfigRepo _fundRequisitionConfigRepo;
        private IFundDisburseRepo _fundDisburseRepo;
        private IEstimateMemoEntityRepo _estimateMemoEntityRepo;
        private IEstimationReferenceRepo _estimationReferenceRepo;
        private ISettlementDepartmentWiseSummaryRepo _settlementDepartmentWiseSummaryRepo;
        private ISettlementParticularWiseSummaryRepo _settlementParticularWiseSummaryRepo;
        private IMemoApproverRepo _memoApproverRepo;
        private IEstimateMemoAttachmentsRepo _estimateMemoAttachmentsRepo;
        private bool _disposed;
        private readonly ConnectionStringSettings _connectionSettings;
        private ISettlementRepo _settlementRepo;
        private ISettlementItemRepo _settlementItemRepo;
        private IAdminSupportRepo _adminSupportRepo;

        #endregion

        #region Constructor

        public UnitOfWork( IDbConnection dbConnection, ConnectionStringSettings connectionStrings, bool beginTransaction = true) : base(connectionStrings)
        {
            _connectionSettings = connectionStrings;

            // Setup Connection & Transaction
            _connection = dbConnection;
            _connection.Open();

            if (beginTransaction)
            {
                _transaction = _connection.BeginTransaction();
            }
        }

        #endregion

        #region Properties

        public IConfigurationRepo ConfigurationRepo
        {
            get { return _configurationRepo ??= new ConfigurationRepo(_connection, _transaction, _connectionSettings); }
        }

        public ISessionRepo SessionRepo
        {
            get { return _sessionRepo ??= new SessionRepo(_connection, _transaction, _connectionSettings); }
        }

        public IUserRepo UserRepo
        {
            get { return _userRepo ??= new UserRepo(_connection, _transaction, _connectionSettings); }
        }

        public IDashboardRepo DashboardRepo
        {
            get { return _dashboardRepo ??= new DashboardRepo(_connection, _transaction, _connectionSettings); }
        }

        public IParticularRepo ParticularRepo
        {
            get { return _particularRepo ??= new ParticularRepo(_connection, _transaction, _connectionSettings); }
        }

        public IItemCategoryRepo ItemCategoryRepo
        {
            get { return _itemCategoryRepo ??= new ItemCategoryRepo(_connection, _transaction, _connectionSettings); }
        }

        public IItemRepo ItemRepo
        {
            get { return _itemRepo ??= new ItemRepo(_connection, _transaction, _connectionSettings); }
        }

        public IProjectRepo ProjectRepo
        {
            get { return _projectRepo ??= new ProjectRepo(_connection, _transaction, _connectionSettings); }
        }

        public IUnitRepo UnitRepo
        {
            get { return _unitRepo ??= new UnitRepo(_connection, _transaction, _connectionSettings); }
        }

        public IDivRepo DivRepo
        {
            get { return _divRepo ??= new DivRepo(_connection, _transaction, _connectionSettings); }
        }

        public IDistRepo DistRepo
        {
            get { return _distRepo ??= new DIstRepo(_connection, _transaction, _connectionSettings); }
        }

        public IThanaRepo ThanaRepo
        {
            get { return _thanaRepo ??= new ThanaRepo(_connection, _transaction, _connectionSettings); }
        }

        public IDepartmentRepo DepartmentRepo
        {
            get { return _departmentRepo ??= new DepartmentRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEmailContentsRepo EmailRepo
        {
            get { return _emailRepo ??= new EmailContentsRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateTypeRepo EstimateTyeRepo
        {
            get { return _estimatetypeRepo ??= new EstimateTypeRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimationRepo EstimationRepo
        {
            get { return _estimationRepo ??= new EstimationRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateLinkedRepo EstimateLinkedRepo
        {
            get { return _estimateLinkedRepo ??= new EstimateLinkedRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateDetailsRepo EstimateDetailsRepo
        {
            get { return _estimateDetailsRepo ??= new EstimateDetailsRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateApproverRepo EstimateApproverRepo
        {
            get { return _estimateApproverRepo ??= new EstimateApproverRepo(_connection, _transaction, _connectionSettings); }
        }

        public IRolePriorityRepo RolePriorityRepo
        {
            get { return _rolePriorityRepo ??= new RolePriorityRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimationAttachmentRepo EstimationAttachment
        {
            get { return _estimationAttachmentRepo ??= new EstimationAttachmentRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateChangeHistoryRepo EstimateChangeHistoryRepo
        {
            get { return _estimateChangeHistoryRepo ??= new EstimateChageHistoryRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateDetailsChangeHistoryRepo EstimateDetailsChangeHistoryRepo
        {
            get { return _estimateDetailsChangeHisotryRepo ??= new EstimateDetailsChangeHistoryRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateApproverChangeHistoryRepo EstimateApproverChangeHistoryRepo
        {
            get { return _estimateApproverChangeHistory ??= new EstimateApproverChangeHistoryRepo(_connection, _transaction, _connectionSettings); }
        }

        public IDepartmentWiseSummaryRepo DepartmentWiseSummaryRepo
        {
            get { return _departmentWiseSummaryRepo ??= new DepartmentWiseSummaryRepo(_connection, _transaction, _connectionSettings); }
        }

        public IDepartmentWiseSummaryHistoryRepo DepartmentWiseSummaryHistoryRepo
        {
            get { return _departmentWiseChangeHistoryRepo ??= new DepartmentWiseSummaryHistoryRepo(_connection, _transaction, _connectionSettings); }
        }

        public IParticularWiseSummaryRepo ParticularWiseSummaryRepo
        {
            get { return _particularWiseSummaryRepo ??= new ParticularWiseSummaryRepo(_connection, _transaction, _connectionSettings); }
        }

        public IParticularWiseSummaryHistoryRepo ParticularWiseSummaryHistoryRepo
        {
            get { return _particularWiseSummaryHistoryRepo ??= new ParticularWiseSummaryHistoryRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateApproverFeedbackRepo EstimateApproverFeedbackRepo
        {
            get { return _estimateApproverFeedbackRepo ??= new EstimateApproverFeedbackRepo(_connection, _transaction, _connectionSettings); }
        }

        public IProcurementApprovalRepo ProcurementApprovalRepo
        {
            get { return _procurementApprovalRepo ??= new ProcurementApprovalRepo(_connection, _transaction, _connectionSettings); }
        }
        public IFundRequisitionRepo FundRequisitionRepo
        {
            get { return _fundRequisitionRepo ??= new FundRequisitionRepo(_connection, _transaction, _connectionSettings); }
        }
        public IFundRequisitionConfigRepo FundRequisitionConfigRepo
        {
            get { return _fundRequisitionConfigRepo ??= new FundRequisitionConfigRepo(_connection, _transaction, _connectionSettings); }
        }

        public IFundDisburseRepo FundDisburseRepo
        {
            get { return _fundDisburseRepo ??= new FundDisburseRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateMemoEntityRepo EstimateMemoEntityRepo
        {
            get { return _estimateMemoEntityRepo ?? new EstimateMemoEntityRepo(_connection, _transaction, _connectionSettings);  }
        }

        public IEstimationReferenceRepo EstimationReferenceRepo
        {
            get { return _estimationReferenceRepo ?? new EstimationReferenceRepo(_connection, _transaction, _connectionSettings); }
        }

        public ISettlementDepartmentWiseSummaryRepo SettlementDepartmentWiseSummaryRepo
        {
            get { return _settlementDepartmentWiseSummaryRepo ?? new SettlementDepartmentWiseSummaryRepo(_connection, _transaction, _connectionSettings); }
        }

        public ISettlementParticularWiseSummaryRepo SettlementParticularWiseSummaryRepo
        {
            get { return _settlementParticularWiseSummaryRepo ?? new SettlementParticularWiseSummaryRepo(_connection, _transaction, _connectionSettings); }
        }

        public IMemoApproverRepo MemoApproverRepo
        {
            get { return _memoApproverRepo ?? new MemoApproverRepo(_connection, _transaction, _connectionSettings); }
        }

        public IEstimateMemoAttachmentsRepo EstimateMemoAttachmentsRepo
        {
            get { return _estimateMemoAttachmentsRepo ?? new EstimateMemoAttachmentsRepo(_connection, _transaction, _connectionSettings); }
        }

        public ISettlementRepo SettlementRepo
        {
            get { return _settlementRepo ??= new SettlementRepo(_connection, _transaction, _connectionSettings); }
        }

        public ISettlementItemRepo SettlementItemRepo
        {
            get { return _settlementItemRepo ??= new SettlementItemRepo(_connection, _transaction, _connectionSettings); }
        }
        public IAdminSupportRepo AdminSupportRepo
        {
            get { return _adminSupportRepo ??= new AdminSupportRepo(_connection, _transaction, _connectionSettings); }
        }
        #endregion

        #region Public Methods

        public bool Commit()
        {
            if (_transaction == null)
            {
                ResetRepositories();
                return true;
            }

            try
            {
                _transaction.Commit();

                return true;
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
                ResetRepositories();
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        private void ResetRepositories()
        {
            _configurationRepo = null;
            _sessionRepo = null;
            _userRepo = null;
            _dashboardRepo = null;
            _adminSupportRepo = null;
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            dispose(false);
        }

        #endregion
    }
}
