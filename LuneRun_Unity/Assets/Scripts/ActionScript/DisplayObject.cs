using System.Collections.Generic;
using UnityEngine;

namespace ActionScript
{
    /// <summary>
    /// Mimics Flash DisplayObject for display list operations.
    /// </summary>
    public class DisplayObject : EventDispatcher
    {
        protected GameObject _gameObject;
        protected Transform _transform;
        protected DisplayObject _parent;
        protected readonly List<DisplayObject> _children = new();
        
        public float x
        {
            get => _transform.localPosition.x;
            set => _transform.localPosition = new Vector3(value, _transform.localPosition.y, _transform.localPosition.z);
        }
        
        public float y
        {
            get => _transform.localPosition.y;
            set => _transform.localPosition = new Vector3(_transform.localPosition.x, value, _transform.localPosition.z);
        }
        
        public float width
        {
            get => GetWidth();
            set => SetWidth(value);
        }
        
        public float height
        {
            get => GetHeight();
            set => SetHeight(value);
        }
        
        public float scaleX
        {
            get => _transform.localScale.x;
            set => _transform.localScale = new Vector3(value, _transform.localScale.y, _transform.localScale.z);
        }
        
        public float scaleY
        {
            get => _transform.localScale.y;
            set => _transform.localScale = new Vector3(_transform.localScale.x, value, _transform.localScale.z);
        }
        
        public float rotation
        {
            get => _transform.localEulerAngles.z;
            set => _transform.localEulerAngles = new Vector3(_transform.localEulerAngles.x, _transform.localEulerAngles.y, value);
        }
        
        public DisplayObject parent => _parent;
        public Stage stage => GetStage();
        public GameObject gameObject => _gameObject;
        public Transform transform => _transform;
        
        public DisplayObject(GameObject go = null)
        {
            if (go == null)
                go = new GameObject(GetType().Name);
            
            _gameObject = go;
            _transform = go.transform;
        }
        
        public virtual DisplayObject AddChild(DisplayObject child)
        {
            if (child == null || child._parent == this)
                return child;
            
            // Remove from previous parent
            child._parent?.RemoveChild(child);
            
            // Add to this parent
            child._transform.SetParent(_transform, false);
            child._parent = this;
            _children.Add(child);
            
            // Dispatch added event
            child.DispatchEvent(Event.ADDED_TO_STAGE, this);
            
            return child;
        }
        
        public virtual void RemoveChild(DisplayObject child)
        {
            if (child == null || child._parent != this)
                return;
            
            _children.Remove(child);
            child._transform.SetParent(null, false);
            child._parent = null;
            
            // Dispatch removed event
            child.DispatchEvent(Event.REMOVED_FROM_STAGE, this);
        }
        
        public virtual DisplayObject GetChildAt(int index)
        {
            if (index < 0 || index >= _children.Count)
                return null;
            return _children[index];
        }
        
        public virtual void RemoveChildAt(int index)
        {
            if (index < 0 || index >= _children.Count)
                return;
            RemoveChild(_children[index]);
        }
        
        public virtual int NumChildren => _children.Count;
        
        protected virtual float GetWidth()
        {
            // Default implementation returns scale x
            return _transform.localScale.x;
        }
        
        protected virtual void SetWidth(float value)
        {
            // Default implementation adjusts scale x
            _transform.localScale = new Vector3(value, _transform.localScale.y, _transform.localScale.z);
        }
        
        protected virtual float GetHeight()
        {
            // Default implementation returns scale y
            return _transform.localScale.y;
        }
        
        protected virtual void SetHeight(float value)
        {
            // Default implementation adjusts scale y
            _transform.localScale = new Vector3(_transform.localScale.x, value, _transform.localScale.z);
        }
        
        protected virtual Stage GetStage()
        {
            if (this is Stage s)
                return s;
            
            DisplayObject current = this;
            while (current._parent != null)
            {
                current = current._parent;
                if (current is Stage stage)
                    return stage;
            }
            return null;
        }
        
        public virtual void Destroy()
        {
            // Remove all children
            while (_children.Count > 0)
            {
                RemoveChildAt(_children.Count - 1);
            }
            
            // Destroy GameObject
            if (_gameObject != null)
                Object.Destroy(_gameObject);
        }
    }
}