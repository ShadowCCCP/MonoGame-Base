using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGame_Base.Project.Utility.Basics.Sprites.Animations
{
    public class AnimationManager
    {
        private readonly Dictionary<object, AnimationSprite> _anims = new Dictionary<object, AnimationSprite>();
        private object _lastKey;
        public bool _stopAnimation { get; private set; }

        // Using this to start the first animation frame at least, in case it's stopped right away...
        private bool startedAnimation;

        public void AddAnimation(object pKey, AnimationSprite pAnimation)
        {
            _anims.Add(pKey, pAnimation);

            // Assign added key to lastKey, only in case it's null...
            _lastKey ??= pKey;
        }

        public void Update(object pKey)
        {
            if (!pKey.Equals(_lastKey))
            {
                _anims[_lastKey].Reset();
                startedAnimation = false;
            }

            if (_anims.ContainsKey(pKey) && (!_stopAnimation || !startedAnimation))
            {
                _anims[pKey].Start();
                _anims[pKey].Update();
                _lastKey = pKey;

                if (!startedAnimation) { startedAnimation = true; }
            }
            else if (!_stopAnimation)
            {
                _anims[_lastKey].Stop();
                _anims[_lastKey].Reset();
            }
        }

        public void SetAnimationPlay(bool pState)
        {
            _stopAnimation = pState;
        }

        public Point GetAnimFrameSize(object pKey)
        {
            Rectangle rect = _anims[pKey].GetFrameRect(0);

            return new Point(rect.Width, rect.Height);
        }

        public int GetCurrentAnimFrame()
        {
            return _anims[_lastKey].GetCurrentFrame();
        }

        public void Reset(object pKey)
        {
            _anims[pKey].Stop();
            _anims[pKey].Reset();
        }

        public void Draw(Vector2 pPos, Vector2 pScale, Vector2 pOrigin, Color pColor,
            float pRotation = 0, SpriteEffects pSpriteEffect = SpriteEffects.None, float pLayerDepth = 1)
        {
            _anims[_lastKey].Draw(pPos, pScale, pOrigin, pColor, pRotation, pSpriteEffect, pLayerDepth);
        }
    }
}
