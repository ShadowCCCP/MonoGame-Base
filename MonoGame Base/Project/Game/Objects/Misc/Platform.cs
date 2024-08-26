using Microsoft.Xna.Framework.Graphics;
using MonoGame_Base.Project.Utility.Basics.Colliders.Shapes;
using MonoGame_Base.Project.Utility.Basics.Colliders;
using MonoGame_Base.Project.Utility.Basics;
using Microsoft.Xna.Framework;
using MonoGame_Base.Project.Utility.Basics.Sprites;
using System.Diagnostics;
using System;

namespace MonoGame_Base.Project.Game.Objects.Misc
{
    public class Platform : GameObject
    {
        private bool _active;

        private Sprite _platformLight;

        private float _lightAlpha;
        private float _lightUpSpeed = 0.01f;

        private Timer _timer;

        public Platform(Vector2 pPosition)
        {
            position = pPosition;
            _spriteScale = new Vector2(10.0f, 10.0f);

            Texture2D platformTexture = Globals.Content.Load<Texture2D>("Sprites/Objects/Misc/Platform/Platform");
            Texture2D platformLightTexture = Globals.Content.Load<Texture2D>("Sprites/Objects/Misc/Platform/PlatformLight");
            _sprite = new Sprite(platformTexture);
            _platformLight = new Sprite(platformLightTexture);

            _timer = new Timer(0.1f, false);

            // Set Collider...
            _spriteSize = new Vector2(platformTexture.Width, platformTexture.Height) * _spriteScale;
            collider = new BoxCollider(position, _spriteSize, this, true);

            _timer.OnTimerFinished += DeactivateLight;
        }

        public override void Update()
        {
            base.Update();

            DecreaseLightOpacity();
            _timer.Update();
        }

        public override void SetPosition(Vector2 pPos)
        {
            position = pPos;
        }

        protected override void OnTrigger(object sender, CollisionEventArgs e)
        {
            IncreaseLightOpacity();
        }

        private void DeactivateLight(object sender, EventArgs e)
        {
            _active = false;
        }

        private void IncreaseLightOpacity()
        {
            // Set it to active and reset the deactivate timer...
            _active = true;
            _timer.RestartTimer();

            if (_lightAlpha != 1)
            {
                _lightAlpha = MathHelper.Clamp(_lightAlpha +_lightUpSpeed, 0.0f, 1.0f);
            }
        }

        private void DecreaseLightOpacity()
        {
            if (_lightAlpha != 0 && !_active)
            {
                _lightAlpha = MathHelper.Clamp(_lightAlpha - _lightUpSpeed, 0.0f, 1.0f);
            }
        }

        protected override void OnUnload(object sender, EventArgs e)
        {
            base.OnUnload(sender, e);

            _timer.OnTimerFinished -= DeactivateLight;
        }

        public override void Draw()
        {
            base.Draw();

            float layerDepth = MathHelper.Clamp(position.Y / Globals.CurrentMap.GetMapSize().Y, 0.1f, 1);
            _platformLight.Draw(position, _spriteScale, Vector2.Zero, Color.White * _lightAlpha, 0, SpriteEffects.FlipHorizontally, layerDepth + 0.001f);
        }
    }
}
