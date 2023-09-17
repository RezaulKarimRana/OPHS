﻿namespace AMS.Models.DomainModels
{
    public class ItemEntity : BaseEntity
    {
        public string Name { get; set; }
        public int Unit_Id { get; set; }
        public string ItemCode { get; set; }
        public int ItemCategory_Id { get; set; }
        public int IndicatingUnitPrice { get; set; }
        public int System_Id { get; set; }
        public string SystemName { get; set; }
    }
}