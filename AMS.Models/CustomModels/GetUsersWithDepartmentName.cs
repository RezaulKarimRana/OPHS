namespace AMS.Models.CustomModels
{
    public class GetUsersWithDepartmentName
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
