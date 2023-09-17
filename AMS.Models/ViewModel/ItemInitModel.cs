using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using System.Collections.Generic;

namespace AMS.Models.ViewModel
{
    public class ItemInitModel
    {
        public List<NameIdPairModel> ParitcularList { get; set; }
        public List<NameIdPairModel> ItemCategoryList { get; set; }
        public List<NameIdPairModel> UnitList { get; set; }
        public List<NameIdPairModel> ModuleList { get; set; }
        public List<NameIdPairModel> StatusList { get; set; }
        public List<NameIdPairModel> RoleList { get; set; }
        public List<DepartmentEntity> DepartmentList { get; set; }
        public List<GetUsersWithDepartmentName> Users { get; set; }
    }
    public class ItemSaveModel
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int ParticularId { get; set; }
        public int ItemCategoryId { get; set; }
        public decimal IndicatingUnitPrice { get; set; }
        public int UnitId { get; set; }
        public int CreatedById { get; set; }
    }
    public class NameModel
    {
        public string Name { get; set; }
        public int CreatedById { get; set; }
    }
    public class ItemCategoryModel
    {
        public int ParticularId { get; set; }
        public string Name { get; set; }
        public int CreatedById { get; set; }
    }
}
