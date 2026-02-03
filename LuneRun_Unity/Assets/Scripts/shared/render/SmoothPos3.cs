using shared.math;

namespace shared.render
{
    /// <summary>
    /// 解决在客户端渲染和服务器逻辑更新频率不同时，物体位置移动不平滑的问题。
    /// 使用场景：
    ///1. 网络游戏同步
    ///服务器： 每秒20次更新(50ms/tick)
    ///客户端： 每秒60帧渲染(16.7ms/帧)
    ///客户端每帧渲染时：
    ///- 用最新的服务器位置作为目标
    ///- 根据时间进度计算插值
    ///- 得到平滑移动效果
    ///2. 物理模拟
    ///物理引擎： 固定50fps更新
    ///渲染引擎： 可变帧率显示
    ///渲染时：
    ///- 从物理引擎获取最新位置
    ///- 基于上次物理更新的位置插值
    ///- 消除因帧率波动导致的卡顿
    /// </summary>
    public class SmoothPos3
    {
        private Vec3 _beginPos = new Vec3();
        private Vec3 _endPos = new Vec3();
        private Vec3 _dir = new Vec3();
        private Vec3 _temp = new Vec3();
        private int _lastTick = -1;

        public SmoothPos3(Vec3Const startPos)
        {
            _endPos.copy(startPos);
        }

        public Vec3 GetPos(Vec3Const targetPos, int currentTick, double interpolation)
        {
            if (_lastTick != currentTick)// 只有tick变化时才更新目标位置,防止同一tick内多次调用时重复更新目标位置
            {
                _beginPos.copy(_endPos);
                _endPos.copy(targetPos);
                _dir.copy(_endPos);
                _dir.subSelf(_beginPos);
                _lastTick = currentTick;
            }
            _temp.copy(_dir);
            _temp.scaleSelf(interpolation);
            _temp.addSelf(_beginPos);
            return _temp;
        }
    }
}