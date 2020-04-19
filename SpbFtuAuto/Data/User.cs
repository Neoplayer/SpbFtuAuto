using Microsoft.AspNetCore.Identity;
using SpbFtuAuto.Data.DataObjects;
using System;

namespace SpbFtuAuto.Data
{
    public class User : IdentityUser
    {
        public string FtuEmail { get; set; }
        public string FtuPassword { get; set; }
        public bool IsActive { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
