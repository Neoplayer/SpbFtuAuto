

namespace SpbFtuAuto.Tasks
{
    public abstract class TaskBase
    {
        protected TaskBase(bool repeatable)
        {
            Repeatable = repeatable;
        }

        private readonly bool Repeatable = false;
        public abstract void Execute();

        public bool IsRepeatable()
        {
            return Repeatable;
        }
    }
}