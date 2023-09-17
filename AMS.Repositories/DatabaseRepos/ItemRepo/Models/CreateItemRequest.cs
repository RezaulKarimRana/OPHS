namespace AMS.Repositories.DatabaseRepos.ItemRepo.Models
{
    public class CreateItemRequest
    {
        public string Name { get; set; }
        public int Unit_Id { get; set; }
        public string ItemCode { get; set; }
        public int ItemCategory_Id { get; set; }
        public int Particular_Id { get; set; }
        public decimal IndicatingUnitPrice { get; set; }
        public int System_Id { get; set; }
        public string SystemName { get; set; }
        public int Created_By { get; set; }
    }
    public class CreateAMSItemRequest
    {
        public string Name { get; set; }
        public int Unit_Id { get; set; }
        public int ItemCategory_Id { get; set; }
        public int Particular_Id { get; set; }
        public decimal IndicatingUnitPrice { get; set; }
        public string SystemName { get; set; }
        public int Created_By { get; set; }
    }
}
