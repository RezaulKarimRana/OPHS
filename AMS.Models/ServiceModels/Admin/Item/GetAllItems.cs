using AMS.Models.DomainModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.Item
{
    public class GetAllItems
    {
        public List<ItemEntity> Items { get; set; }

        public GetAllItems()
        {
            Items = new List<ItemEntity>();
        }
    }
}
