using AMS.Models.CustomModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.Item
{
    public class GetItemUnitDetailsResponse : ServiceResponse
    {
        public List<GetItemUnitDetails> ItemUnitModel { get; set; }

        public GetItemUnitDetailsResponse()
        {
            ItemUnitModel = new List<GetItemUnitDetails>();
        }
    }
}
