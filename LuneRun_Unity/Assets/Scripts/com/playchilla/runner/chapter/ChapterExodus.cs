using shared.math;
using com.playchilla.runner.player;

namespace com.playchilla.runner.chapter
{
    public class ChapterExodus : Chapter
    {
        private readonly Vec3 _wantedLookPos = new Vec3();
        private int _step = 0;
        private PlayerCam _cam;
        private int _startTick = -1;

        public ChapterExodus(Level level) : base(level)
        {
            _level.topPush("You should be close to the teleporter", onTitleDone);
            _cam = _level.getPlayerView().getCam();
        }

        public void onTitleDone()
        {
            _level.topPush("but hurry, the cube is leaving", onDone);
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