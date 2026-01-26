using UnityEngine;
using com.playchilla.runner;

namespace com.playchilla.runner.track.entity
{
    public class SpeedView : EntityView
    {
        private GameObject visualObject;
        private Level level;
        
        // Unity-compatible constructor (no parameters)
        public SpeedView() : base()
        {
            // Initialize in Awake or via Initialize method
        }
        
        public void Initialize(RunnerEntity entity, Level levelRef)
        {
            level = levelRef;
            base.Initialize(entity);
            CreateVisual();
        }
        
        public override void Initialize(RunnerEntity entity)
        {
            base.Initialize(entity);
            CreateVisual();
        }
        
        private void CreateVisual()
        {
            visualObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visualObject.transform.SetParent(transform);
            visualObject.transform.localPosition = Vector3.zero;
            visualObject.transform.localScale = new Vector3(2f, 2f, 2f); // Scale based on entity radius
            // Set color to yellow (speed entity)
            Renderer renderer = visualObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.yellow;
            }
        }
        
        private void OnDestroy()
        {
            if (visualObject != null)
            {
                Destroy(visualObject);
            }
        }
    }
}