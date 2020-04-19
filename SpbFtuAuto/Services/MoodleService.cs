using SpbFtuAuto.Data;
using SpbFtuAuto.Data.DataObjects;
using SpbFtuAuto.Tasks;
using SpbFtuAuto.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpbFtuAuto.Services
{
    public class MoodleService
    {
        private Thread _queueThread;

        public bool IsActive = true;
        readonly ConcurrentQueue<TaskBase> _tasks = new ConcurrentQueue<TaskBase>();

        public MoodleService()
        {}

        public void Start()
        {   


            _queueThread = new Thread(QueueCycle);
            _queueThread.Start();
        }

        public void AddTaskToQueue(TaskType TaskName, bool IsRepeatable = false)
        {
            switch (TaskName)
            {
                case TaskType.Online:
                    _tasks.Enqueue(new OnlineTask(IsRepeatable));
                    break;
                default:
                    break;
            }
        }

        public enum TaskType
        {
            Online
        }

        private void QueueCycle()
        {
            Logger.Log("Queue Thread Start", Logger.LogType.Cyan);
            while (IsActive)
            {
                _tasks.TryDequeue(out TaskBase result);
                if (result != null)
                {
                    try
                    {
                        result.Execute();
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e);
                        Thread.Sleep(TimeSpan.FromSeconds(15));
                    }
                    if (result.IsRepeatable())
                    {
                        _tasks.Enqueue(result);
                    }
                    continue;
                }
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
            Logger.Log("Queue Thread Stop", Logger.LogType.Cyan);
        }
    }
}
