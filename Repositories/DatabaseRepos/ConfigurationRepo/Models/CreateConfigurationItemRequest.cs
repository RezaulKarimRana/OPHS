using System;

namespace Repositories.DatabaseRepos.ConfigurationRepo.Models
{
    public class CreateConfigurationItemRequest
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

        public int Created_By { get; set; }
    }
}
