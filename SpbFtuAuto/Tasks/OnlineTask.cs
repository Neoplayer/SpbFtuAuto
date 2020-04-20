using SpbFtuAuto.Data;
using SpbFtuAuto.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SpbFtuAuto.Tasks
{
    public class OnlineTask : TaskBase
    {
        public OnlineTask(bool repeatable) : base(repeatable)
        {
        }

        ApplicationDbContext _db = new ApplicationDbContext();

        public override void Execute()
        {
            foreach (var user in _db.Users.Where(x => x.IsActive == true && x.GroupId != 1 && (DateTime.Now - x.LastLogin).TotalSeconds < 900d))
            {
                foreach (var lesson in user.Group.Lessons.Where(x => x.DayOfWeek == ConvertDayOfWeek(DateTime.Now.DayOfWeek) && DateTime.Now.TimeOfDay > x.FromTimeOfDay && DateTime.Now.TimeOfDay < x.ToTimeOfDay))
                {
                    if(lesson.Group.Users.Contains(user))
                    {
                        if(!LogIn(user.FtuEmail, user.FtuPassword, lesson.Subject.Id))
                        {
                            return;
                        }
                    }
                }
                user.LastLogin = DateTime.Now;
                _db.SaveChanges();
            }
        }

        private bool LogIn(string Email, string Password, int CourseId)
        {
            var browser = new Browser<ESteamRequestType>();

            var InitRequest = browser.ExecuteRequest(new Request<ESteamRequestType>(HttpMethod.Get,
                "https://edu.spbftu.ru/login/index.php", ESteamRequestType.Request));
            var loginTokenResponce = InitRequest.HttpRespMsg.Result.Content.ReadAsStringAsync().Result;
            Thread.Sleep(TimeSpan.FromSeconds(2));
            var loginToken = loginTokenResponce.Split("<input type=\"hidden\" name=\"logintoken\" value=\"")[1].Substring(0, 32);
            var LoginMoodleSession = InitRequest
                .HttpRespMsg
                .Result
                .Headers
                .ToDictionary(x => x.Key, x => x.Value)["Set-Cookie"]
                .First()
                .Split("MoodleSession=")[1]
                .Substring(0, 26);

            browser.CookieContainer.Add(new Cookie("MoodleSession", LoginMoodleSession, "/", "edu.spbftu.ru"));

            Dictionary<string, string> requestData = new Dictionary<string, string>
            {
                { "anchor", "" },
                { "logintoken", loginToken},
                { "username", Email },
                { "password", Password }
            };
            var LoginRequest = browser.ExecuteRequest(new Request<ESteamRequestType>(HttpMethod.Post,
                "https://edu.spbftu.ru/login/index.php", ESteamRequestType.Request, requestData));

            var Login = LoginRequest
                .HttpRespMsg
                .Result
                .Headers
                .ToDictionary(x => x.Key, x => x.Value);
            var s = LoginRequest.GetContent();

            browser.ExecuteRequest(new Request<ESteamRequestType>(HttpMethod.Get,
                $"https://edu.spbftu.ru/user/index.php?id={CourseId}", ESteamRequestType.Request));

            if (s.Contains("Личный кабинет"))
                return true;
            else
                return false;
        }

        private SpbFtuAuto.Data.DataObjects.DaysOfWeek ConvertDayOfWeek(DayOfWeek day)
        {
            switch(day)
            {
                case DayOfWeek.Monday:
                    return SpbFtuAuto.Data.DataObjects.DaysOfWeek.Понедельник;
                case DayOfWeek.Tuesday:
                    return SpbFtuAuto.Data.DataObjects.DaysOfWeek.Вторник;
                case DayOfWeek.Wednesday:
                    return SpbFtuAuto.Data.DataObjects.DaysOfWeek.Среда;
                case DayOfWeek.Thursday:
                    return SpbFtuAuto.Data.DataObjects.DaysOfWeek.Черверг;
                case DayOfWeek.Friday:
                    return SpbFtuAuto.Data.DataObjects.DaysOfWeek.Пятница;
                case DayOfWeek.Saturday:
                    return SpbFtuAuto.Data.DataObjects.DaysOfWeek.Суббота;
            }
            return SpbFtuAuto.Data.DataObjects.DaysOfWeek.Понедельник;
        }
    }
}
