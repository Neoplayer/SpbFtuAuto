using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpbFtuAuto.Data.DataObjects
{
    public class Time
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public WeekType WeekType { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public List<Lesson> Lessons { get; set; }

        public Time()
        {
            Lessons = new List<Lesson>();
        }
    }
    public enum WeekType
    {
        Even,
        Odd
    }
}
