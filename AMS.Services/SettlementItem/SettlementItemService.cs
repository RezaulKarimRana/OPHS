using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Models.CustomModels;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Managers.Contracts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace AMS.Services.SettlementItem
{
    public class SettlementItemService : ISettlementItemService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;
        private readonly IDepartmentService _departmentService;
        private readonly IUserService _userService;
        private readonly IBudgetApproverService _budgetApproverService;
        private readonly IConfiguration _configuration;
        IDataProtector _protector;

        public SettlementItemService(IUnitOfWorkFactory uowFactory, ISessionManager sessionManager,
            IDepartmentService departmentService, IUserService userService,
            IBudgetApproverService budgetApproverService, IDataProtectionProvider provider,
            IConfiguration configuration)
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
            _departmentService = departmentService;
            _userService = userService;
            _budgetApproverService = budgetApproverService;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
        }

        public async Task<List<EstimateSettleCompleteItem>> getSettlementItemsByEstimateId(int estiId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new List<EstimateSettleCompleteItem>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.SettlementItemRepo.getSettlementItemsByEstimateId(sessionUser.Id,
                    sessionUser.Department_Id, estiId);
                uow.Commit();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EstimateSettleCompleteItem>> getSettlementItemsBySettlementId(int settlementId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new List<EstimateSettleCompleteItem>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.SettlementItemRepo.getSettlementItemsBySettlementId(sessionUser.Id,
                    sessionUser.Department_Id, settlementId);
                uow.Commit();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}