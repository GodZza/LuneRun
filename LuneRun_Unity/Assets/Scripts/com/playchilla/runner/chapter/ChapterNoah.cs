using shared.math;
using com.playchilla.runner.player;

namespace com.playchilla.runner.chapter
{
    public class ChapterNoah : Chapter
    {
        private readonly Vec3 _wantedLookPos = new Vec3();
        private int _step = 0;
        private PlayerCam _cam;
        private int _startTick = -1;

        public ChapterNoah(Level level) : base(level)
        {
            _level.TopPush("There she is", onTitleDone);
            _cam = _level.GetPlayerView().getCam();
        }

        public void onTitleDone()
        {
            _level.topPush("the interstellar mega cube", onHappeningDone);
        }

        public void onHappeningDone()
        {
            _level.topPush("I must get there in time", onDone);
        }

        public void onDone()
        {
            _done = true;
        }

        public override void tick(int deltaTime)
        {
            var player = _level.GetPlayer();
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
                            if (deltaTime >= _startTick + 270)
                            {
                                _done = true;
                            }
                        }
                    }
                    else
                    {
                        _cam.SetLookOffset(new Vec3(0.5, 2.9, 0), 0.04);
                        if (deltaTime >= _startTick + 250)
                        {
                            _step = 3;
                        }
                    }
                }
                else
                {
                    _cam.SetLookOffset(new Vec3(0.5, 2.9, 0), 0.04);
                    if (deltaTime >= _startTick + 120)
                        _step = 2;
                }
            }
            else
            {
                _cam.SetLookOffset(new Vec3(0, 0, 0), 1);
                if (deltaTime >= _startTick + 50)
                    _step = 1;
            }
        }
    }
}