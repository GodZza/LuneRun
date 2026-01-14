namespace away3d.core.managers
{
    public class Context3D
    {
        public string driverInfo = "hardware";
    }
    
    public class Stage3DProxy
    {
        public Context3D context3D = new Context3D();
        public void addEventListener(string eventType, System.Action<away3d.events.Stage3DEvent> handler) { }
        public void removeEventListener(string eventType, System.Action<away3d.events.Stage3DEvent> handler) { }
        public void dispose() { }
    }
}