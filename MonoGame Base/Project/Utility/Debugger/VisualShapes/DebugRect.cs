using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_Base.Project.Utility.Basics;
using MonoGame_Base.Project.Utility.Basics.Sprites;
using System.Collections.Generic;

namespace MonoGame_Base.Project.Utility.Debugger.VisualShapes
{
    public class DebugRect : IDebug
    {
        private static Dictionary<Color, Texture2D> _debugTextures = new Dictionary<Color, Texture2D>();
        private Texture2D _currentTexture;

        private Vector2 _position;
        private Vector2 _size;

        public DebugRect(Vector2 position, Vector2 size)
        {
            _position = position;
            _size = size;
            _currentTexture = GetColoredTexture();
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public void SetSize(Vector2 size)
        {
            _size = size;
        }

        private Texture2D GetColoredTexture()
        {
            Color color = Globals.GetRandomColor();
            if (!_debugTextures.ContainsKey(color))
            {
                Texture2D texture = new Texture2D(Globals.GraphicsDevice, 1, 1);
                texture.SetData(new[] { color });
                _debugTextures.Add(color, texture);
            }
            return _debugTextures[color];
        }

        public void Draw()
        {
            Globals.SpriteBatch.Draw(_currentTexture, _position, null, Color.White * 0.35f, 0f, Vector2.Zero, _size, SpriteEffects.None, 1f);
        }
    }
}
