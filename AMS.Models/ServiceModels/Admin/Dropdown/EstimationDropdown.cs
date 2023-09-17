using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.ServiceModels.Admin.Dropdown
{
    public class EstimationDropdown
    {
        public int Id { get; set; }
        public int EstimationTypeId { get; set; }
        public int IsDefaultSelected { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string DropdownName { get; set; }
    }
}
