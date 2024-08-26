using Microsoft.Xna.Framework.Graphics;
using MonoGame_Base.Project.Utility.Basics.Colliders.Shapes;
using MonoGame_Base.Project.Utility.Basics.Colliders;
using MonoGame_Base.Project.Utility.Basics;
using System;
using Microsoft.Xna.Framework;
using MonoGame_Base.Project.Utility.Basics.Sprites;
using System.Diagnostics;

namespace MonoGame_Base.Project.Game.Objects.Misc
{
    public class SmileyBall : GameObject
    {
        private float _travelDistance = 500.0f;
        private float _crossedDistance;

        private float _speed = 100.0f;

        private Vector2 _direction;

        public SmileyBall(Vector2 pPosition)
        {
            position = pPosition;
            _spriteScale = new Vector2(3.5f, 3.5f);

            _crossedDistance = _travelDistance / 2;

            _direction = new Vector2(1, 0);
            _direction.Normalize();

            Texture2D smileyBallTexture = Globals.Content.Load<Texture2D>("Sprites/Objects/Misc/SmileyBall/SmileyBall");
            _sprite = new Sprite(smileyBallTexture);

            // Set Collider...
            _spriteSize = new Vector2(smileyBallTexture.Width, smileyBallTexture.Height) * _spriteScale;
            collider = new CircleCollider(position, smileyBallTexture.Width / 2 * _spriteScale.X, this);
        }

        public override void Update()
        {
            base.Update();
            Move();
        }

        private void Move()
        {
            velocity = _direction * _speed * Globals.TotalSeconds;

            position += velocity;
            _crossedDistance += velocity.Length();

            SwitchDirection();
        }

        private void SwitchDirection()
        {
            if (_crossedDistance >= _travelDistance)
            {
                _crossedDistance = 0;
                _direction *= -1;
            }
        }

        public override void SetPosition(Vector2 pPos)
        {
            position = pPos;
        }

        protected override void OnCollision(object sender, CollisionEventArgs e)
        {

        }
    }
}
