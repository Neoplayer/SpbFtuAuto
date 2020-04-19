using System;

namespace SpbFtuAuto.Utils
{
    public static class Logger
    {
        static readonly object lockObject = new object();
        public static void LogNAE(string data)
        {
            Log($"NullArgumentException occured! Variable '{data}'", LogType.Red);
        }
        public static void Log(string data, LogType type = LogType.Default)
        {
            lock (lockObject)
            {
                switch (type)
                {
                    case LogType.Green:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogType.Red:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogType.Yellow:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogType.Cyan:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    default:
                        break;
                }
                string timecode = DateTime.Now.ToString("[hh:mm:ss]");
                Console.WriteLine(timecode + data);
                Console.ResetColor();
            }
        }
        public static void Log(Exception e)
        {
            Log(e.StackTrace, LogType.Red);
        }
        public enum LogType
        {
            Green,
            Red,
            Default,
            Yellow,
            Cyan
        }
    }
}
