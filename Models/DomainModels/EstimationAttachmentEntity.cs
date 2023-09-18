namespace Models.DomainModels
{
    public class EstimationAttachmentEntity : BaseEntity
    {
        public string URL { get; set; }
        public string FileName { get; set; }
        public int Estimation_Id { get; set; }
    }
}
