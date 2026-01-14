using UnityEngine;

namespace com.playchilla.runner
{
    public class Main : MonoBehaviour
    {
        void Awake()
        {
            // Create the Engine GameObject
            GameObject engineObject = new GameObject("Engine");
            engineObject.AddComponent<Engine>();
            DontDestroyOnLoad(engineObject);
            
            Debug.Log("Main created Engine instance");
        }
    }
}