using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpbFtuAuto.Data.DataObjects
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public List<Lesson> Lessons { get; set; }
        public Group()
        {
            Lessons = new List<Lesson>();
        }
    }
}
