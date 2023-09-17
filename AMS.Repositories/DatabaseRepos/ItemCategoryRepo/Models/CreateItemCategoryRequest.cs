namespace AMS.Repositories.DatabaseRepos.ItemCategoryRepo.Models
{
    public class CreateItemCategoryRequest
    {
        public string Name { get; set; }
        public int Particular_Id { get; set; }
        public int Created_By { get; set; }
    }
}
