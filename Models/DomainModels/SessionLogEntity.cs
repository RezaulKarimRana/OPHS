namespace Models.DomainModels
{
    public class SessionLogEntity : BaseEntity
    {
        public int Session_Id { get; set; }

        public string Method { get; set; }

        public string Page { get; set; }

        public string Handler_Name { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public bool IsAJAX { get; set; }

        public string Action_Data_JSON { get; set; }

        public string Url { get; set; }

        public double Elapsed_Milliseconds { get; set; }
    }
}
