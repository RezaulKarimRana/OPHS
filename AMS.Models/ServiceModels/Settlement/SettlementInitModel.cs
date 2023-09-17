using AMS.Models.ViewModel;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Settlement
{
    public class SettlementInitModel
    {
        public List<NameIdPairModel>? DistrictList { get; set; }
        public List<NameIdPairModel>? ParticularList { get; set; }
        public List<NameIdPairModel>? AreaTypeList { get; set; }
        public List<NameIdPairModel>? DepartmentList { get; set; }
    }
}
