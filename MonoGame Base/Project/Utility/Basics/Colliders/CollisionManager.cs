using MonoGame_Base.Project.Utility.Basics.Colliders.Shapes;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame_Base.Project.Utility.Basics.Colliders
{
    public static class CollisionManager
    {
        private static readonly HashSet<Collider> _colliders = new HashSet<Collider>();

        public static void CheckCollisions()
        {
            foreach (Collider collider in _colliders)
            {
                foreach (Collider other in _colliders)
                {
                    if (collider == other || collider.HasCheckedCollision(other))
                        continue;

                    bool isCollision = collider.CheckCollisionWithBox(other);

                    if (isCollision)
                    {
                        if (collider.IsTrigger || other.IsTrigger)
                        {
                            HandleTriggerCollision(collider, other);
                        }
                        else
                        {
                            HandleNormalCollision(collider, other);
                            ResolveCollision(collider, other);
                        }
                    }
                }

                collider.ClearCheckedCollisions();
            }
        }

        private static void HandleNormalCollision(Collider collider, Collider other)
        {
            if (collider is BoxCollider boxCollider && other is BoxCollider otherBoxCollider)
            {
                boxCollider.ResolveCollision(otherBoxCollider);
            }

            collider.OnCollide?.Invoke(collider, new CollisionEventArgs(other));
            other.OnCollide?.Invoke(other, new CollisionEventArgs(collider));
        }

        private static void HandleTriggerCollision(Collider collider, Collider other)
        {
            collider.OnTrigger?.Invoke(collider, new CollisionEventArgs(other));
            other.OnTrigger?.Invoke(other, new CollisionEventArgs(collider));
        }

        private static void ResolveCollision(Collider collider, Collider other)
        {
            if (collider is BoxCollider boxCollider)
            {
                if (other is BoxCollider otherBoxCollider)
                {
                    boxCollider.ResolveCollision(otherBoxCollider);
                }
                else if (other is CircleCollider circleCollider)
                {
                    boxCollider.ResolveCollision(circleCollider);
                }
            }
            else if (collider is CircleCollider circleCollider && other is BoxCollider otherBoxCollider)
            {
                otherBoxCollider.ResolveCollision(circleCollider);
            }
            // Add more conditions here if you have other types of colliders
        }

        public static void AddCollider(Collider collider)
        {
            _colliders.Add(collider);
        }

        public static void RemoveCollider(Collider collider)
        {
            _colliders.Remove(collider);
        }

        public static void DrawColliders()
        {
            foreach (Collider collider in _colliders)
            {
                collider.Draw();
            }
        }
    }
}
