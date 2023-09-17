namespace AMS.Repositories.DatabaseRepos.ItemRepo.Models
{
    public class UpdateItemRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Unit_Id { get; set; }
        public string ItemCode { get; set; }
        public int ItemCategory_Id { get; set; }
        public int IndicatingUnitPrice { get; set; }
        public int Updated_By { get; set; }
    }
}
