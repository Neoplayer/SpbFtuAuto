using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using SpbFtuAuto.Data;
using SpbFtuAuto.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpbFtuAuto.Areas.Identity.Pages.Account.Manage
{
    public partial class GroupPage : ComponentBase
    {
        private ApplicationDbContext db;

        public string TempValue { get; set; }
        List<string> s = new List<string>();
        public string Selected { get; set; }

        public GroupPage()
        {
        }

        //public SpbFtuAuto.Data.DataObjects.Group Group { get; set; }
        //public string groupName
        //{
        //    get
        //    {
        //        return Group == null ? "" : Group.Name;
        //    }
        //    set
        //    {
        //        if (value != "")
        //            Group = db.Groups.FirstOrDefault(x => x.Id == Convert.ToInt32(value));
        //    }
        //}


        void Click(MouseEventArgs e)
        {
            s.Add(TempValue);
            TempValue = "";
        }

        protected override void OnInitialized()
        {
        }
    }
}
