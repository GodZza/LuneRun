using shared.math;
using com.playchilla.runner.player;

namespace com.playchilla.runner.chapter
{
    public class ChapterLongJump : Chapter
    {
        private readonly Vec3 _wantedLookPos = new Vec3();
        private int _step = 0;
        private PlayerCam _cam;
        private int _startTick = -1;

        public ChapterLongJump(Level level) : base(level)
        {
            _level.topPush("To get high speed and make long jumps", on1);
            _cam = _level.getPlayerView().getCam();
        }

        public void on1()
        {
            _level.topPush("jump in the next uphill", on2);
        }

        public void on2()
        {
            _level.topPush("then land in the slope holding SPACE", onDone);
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