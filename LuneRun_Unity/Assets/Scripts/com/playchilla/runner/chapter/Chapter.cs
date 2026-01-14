namespace com.playchilla.runner.chapter
{
    public class Chapter
    {
        protected Level _level;
        protected bool _done = false;

        public Chapter(Level level)
        {
            _level = level;
        }

        public bool isDone()
        {
            return _done;
        }

        public virtual void tick(int deltaTime)
        {
            // empty
        }

        public virtual void render(int time, double interpolation)
        {
            // empty
        }
    }
}