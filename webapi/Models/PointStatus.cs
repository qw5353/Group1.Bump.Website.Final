﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace webapi.Models
{
    public partial class PointStatus
    {
        public PointStatus()
        {
            MemberPoints = new HashSet<MemberPoint>();
        }

        public int Id { get; set; }
        public int Name { get; set; }

        public virtual ICollection<MemberPoint> MemberPoints { get; set; }
    }
}