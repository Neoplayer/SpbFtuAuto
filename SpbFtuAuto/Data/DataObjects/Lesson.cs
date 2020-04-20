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

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public DaysOfWeek DayOfWeek { get; set; }
        public TimeSpan FromTimeOfDay { get; set; }
        public TimeSpan ToTimeOfDay { get; set; }
    }
    public enum DaysOfWeek{
        Понедельник,
        Вторник,
        Среда,
        Черверг,
        Пятница,
        Суббота
    }
}
