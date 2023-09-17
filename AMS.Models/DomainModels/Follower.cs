using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.DomainModels
{
    public class Follower : BaseEntity
    {
        public int UserId { get; set; }
        public int FollowerUserId { get; set; }
    }
}
