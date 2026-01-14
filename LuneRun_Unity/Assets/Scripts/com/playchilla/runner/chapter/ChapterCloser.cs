using shared.math;
using com.playchilla.runner.player;

namespace com.playchilla.runner.chapter
{
    public class ChapterCloser : Chapter
    {
        private readonly Vec3 _wantedLookPos = new Vec3();
        private int _step = 0;
        private PlayerCam _cam;
        private int _startTick = -1;

        public ChapterCloser(Level level) : base(level)
        {
            _level.topPush("the planet, so beautiful", onTitleDone);
            _cam = _level.getPlayerView().getCam();
        }

        public void onTitleDone()
        {
            _level.topPush("yet the end of this world", onDone);
        }

        public void onDone()
        {
            _done = true;
        }

        public override void tick(int deltaTime)
        {
            // empty
        }
    }
}