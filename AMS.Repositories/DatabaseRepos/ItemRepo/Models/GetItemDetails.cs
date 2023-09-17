using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Repositories.DatabaseRepos.ItemRepo.Models
{
    public class GetItemDetails
    {
        public int ParticularId { get; set; }
        public string ParticularName { get; set; }
        public int ItemCategoryId { get; set; }
        public string ItemCategoryName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemUnitId { get; set; }
        public string ItemUnitName { get; set; }
        public string ItemCode { get; set; }
        public double ItemUnitPrice { get; set; }

    }
}
