﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace webapi.Models
{
    public partial class SkillCourse
    {
        public SkillCourse()
        {
            Skillcurricula = new HashSet<Skillcurriculum>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public int PeopleMin { get; set; }
        public int PeopleMax { get; set; }

        public virtual ICollection<Skillcurriculum> Skillcurricula { get; set; }
    }
}