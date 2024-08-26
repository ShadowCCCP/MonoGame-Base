using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MonoGame_Base.Project.Utility.Basics.Colliders
{
    public class CollisionEventArgs : EventArgs
    {
        public Collider collider { get; private set; }

        public CollisionEventArgs(Collider pCol)
        {
            collider = pCol;
        }
    }

    public delegate void CollisionEventHandler(object sender, CollisionEventArgs e);

    public abstract class Collider
    {
        public bool IsTrigger { get; private set; }
        public CollisionEventHandler OnCollide { get; set; }
        public CollisionEventHandler OnTrigger { get; set; }
        public GameObject GameObject { get; private set; }
        public Vector2 Position { get; private set; }

        private readonly HashSet<Collider> _checkedCollisions = new HashSet<Collider>();
        private readonly HashSet<Collider> _previouslyCollidingTriggers = new HashSet<Collider>();

        protected Collider(GameObject gameObject, bool isTrigger = false)
        {
            GameObject = gameObject;
            IsTrigger = isTrigger;
            Globals.OnGameClosed += OnUnload;
        }

        public abstract bool CheckCollisionWithBox(Collider other);
        public abstract void ResolveCollision(Collider other);

        public void RegisterCollision(Collider other)
        {
            _checkedCollisions.Add(other);
            other._checkedCollisions.Add(this);

            if (other.IsTrigger || IsTrigger)
            {
                _previouslyCollidingTriggers.Add(other);
            }
        }

        public void ClearCheckedCollisions()
        {
            _checkedCollisions.Clear();
        }

        public bool HasCheckedCollision(Collider other)
        {
            return _checkedCollisions.Contains(other);
        }

        public virtual void SetPosition(Vector2 position, bool updateGameObject = false)
        {
            Position = position;
            if (updateGameObject && GameObject != null)
            {
                GameObject.SetPosition(position);
            }
        }

        public void Activate()
        {
            CollisionManager.AddCollider(this);
        }

        public void Deactivate()
        {
            CollisionManager.RemoveCollider(this);
        }

        public abstract void Draw();

        private void OnUnload(object sender, EventArgs e)
        {
            Globals.OnGameClosed -= OnUnload;
        }
    }
}
