using Microsoft.Xna.Framework;
using MonoGame_Base.Project.Utility.Debugger.VisualShapes;
using System;

namespace MonoGame_Base.Project.Utility.Basics.Colliders.Shapes
{
    public class CircleCollider : Collider
    {
        public float Radius { get; private set; }
        private DebugCircle _debugCircle;

        public CircleCollider(Vector2 position, float radius, GameObject attachedObject, bool isTrigger = false)
            : base(attachedObject, isTrigger)
        {
            SetPosition(position);
            SetRadius(radius);
            _debugCircle = new DebugCircle(position, radius);
        }

        public override bool CheckCollisionWithBox(Collider other)
        {
            if (other is CircleCollider circleCollider)
            {
                return CheckCollisionWithCircle(circleCollider);
            }
            else if (other is BoxCollider boxCollider)
            {
                return boxCollider.CheckCollisionWithBox(this);
            }
            return false;
        }

        private bool CheckCollisionWithCircle(CircleCollider other)
        {
            // Offset the positions by the radius during the collision check
            Vector2 thisPositionOffset = this.Position + new Vector2(this.Radius, this.Radius);
            Vector2 otherPositionOffset = other.Position + new Vector2(other.Radius, other.Radius);

            float distance = Vector2.Distance(thisPositionOffset, otherPositionOffset);
            return distance < (this.Radius + other.Radius);
        }

        public override void ResolveCollision(Collider other)
        {
            if (other is CircleCollider circleCollider)
            {
                ResolveCollisionWithCircle(circleCollider);
            }
            else if (other is BoxCollider boxCollider)
            {
                ResolveColliderWithBox(boxCollider);
            }
        }

        private void ResolveCollisionWithCircle(CircleCollider other)
        {
            // Offset the positions by the radius during the collision resolution
            Vector2 thisPositionOffset = this.Position + new Vector2(this.Radius, this.Radius);
            Vector2 otherPositionOffset = other.Position + new Vector2(other.Radius, other.Radius);

            Vector2 direction = thisPositionOffset - otherPositionOffset;
            float distance = direction.Length();

            if (distance < this.Radius + other.Radius)
            {
                Vector2 correction = direction * ((this.Radius + other.Radius - distance) / distance);
                this.SetPosition(this.Position + correction, true);

                // Adjust the velocity based on the direction of the correction
                if (Math.Abs(correction.X) > Math.Abs(correction.Y))
                {
                    this.GameObject.velocity = new Vector2(0, this.GameObject.velocity.Y);
                }
                else
                {
                    this.GameObject.velocity = new Vector2(this.GameObject.velocity.X, 0);
                }
            }
        }

        // New method to resolve collision with a BoxCollider
        private void ResolveColliderWithBox(BoxCollider box)
        {
            // Offset the circle's position by the radius during the collision resolution
            Vector2 circlePositionOffset = Position + new Vector2(Radius, Radius);

            // Find the closest point on the box to the circle's center
            Vector2 closestPoint = new Vector2(
                MathHelper.Clamp(circlePositionOffset.X, box.Bounds.Left, box.Bounds.Right),
                MathHelper.Clamp(circlePositionOffset.Y, box.Bounds.Top, box.Bounds.Bottom)
            );

            // Calculate the distance from the circle's center to this closest point
            Vector2 circleToBox = circlePositionOffset - closestPoint;
            float distance = circleToBox.Length();

            if (distance < Radius)
            {
                // Determine how much to move the circle back to resolve the collision
                Vector2 correction = circleToBox * ((Radius - distance) / distance);

                // Push the box away if it's not static
                if (box.GameObject != null && !box.GameObject.velocity.Equals(Vector2.Zero))
                {
                    box.GameObject.SetPosition(box.GameObject.position + correction);
                }

                // Push the circle away as well
                SetPosition(Position - correction, true);

                // Adjust the velocity based on the direction of the correction
                if (Math.Abs(correction.X) > Math.Abs(correction.Y))
                {
                    GameObject.velocity = new Vector2(0, GameObject.velocity.Y);
                }
                else
                {
                    GameObject.velocity = new Vector2(GameObject.velocity.X, 0);
                }
            }
        }

        public void SetRadius(float radius)
        {
            Radius = radius;
            _debugCircle?.SetRadius(radius);
        }

        public override void SetPosition(Vector2 position, bool updateGameObject = false)
        {
            base.SetPosition(position, updateGameObject);
            _debugCircle?.SetPosition(position);
        }

        public override void Draw()
        {
            if (Globals.Debugging)
            {
                _debugCircle.Draw();
            }
        }
    }
}
