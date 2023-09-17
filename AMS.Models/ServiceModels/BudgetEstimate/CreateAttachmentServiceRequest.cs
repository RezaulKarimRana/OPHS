namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class CreateAttachmentServiceRequest
    {
        public string URL { get; set; }
        public string FileName { get; set; }
        public int Estimation_Id { get; set; }
    }
}
