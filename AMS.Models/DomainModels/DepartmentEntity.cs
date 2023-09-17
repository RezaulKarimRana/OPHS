namespace AMS.Models.DomainModels
{
    public class DepartmentEntity : BaseEntity
    {
        public string Name { get; set; }
        public int CanEdit { get; set; }
    }
}
