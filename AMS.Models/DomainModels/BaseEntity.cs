using System;

namespace AMS.Models.DomainModels
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public int Created_By { get; set; }

        public DateTime Created_Date { get; set; }

        public int Updated_By { get; set; }

        public DateTime Updated_Date { get; set; }

        public bool Is_Deleted { get; set; }

        public static class EntityStatus
        {
            public static int Active { get { return 1; } }
            public static int InActive { get { return -1; } }
            public static int Pending { get { return 2; } }
            public static int Completed { get { return 100; } }
            public static int CR { get { return -404; } }
            public static int Draft { get { return 5; } }
            public static int Reject { get { return -500; } }
        }
    }

    public static class StatusTypeText
    {
        public static string Active { get { return "Active"; } }
        public static string Inactive { get { return "Inactive"; } }
        public static string Pending { get { return "Pending"; } }
        public static string Completed { get { return "Completed"; } }
        public static string CR { get { return "CR"; } }
        public static string Draft { get { return "Draft"; } }
        public static string Reject { get { return "Reject"; } }

        public static string GetStatusText(int status)
        {
            if (status == BaseEntity.EntityStatus.Active)
            {
                return Active;
            }
            else if (status == BaseEntity.EntityStatus.InActive)
            {
                return Inactive;
            }
            else if (status == BaseEntity.EntityStatus.Pending)
            {
                return Pending;
            }
            else if (status == BaseEntity.EntityStatus.Completed)
            {
                return Completed;
            }
            else if (status == BaseEntity.EntityStatus.CR)
            {
                return CR;
            }
            else if (status == BaseEntity.EntityStatus.Draft)
            {
                return Draft;
            }
            else if (status == BaseEntity.EntityStatus.Reject)
            {
                return Reject;
            }
            else
            {
                return "Unknown";
            }
        }
    }
}
