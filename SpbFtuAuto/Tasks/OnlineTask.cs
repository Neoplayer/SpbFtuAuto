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
            if (DateTime.Now > DateTime.Parse("1:00") && DateTime.Now < DateTime.Parse("21:00"))
            {
                var users = _db.Users;
                var time = (DateTime.Now - TimeSpan.FromSeconds(10));
                foreach (User user in _db.Users.Where(x => x.FtuEmail != null && x.FtuPassword != null))
                {
                    if ((DateTime.Now - user.LastLogin).TotalSeconds < 900d)
                        continue;


                    foreach (var lesson in user.Group.Lessons.Where(x => x.Time.DayOfWeek == DateTime.Now.DayOfWeek))
                    {
                        Logger.Log($"Ligging to {lesson.Subject.Name} from user {user.Email}");
                        if(!LogIn(user.FtuEmail, user.FtuPassword, lesson.Subject.Id))
                        {
                            return;
                        }
                        Logger.Log($"Login complete");
                        Thread.Sleep(TimeSpan.FromSeconds(15));
                    }
                    user.LastLogin = DateTime.Now;
                    _db.SaveChanges();
                }
            }
        }

        public bool LogIn(string Email, string Password, int CourseId)
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
    }
}
