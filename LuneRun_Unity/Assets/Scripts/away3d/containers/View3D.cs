namespace away3d.containers
{
    [System.Obsolete(View3D.Obsolete)]
    public class View3D
    {
        public uint backgroundColor;
        public uint antiAlias;
        public away3d.core.managers.Stage3DProxy stage3DProxy;
        
        public void Render() { }

        public const string Obsolete = @"
不使用这个类，其作用记录：
1. 在Game 类是 为了UI获取显示屏幕的宽高。
";
    }
}