namespace AMS.Infrastructure.Authorization
{
    /// <summary>
    /// These constants map to the [Key] field of each session event item
    /// </summary>
    public class PermissionKeys
    {
        public const string ViewSessions = "SESSIONS_VIEW";
        public const string ManageUsers = "USERS_MANAGE";
        public const string ManageBudgetEstimation = "BUDGET_ESTIMATION";
        public const string ManageRoles = "ROLES_MANAGE";
        public const string ManageConfiguration = "CONFIGURATION_MANAGE";

        public const string CreateEstimation = "CREATE_ESTIMATION";
        public const string DraftEstimation = "DRAFT_ESTIMATION";
        public const string Approvalparking = "APPROVAL_PARKING";
        public const string RunningStatusBoard = "STATUS_BOARD_RUNNING";
        public const string CompleteStatusBoard = "STATUS_BOARD_COMPLETE";
        

        //FollowingBoard
       

        public const string CompleteFollowingBoard = "COMPLETE_FOLLOWING_BOARD";
        public const string RunningBudgetForSepecificUser = "RUNNING_BUDGET_FOR_SPECIFIC_USER";
        public const string PendingBudgetForSepecificUser = "PENDING_BUDGET_FOR_SPECIFIC_USER";
        public const string AllBudgetForSepecificUser = "ALL_BUDGET_For_SPECIFIC_USER";
        public const string RejectedBudgetForSepecificUser = "REJECTED_BUDGET_FOR_SPECIFIC_USER";

        public const string RunningStatusBoardViewOnly = "STATUS_BOARD_RUNNING_VIEW_ONLY";
        public const string CompleteStatusBoardViewOnly = "STATUS_BOARD_COMPLETE_VIEW_ONLY";

        public const string DashboardView = "DashboardView";
        //Fund Settlement init
        public const string FUND_REQ_INIT = "FUND_REQ_INIT";
        public const string SETTLEMENT_REQ_INIT = "SETTLEMENT_REQ_INIT";
        //Fund Requisition

        public const string SubmittedFundRequisitionList = "SUBMITTED_FUND_REQUISITION_LIST";
        public const string RejectedFundRequisitionList = "REJECTED_FUND_REQUISITION_LIST";
        public const string CompletedFundRequisitionList = "COMPLETED_FUND_REQUISITION_LIST";
        //FUND DISBURSE REQUEST
        public const string FUND_DISBURSE_PENDING = "FUND_DISBURSE_PENDING";
        public const string FUND_DISBURSE_COMPLETED = "FUND_DISBURSE_COMPLETED";
        
        //SETTLEMENT
        public const string SETTLEMENT_PENDING = "SETTLEMENT_PENDING";
        public const string SETTLEMENT_PENDING_FINAL = "SETTLEMENT_PENDING_FINAL";
        public const string SETTLEMENT_COMPLETED = "SETTLEMENT_COMPLETED";
        public const string SETTLEMENT_REJECTED = "SETTLEMENT_REJECTED";
        public const string SETTLEMENT_ON_GOING = "SETTLEMENT_ON_GOING";
        public const string SETTLEMENT_ROLLBACK = "SETTLEMENT_ROLLBACK";

        //Memo
        public const string MEMO_INITIATE = "MEMO_INITIATE";
        public const string MEMO_COMPLETED = "MEMO_COMPLETED";
        public const string MEMO_PENDING = "MEMO_PENDING";
        public const string MEMO_STATUS_BOARD_RUNNING = "MEMO_STATUS_BOARD_RUNNING";
        public const string MEMO_STATUS_BOARD_COMPLETED = "MEMO_STATUS_BOARD_COMPLETED";

        //FUND DISBURSE Finance
        public const string FUND_DISBURSE_PENDING_FINANCE = "FUND_DISBURSE_PENDING_FINANCE";
        public const string FUND_DISBURSE_ROLLBACK_FINANCE = "FUND_DISBURSE_ROLLBACK_FINANCE";
        public const string FUND_DISBURSE_COMPLETED_FINANCE = "FUND_DISBURSE_COMPLETED_FINANCE";
        public const string FUND_REQUISITION_REJECTED_FINANCE = "FUND_REQUISITION_REJECTED_FINANCE";
        public const string FUND_DISBURSE_BY_FINANCE_WAITING_FOR_ACKNOWLEDGE = "FUND_DISBURSE_BY_FINANCE_WAITING_FOR_ACKNOWLEDGE";


    }
}
