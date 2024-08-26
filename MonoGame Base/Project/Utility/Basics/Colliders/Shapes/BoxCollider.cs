using Microsoft.Xna.Framework;
using MonoGame_Base.Project.Utility.Debugger.VisualShapes;
using System;
using System.Diagnostics;

namespace MonoGame_Base.Project.Utility.Basics.Colliders.Shapes
{
    public class BoxCollider : Collider
    {
        public Rectangle Bounds { get; private set; }
        private DebugRect _debugRect;

        public BoxCollider(Vector2 position, Vector2 size, GameObject attachedObject, bool isTrigger = false)
            : base(attachedObject, isTrigger)
        {
            SetPosition(position);
            SetSize(size);
            _debugRect = new DebugRect(position, size);
        }

        public override bool CheckCollisionWithBox(Collider other)
        {
            if (other is BoxCollider boxCollider)
            {
                return Bounds.Intersects(boxCollider.Bounds);
            }
            else if (other is CircleCollider circleCollider)
            {
                return CheckCollisionWithCircle(circleCollider);
            }
            return false;
        }

        private bool CheckCollisionWithCircle(CircleCollider circle)
        {
            // Offset the circle's position by the radius during the collision check
            Vector2 circlePositionOffset = circle.Position + new Vector2(circle.Radius, circle.Radius);

            Vector2 closestPoint = new Vector2(
                MathHelper.Clamp(circlePositionOffset.X, Bounds.Left, Bounds.Right),
                MathHelper.Clamp(circlePositionOffset.Y, Bounds.Top, Bounds.Bottom)
            );

            float distance = Vector2.Distance(circlePositionOffset, closestPoint);
            return distance < circle.Radius;
        }

        public override void ResolveCollision(Collider other)
        {
            if (GameObject == null) return;

            if (other is BoxCollider boxCollider)
            {
                ResolveCollisionWithBox(boxCollider);
            }
            else if (other is CircleCollider circleCollider)
            {
                ResolveCollisionWithCircle(circleCollider);
            }
        }

        private void ResolveCollisionWithBox(BoxCollider other)
        {
            Vector2 depth = GetIntersectionDepth(Bounds, other.Bounds);

            if (depth != Vector2.Zero)
            {
                if (Math.Abs(depth.X) < Math.Abs(depth.Y))
                {
                    GameObject.SetPosition(new Vector2(GameObject.position.X + depth.X, GameObject.position.Y));
                    GameObject.velocity = new Vector2(0, GameObject.velocity.Y);
                }
                else
                {
                    GameObject.SetPosition(new Vector2(GameObject.position.X, GameObject.position.Y + depth.Y));
                    GameObject.velocity = new Vector2(GameObject.velocity.X, 0);
                }
            }
        }

        private void ResolveCollisionWithCircle(CircleCollider circle)
        {
            // Offset the circle's position by the radius during the collision resolution
            Vector2 circlePositionOffset = circle.Position + new Vector2(circle.Radius, circle.Radius);

            Vector2 closestPoint = new Vector2(
                MathHelper.Clamp(circlePositionOffset.X, Bounds.Left, Bounds.Right),
                MathHelper.Clamp(circlePositionOffset.Y, Bounds.Top, Bounds.Bottom)
            );

            Vector2 circleToBox = closestPoint - circlePositionOffset;
            float distance = circleToBox.Length();

            if (distance < circle.Radius)
            {
                Vector2 correction = circleToBox * ((circle.Radius - distance) / distance);
                circle.SetPosition(circle.Position - correction, true);

                if (Math.Abs(correction.X) > Math.Abs(correction.Y))
                {
                    circle.GameObject.velocity = new Vector2(0, circle.GameObject.velocity.Y);
                }
                else
                {
                    circle.GameObject.velocity = new Vector2(circle.GameObject.velocity.X, 0);
                }
            }
        }

        private Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        public void SetSize(Vector2 size)
        {
            Bounds = new Rectangle(Bounds.Location, size.ToPoint());
            _debugRect?.SetSize(size);
        }

        public override void SetPosition(Vector2 position, bool updateGameObject = false)
        {
            base.SetPosition(position, updateGameObject);
            Bounds = new Rectangle(position.ToPoint(), Bounds.Size);
            _debugRect?.SetPosition(position);
        }

        public override void Draw()
        {
            if (Globals.Debugging)
            {
                _debugRect.Draw();
            }
        }
    }
}
