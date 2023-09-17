namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class CreateEstimateApproverServiceRequest
    {
        public int Estimate_Id { get; set; }
        public int User_Id { get; set; }
        public string ApproverFullName { get; set; }
        public string ApproverDepartment { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int RolePriority_Id { get; set; }
        public string ApproverRoleName { get; set; }
        public string ApproverRole { get; set; }
        public string PlanDate { get; set; }
        public string ExpectedTime { get; set; }

        public CreateEstimateApproverServiceRequest()
        {
            Remarks = "";
            RolePriority_Id = 0;
        }
    }

}
