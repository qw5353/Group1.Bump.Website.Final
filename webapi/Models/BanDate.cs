﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace webapi.Models
{
    public partial class BanDate
    {
        public int Id { get; set; }
        public DateTime BanStartDateTime { get; set; }
        public int FieldId { get; set; }
        public DateTime BanEndDateTime { get; set; }

        public virtual Field Field { get; set; }
    }
}