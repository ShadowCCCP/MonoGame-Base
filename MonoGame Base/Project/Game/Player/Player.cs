using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame_Base.Project.Utility.Basics.Colliders;
using MonoGame_Base.Project.Utility.Basics;
using System;
using MonoGame_Base.Project.Utility.Basics.Sprites;
using MonoGame_Base.Project.Utility.Basics.Colliders.Shapes;
using System.Diagnostics;

namespace MonoGame_Base.Project.Game.Player
{
    public class Player : GameObject
    {
        public override Vector2 position
        {
            get { return base.position; }
            set
            {
                base.position = value;
                Globals.PlayerPosition = value;
            }
        }

        private float _speed = 200.0f;
        private float _sprintSpeed = 400.0f;
        private bool _moving;

        public Player(Vector2 pPosition)
        {
            position = pPosition;
            _spriteScale = new Vector2(3.5f, 3.5f);

            Texture2D playerTexture = Globals.Content.Load<Texture2D>("Sprites/Player/Player");
            _sprite = new Sprite(playerTexture);

            // Set Collider...
            _spriteSize = new Vector2(playerTexture.Width, playerTexture.Height) * _spriteScale;
            collider = new BoxCollider(position, _spriteSize, this);
        }

        public override void Update()
        {
            base.Update();
            CheckInput();
        }

        private void CheckInput()
        {
            if (InputManager.Moving)
            {
                _moving = true;
                // Check if sprinting...
                float speed;
                if (InputManager.Sprint) { speed = _sprintSpeed; }
                else { speed = _speed; }

                // Update player position...
                velocity = Vector2.Normalize(InputManager.Direction) * speed * Globals.TotalSeconds;

                SetPosition(position + velocity);
            }
            else if (_moving) { _moving = false; }
        }

        private void MirrorSpriteTowardsPosition(Vector2 pPos)
        {
            // Check in what direction the interaction is taking place...
            Vector2 distanceVector = pPos - position;
            Vector2 right = new Vector2(1, 0);
            float dotProduct = Vector2.Dot(distanceVector, right);

            // Mirror the player sprite if interaction is on the left...
            if (dotProduct > 0) { _mirrorSprite = false; }
            else { _mirrorSprite = true; }
        }

        public override void SetPosition(Vector2 pPos)
        {
            position = pPos;
        }
    }
}
