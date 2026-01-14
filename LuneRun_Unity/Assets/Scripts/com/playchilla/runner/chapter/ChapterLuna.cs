using shared.math;
using com.playchilla.runner.player;

namespace com.playchilla.runner.chapter
{
    public class ChapterLuna : Chapter
    {
        private readonly Vec3 _wantedLookPos = new Vec3();
        private int _step = 0;
        private PlayerCam _cam;
        private int _startTick = -1;

        public ChapterLuna(Level level) : base(level)
        {
            _level.topPush("One hour ago", onTitleDone);
            _cam = _level.getPlayerView().getCam();
        }

        public void onTitleDone()
        {
            _level.topPush("it is happening", onHappeningDone);
        }

        public void onHappeningDone()
        {
            _level.topPush("hurry, do not rest", onDone);
        }

        public void onDone()
        {
            _done = true;
        }

        public override void tick(int deltaTime)
        {
            var player = _level.getPlayer();
            if (_startTick == -1)
            {
                _startTick = deltaTime;
            }
            if (_step != 0)
            {
                if (_step != 1)
                {
                    if (_step != 2)
                    {
                        if (_step == 3)
                        {
                            _cam.SetLookOffset(new Vec3(0, 0, 0), 0.1);
                        }
                    }
                    else
                    {
                        _cam.SetLookOffset(new Vec3(-1, 0.7, 0), 0.04);
                        if (deltaTime >= _startTick + 250)
                        {
                            _step = 3;
                        }
                    }
                }
                else
                {
                    _cam.SetLookOffset(new Vec3(-1, 0.7, 0), 0.04);
                    if (deltaTime >= _startTick + 120)
                    {
                        _step = 2;
                    }
                }
            }
            else
            {
                _cam.SetLookOffset(new Vec3(0, 0, 0), 1);
                if (deltaTime >= _startTick + 50)
                {
                    _step = 1;
                }
            }
        }
    }
}