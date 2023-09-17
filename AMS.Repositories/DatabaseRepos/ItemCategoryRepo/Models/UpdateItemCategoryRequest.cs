namespace AMS.Repositories.DatabaseRepos.ItemCategoryRepo.Models
{
    public class UpdateItemCategoryRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Particular_Id { get; set; }
        public int Updated_By { get; set; }
    }
}
