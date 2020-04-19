using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpbFtuAuto.Data.DataObjects
{
    public class Lesson
    {
        public int Id { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public Subject Subject { get; set; }
        public Time Time { get; set; }
    }
}
