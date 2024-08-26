using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_Base.Project.Utility.Basics.Colliders;
using MonoGame_Base.Project.Utility.Basics.Sprites;
using MonoGame_Base.Project.Utility.Scenes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame_Base.Project.Utility.Basics
{
    public class GameObject
    {
        protected Sprite _sprite;
        protected bool _mirrorSprite = false;
        protected Vector2 _spriteScale = new Vector2(1.0f, 1.0f);

        public Vector2 _spriteSize { get; protected set; }

        private Vector2 _position;
        public Vector2 velocity { get; set; }

        public virtual Vector2 position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    Vector2 pos = HoldBoundaries(value);

                    _position = pos;
                    UpdatePosition();
                }
            }
        }

        private Collider _collider;
        public Collider collider
        {
            get { return _collider; }
            protected set
            {
                // Unsubscribe and subscribe to the OnCollide event if necessary...
                if (_collider != null && !_collider.IsTrigger) { _collider.OnCollide -= OnCollision; }
                else if (_collider != null) { _collider.OnTrigger -= OnTrigger; }

                _collider = value;

                if (_collider != null && !_collider.IsTrigger) { _collider.OnCollide += OnCollision; }
                else if (_collider != null) { _collider.OnTrigger += OnTrigger; }
            }
        }

        // This collider is used, to see if this object is standing on something...
        private Collider _bottomCollider;
        public Collider bottomCollider
        {
            get { return _bottomCollider; }
            protected set
            {
                // Unsubscribe and subscribe to the OnCollide event as necessary...
                if (_collider != null && !_collider.IsTrigger) { _collider.OnCollide -= OnCollision; }
                else if (_collider != null) { _collider.OnTrigger -= OnTrigger; }

                _bottomCollider = value;

                if (_collider != null && !_collider.IsTrigger) { _collider.OnCollide += OnCollision; }
                else if (_collider != null) { _collider.OnTrigger += OnTrigger; }
            }
        }

        protected float _bottomColliderHeight = 10;

        private GameObject _parent;
        public GameObject parent
        {
            get { return _parent; }
            private set
            {
                if (_parent != value && value != null)
                {
                    _parent = value;
                    offsetToParent = position - _parent.position;
                }
            }
        }
        private Vector2 offsetToParent;

        public List<GameObject> _children { get; private set; }

        public bool interactable { get; protected set; }

        public GameObject()
        {
            Globals.OnGameClosed += OnUnload;
            position = Vector2.Zero;
            parent = null;
            velocity = new Vector2();
            _children = new List<GameObject>();
        }

        public void SetParent(GameObject pParent)
        {
            parent = pParent;
            pParent._children.Add(this);
        }

        public void AddChild(GameObject pChild, bool pAddToScene = false)
        {
            pChild.SetParent(this);

            if (pAddToScene) { SceneManager.AddGameObject(pChild); }
        }

        public void RemoveChild(GameObject pChild, bool pDeleteObject = false)
        {
            pChild.parent = null;
            _children.Remove(pChild);

            if (pDeleteObject) { SceneManager.RemoveGameObject(pChild); }
        }

        public void RemoveAllChildren(bool pDeleteObject = false)
        {
            foreach (GameObject child in _children)
            {
                child.parent = null;
                _children.Remove(child);

                if (pDeleteObject) { SceneManager.RemoveGameObject(child); }
            }
        }

        public virtual void SetPosition(Vector2 pPos)
        {
            _position = pPos;
            UpdateColliderPosition();
        }

        public void UpdatePosition()
        {
            if (parent != null)
            {
                _position = parent._position + offsetToParent;
            }

            // Update Children...
            foreach (GameObject child in _children)
            {
                child.UpdatePosition();
            }

            UpdateColliderPosition();
        }

        private Vector2 HoldBoundaries(Vector2 pCurrentPos)
        {
            float x = pCurrentPos.X;
            float y = pCurrentPos.Y;

            if (Globals.CurrentMap != null)
            {
                x = MathHelper.Clamp(pCurrentPos.X, 0, Globals.CurrentMap.GetMapSize().X - _spriteSize.X);
                y = MathHelper.Clamp(pCurrentPos.Y, 0, Globals.CurrentMap.GetMapSize().Y - _spriteSize.Y);
            }

            return new Vector2(x, y);
        }

        // Not making this abstract, as I don't want to have to override Update, Draw and OnCollision for every gameObject...
        public virtual void Update() {
            
        }
        protected virtual void OnCollision(object sender, CollisionEventArgs e) { }
        protected virtual void OnTrigger(object sender, CollisionEventArgs e) { }
        protected virtual void UpdateColliderPosition()
        {
            if (collider != null) { collider.SetPosition(new Vector2(position.X, position.Y)); }
            if (bottomCollider != null)
            {
                bottomCollider.SetPosition(new Vector2(position.X, position.Y + _spriteSize.Y - _bottomColliderHeight));
            }
        }
        public virtual void Draw()
        {
            if (_sprite != null)
            {
                float layerDepth = MathHelper.Clamp(position.Y / Globals.CurrentMap.GetMapSize().Y, 0.1f, 1);
                if (_mirrorSprite) { _sprite.Draw(position, _spriteScale, Vector2.Zero, Color.White, 0, SpriteEffects.FlipHorizontally, layerDepth); }
                else { _sprite.Draw(position, _spriteScale, Vector2.Zero, Color.White, 0, SpriteEffects.None, layerDepth); }
            }
        }

        public virtual void ActivateCollider()
        {
            if (collider != null) { collider.Activate(); }
            if (bottomCollider != null) { bottomCollider.Activate(); }
        }

        public virtual void DeactivateCollider()
        {
            if (collider != null) { collider.Deactivate(); }
            if (bottomCollider != null) { bottomCollider.Deactivate(); }
        }

        protected virtual void OnUnload(object sender, EventArgs e)
        {
            // Remove and unsubscribe collider...
            collider = null;
            Globals.OnGameClosed -= OnUnload;
        }
    }
}
