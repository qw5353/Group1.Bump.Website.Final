﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace webapi.Models
{
    public partial class Cart
    {
        public Cart()
        {
            CartDetails = new HashSet<CartDetail>();
        }

        public int Id { get; set; }
        public int MemberId { get; set; }

        public virtual Member Member { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}