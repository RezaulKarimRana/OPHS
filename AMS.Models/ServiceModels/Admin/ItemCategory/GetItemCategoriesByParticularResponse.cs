using AMS.Models.DomainModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.ItemCategory
{
    public class GetItemCategoriesByParticularResponse : ServiceResponse
    {
        public IList<ItemCategoryEntity> ItemCategories { get; set; }
        public GetItemCategoriesByParticularResponse()
        {
            ItemCategories = new List<ItemCategoryEntity>();
        }
    }
}
