using System;

namespace Models.DomainModels
{
    public class ConfigurationEntity : BaseEntity
    {
        public string Key { get; set; }

        public string Description { get; set; }

        public bool Is_Client_Side { get; set; }

        public bool? Boolean_Value { get; set; }

        public DateTime? DateTime_Value { get; set; }

        public DateTime? Date_Value { get; set; }

        public TimeSpan? Time_Value { get; set; }

        public decimal? Decimal_Value { get; set; }

        public int? Int_Value { get; set; }

        public decimal? Money_Value { get; set; }

        public string String_Value { get; set; }
    }
}
