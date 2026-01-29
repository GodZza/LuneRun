using UnityEngine;
using com.playchilla.runner.player;
using com.playchilla.runner.api;
using com.playchilla.runner.highscore;
using com.playchilla.runner.ambient;
using com.playchilla.gameapi.api;
using shared.input;
using shared.math;
using shared.timer;
using shared.debug;
using Audio = com.playchilla.runner.Audio;
using shared.sound;
using away3d.containers;
using Ud = UnityEngine.Debug;

namespace com.playchilla.runner
{
    /// <summary>
    /// Game - 游戏主控制器
    /// 对应 AS 代码中的 Game 类
    /// </summary>
    public class Game : MonoBehaviour, ITickable, ITickHook
    {
        // Fields (对应 AS 代码中的 internal var)
        private KeyboardInput _keyboard = new KeyboardInput();
        private MouseInput _mouseInput = new MouseInput();
        private UnityTicker _ticker;
        private Level _level;
        private bool _setup = false;
        private bool _shouldExit = false;
        private bool _lastFullscreen;
        private int _startLevelId;
        private LevelHighscoreGui _highscoreGui = null;
        private Highscore _highscore;
        private IRunnerApi _runnerApi;
        private Settings _settings;
        private bool _isCompleted = false;
        private Curtain _curtain;
        private bool _isHardware;

        /// <summary>
        /// 构造函数（在Unity中通常通过初始化方法替代）
        /// 对应 AS: public function Game(arg1:away3d.containers.View3D, arg2:int, arg3:com.playchilla.runner.api.IRunnerApi, arg4:com.playchilla.runner.player.Settings, arg5:Boolean)
        /// </summary>
        public void Initialize(int startLevelId, IRunnerApi runnerApi, Settings settings, bool isHardware)
        {
            _startLevelId = startLevelId;
            _runnerApi = runnerApi;
            _settings = settings;
            _isHardware = isHardware;

            // 对应 AS: this._ticker = new shared.timer.Ticker(this, 25, 1, this);
            // 在 Unity 中使用 UnityTicker，它会在 Update 中自动执行 tick
            _ticker = new UnityTicker(this, 25f, 1, this);

            // 添加到 Unity 生命周期
            enabled = true;
        }

        void Start()
        {
            // 对应 AS: _onAddedToStage
            OnAddedToStage();
        }

        void Update()
        {
            // 对应 AS: public function update():void
            UpdateMethod();
        }

        /// <summary>
        /// 对应 AS: internal function _onAddedToStage(arg1:flash.events.Event):void
        /// </summary>
        private void OnAddedToStage()
        {
            // 监听键盘事件（Unity 中通过 Input 自动处理，AS 中通过事件监听）
            // Unity 不需要手动添加键盘/鼠标事件监听器，Input 系统会自动处理

            // 触摸事件支持（Unity 中通过 Input.touchCount 自动检测）
            if (Input.touchSupported)
            {
                // Unity 自动处理触摸事件
            }

            _setup = true;

            // 播放背景音乐
            Audio.Music.getSound(Sound.STrack1).loop(0.5f);

            // 创建初始关卡
            CreateLevel(_startLevelId, true);
        }

        /// <summary>
        /// 对应 AS: public function tick(arg1:int):Boolean
        /// </summary>
        public bool tick(int deltaTime)
        {
            if (_highscoreGui != null)
            {
                //_highscoreGui.tick(deltaTime);
                //ReposHighscore();
                //if (_keyboard.IsPressed(KeyCode.Tab))
                //{
                //    HideHighscore();
                //}
                //return;
            }
            else
            {
                _level.Tick(deltaTime);
                if (_level.HasFailed())
                {
                    CreateLevel(_level.GetLevelId(), false);
                }
                if (_level.HasCompleted() && _curtain == null)
                {
                    _curtain = Utils.New<Curtain>();
                    _curtain.Initialize(3, 1);
                    _curtain.Close(OnClosedCurtain); // 暂不实现 Curtan 的 close 方法
                    _isCompleted = true;
                }
                if (_keyboard.IsPressed(KeyCode.Tab))
                {
                    ShowHighscore();
                }
            }
            return true;
        }

        /// <summary>
        /// 对应 AS: internal function _onClosedCurtain():void
        /// </summary>
        private void OnClosedCurtain()
        {
            _shouldExit = true;
        }

        /// <summary>
        /// 对应 AS: internal function _createLevel(arg1:int, arg2:Boolean):void
        /// </summary>
        private void CreateLevel(int levelId, bool showChapter)
        {
            if (_level != null)
            {
                UnityEngine.Object.Destroy(_level);
            }
            _level = new GameObject($"Level {levelId}").AddComponent<Level>();
            // _level.Initialize(_view, _mouseInput, _keyboard, levelId, showChapter, _settings, _runnerApi, _isHardware);
            _ticker.Reset();
        }

        /// <summary>
        /// 对应 AS: public function preTick(arg1:int):void
        /// </summary>
        public void PreTick(int deltaTime)
        {
            //Engine.pt.Start("level.tick");
        }

        /// <summary>
        /// 对应 AS: public function postTick(arg1:int):void
        /// </summary>
        public void PostTick(int deltaTime)
        {
            //Engine.pt.Stop("level.tick");
            //Engine.pt.Start("level.renderTick");
            //_level.RenderTick(deltaTime);
            //Engine.pt.Stop("level.renderTick");
            _mouseInput.Reset();
            _keyboard.Reset();
        }

        //internal function _onAddedToStage(arg1:flash.events.Event):void
        //{
        //    removeEventListener(flash.events.Event.ADDED_TO_STAGE, this._onAddedToStage);
        //stage.addEventListener(flash.events.KeyboardEvent.KEY_DOWN, this._onKeyDown);
        //stage.addEventListener(flash.events.KeyboardEvent.KEY_UP, this._onKeyUp);
        //stage.addEventListener(flash.events.MouseEvent.MOUSE_DOWN, this._onMouseDown);
        //stage.addEventListener(flash.events.MouseEvent.MOUSE_UP, this._onMouseUp);
        //    if (flash.ui.Multitouch.supportsTouchEvents) 
        //    {
        //        flash.ui.Multitouch.inputMode = flash.ui.MultitouchInputMode.TOUCH_POINT;
        //        stage.addEventListener(flash.events.TouchEvent.TOUCH_BEGIN, this._onTouchBegin);
        //stage.addEventListener(flash.events.TouchEvent.TOUCH_END, this._onTouchEnd);
        //}
        //    this._setup = true;
        //    com.playchilla.runner.Audio.Music.getSound(STrack1).loop(0.5);
        //    this._createLevel(this._startLevelId, true);
        //    return;
        //}

    /// <summary>
    /// 对应 AS: internal function _onKeyDown(arg1:flash.events.KeyboardEvent):void
    /// </summary>
    private void OnKeyDown(int keyCode)
        {
            _keyboard.SetPress(keyCode);
        }

        /// <summary>
        /// 对应 AS: internal function _onKeyUp(arg1:flash.events.KeyboardEvent):void
        /// </summary>
        private void OnKeyUp(int keyCode)
        {
            _keyboard.SetRelease(keyCode);
        }

        /// <summary>
        /// 对应 AS: internal function _onMouseDown(arg1:flash.events.MouseEvent):void
        /// </summary>
        private void OnMouseDown(float x, float y)
        {
            _mouseInput.SetPress(new Vec2(x, y));
        }

        /// <summary>
        /// 对应 AS: internal function _onMouseUp(arg1:flash.events.MouseEvent):void
        /// </summary>
        private void OnMouseUp(float x, float y)
        {
            _mouseInput.SetRelease(new Vec2(x, y));
        }

        /// <summary>
        /// 对应 AS: internal function _onTouchBegin(arg1:flash.events.TouchEvent):void
        /// </summary>
        private void OnTouchBegin()
        {
            _keyboard.SetPress(KeyCode.Space);
        }

        /// <summary>
        /// 对应 AS: internal function _onTouchEnd(arg1:flash.events.TouchEvent):void
        /// </summary>
        private void OnTouchEnd()
        {
            _keyboard.SetRelease(KeyCode.Space);
        }

        /// <summary>
        /// 对应 AS: public function _showHighscore():void
        /// </summary>
        public void ShowHighscore()
        {
            shared.debug.Debug.Assert(_highscoreGui == null, "Trying to create two highscore guis.");
            // _highscoreGui = new LevelHighscoreGui(_runnerApi.GetUserId(), _level.GetLevelId(), _runnerApi, OnCloseHighscore, "BACK", OnCloseHighscore, true);
            ReposHighscore();
            // addChild(_highscoreGui); // Unity 中通过 GameObject 添加
        }

        /// <summary>
        /// 对应 AS: internal function _reposHighscore():void
        /// </summary>
        private void ReposHighscore()
        {
            // 在 Unity 中，UI 位置通过 RectTransform 或 Canvas 处理
            // _highscoreGui.x = (_view.width - _highscoreGui.width) * 0.5;
            // _highscoreGui.y = (_view.height - HighscoreGui.DialogHeight) * 0.5;
        }

        /// <summary>
        /// 对应 AS: internal function _onCloseHighscore(arg1:flash.events.Event):void
        /// </summary>
        private void OnCloseHighscore()
        {
            HideHighscore();
        }

        /// <summary>
        /// 对应 AS: public function _hideHighscore():void
        /// </summary>
        public void HideHighscore()
        {
            shared.debug.Debug.Assert(!(_highscoreGui == null), "Trying to remove a non existing highscore dialog.");
            // removeChild(_highscoreGui);
            // _highscoreGui.destroy();
            _highscoreGui = null;
        }

        /// <summary>
        /// 对应 AS: public function isCompleted():Boolean
        /// </summary>
        public bool IsCompleted()
        {
            return _isCompleted;
        }

        /// <summary>
        /// 对应 AS: public function destroy():void
        /// </summary>
        public void Destroy()
        {
            // 移除事件监听器（Unity 中不需要手动移除）
            _level.Destroy();
            if (_highscoreGui != null)
            {
                // _highscoreGui.destroy();
            }
            Audio.Music.getSound(Sound.STrack1).stop(1000);
        }

        /// <summary>
        /// 对应 AS: public function update():void
        /// </summary>
        private void UpdateMethod()
        {
            if (!_setup)
            {
                return;
            }

            // 处理 ESC 键退出
            if (!_lastFullscreen && _keyboard.IsPressed((int)KeyCode.Escape))
            {
                _shouldExit = true;
            }
            _lastFullscreen = Screen.fullScreen;

            // 更新鼠标位置
            _mouseInput.SetPosition(new Vec2(Input.mousePosition.x, Input.mousePosition.y));

            // 执行 ticker 步进
            _ticker.Step();

            //// 渲染
            //Engine.pt.Start("level.render");
            //_level.Render(_ticker.GetTick(), _ticker.GetAlpha());
            //Engine.pt.Stop("level.render");

            if (_highscoreGui != null)
            {
                // _highscoreGui.render(_ticker.GetTick(), _ticker.GetAlpha());
            }
        }

        /// <summary>
        /// 对应 AS: public function shouldExit():Boolean
        /// </summary>
        public bool ShouldExit()
        {
            return _shouldExit;
        }

        /// <summary>
        /// 获取键盘输入（用于外部访问）
        /// </summary>
        public KeyboardInput GetKeyboardInput()
        {
            return _keyboard;
        }

        /// <summary>
        /// 获取鼠标输入（用于外部访问）
        /// </summary>
        public MouseInput GetMouseInput()
        {
            return _mouseInput;
        }

        /// <summary>
        /// 获取关卡实例（用于外部访问）
        /// </summary>
        public Level GetLevel()
        {
            return _level;
        }
    }
}


public static class Utils
{
    public static T New<T>(string name = null) where T : Component
    {
        var go = new GameObject();
        if (string.IsNullOrEmpty(name))
        {
            go.name = typeof(T).Name;
        }
        else
        {
            go.name = name;
        }
        return go.AddComponent<T>();
    }

    public static void Destroy<T>(this T instance) where T : Component
    {
        UnityEngine.Object.Destroy(instance);
    }
}

//      Tick
namespace com.playchilla.runner
{
    public class Tick 
    {
        public static double ticksToSec(int tick)
        {
            return 1 / ticksPerSecond * tick;
        }

        public static long secToTicks(double sec)
        {
            return (long)(ticksPerSecond * sec);
        }

        public const int ticksPerSecond = 25;
    }
}


