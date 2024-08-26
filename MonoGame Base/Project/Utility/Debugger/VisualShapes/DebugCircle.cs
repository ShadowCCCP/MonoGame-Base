using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_Base.Project.Utility.Basics;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame_Base.Project.Utility.Debugger.VisualShapes
{
    public class DebugCircle : IDebug
    {
        private static Dictionary<Color, Texture2D> _debugTextures = new Dictionary<Color, Texture2D>();
        private Texture2D _circleTexture;

        private Vector2 _position;
        private float _radius;

        public DebugCircle(Vector2 position, float radius)
        {
            _position = position;
            _radius = radius;
            _circleTexture = GetColoredCircleTexture();
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public void SetRadius(float radius)
        {
            _radius = radius;
        }

        private Texture2D GetColoredCircleTexture()
        {
            Color color = Globals.GetRandomColor();
            if (!_debugTextures.ContainsKey(color))
            {
                int diameter = 64; // Small but sufficient for a smooth circle
                Texture2D texture = new Texture2D(Globals.GraphicsDevice, diameter, diameter);
                Color[] data = new Color[diameter * diameter];

                int radius = diameter / 2;
                Vector2 center = new Vector2(radius, radius);

                for (int y = 0; y < diameter; y++)
                {
                    for (int x = 0; x < diameter; x++)
                    {
                        Vector2 diff = new Vector2(x, y) - center;
                        if (diff.LengthSquared() <= radius * radius)
                        {
                            data[y * diameter + x] = color;
                        }
                        else
                        {
                            data[y * diameter + x] = Color.Transparent;
                        }
                    }
                }

                texture.SetData(data);
                _debugTextures.Add(color, texture);
            }
            return _debugTextures[color];
        }

        public void Draw()
        {
            // Scale the circle to the correct size
            Vector2 scale = new Vector2(_radius * 2f / _circleTexture.Width);
            Vector2 drawPosition = _position;
            Globals.SpriteBatch.Draw(_circleTexture, drawPosition, null, Color.White * 0.35f, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        }
    }
}
