using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DomainModels
{
    public class EstimateMemoAttachmentsEntity : BaseEntity
    {
        public string URL { get; set; }
        public string FileName { get; set; }
        public int EstimateMemo_Id { get; set; }
    }
}
