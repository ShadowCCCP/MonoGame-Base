using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGame_Base.Project.Utility.Basics.Sprites
{
    public class Sprite
    {
        protected Texture2D _texture;

        public Sprite(Texture2D pTexture)
        {
            _texture = pTexture;
        }

        public void SetTexture(Texture2D pTexture)
        {
            _texture = pTexture;
        }

        public virtual void Draw(Vector2 pPos, Vector2 pScale, Vector2 pOrigin, Color pColor,
            float pRotation = 0, SpriteEffects pSpriteEffect = SpriteEffects.None, float pLayerDepth = 1)
        {
            Globals.SpriteBatch.Draw(_texture, pPos, null, pColor, pRotation, pOrigin, pScale, pSpriteEffect, pLayerDepth);
        }

        public static List<Rectangle> GetMergedBoundingBoxes(Texture2D texture)
        {
            // Get the pixel data from the texture
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);

            // List to store the resulting rectangles
            List<Rectangle> rectangles = new List<Rectangle>();

            // Temp list to store the rectangles being formed on the current row
            List<Rectangle> currentRowRectangles = new List<Rectangle>();

            for (int y = 0; y < texture.Height; y++)
            {
                currentRowRectangles.Clear();

                for (int x = 0; x < texture.Width; x++)
                {
                    Color pixel = colorData[y * texture.Width + x];

                    if (pixel.A > 0) // Non-transparent pixel
                    {
                        if (currentRowRectangles.Count == 0 || x != currentRowRectangles[^1].Right)
                        {
                            // Start a new rectangle
                            currentRowRectangles.Add(new Rectangle(x, y, 1, 1));
                        }
                        else
                        {
                            // Extend the current rectangle horizontally
                            Rectangle lastRect = currentRowRectangles[^1];
                            lastRect.Width++;
                            currentRowRectangles[^1] = lastRect;
                        }
                    }
                }

                // Attempt to merge current row rectangles with previous row's rectangles
                for (int i = 0; i < currentRowRectangles.Count; i++)
                {
                    Rectangle currentRect = currentRowRectangles[i];
                    bool merged = false;

                    // Check if it can be merged with any rectangle from the previous row
                    for (int j = 0; j < rectangles.Count; j++)
                    {
                        Rectangle previousRect = rectangles[j];
                        if (previousRect.X == currentRect.X &&
                            previousRect.Width == currentRect.Width &&
                            previousRect.Y + previousRect.Height == currentRect.Y)
                        {
                            // Merge the rectangles vertically
                            previousRect.Height++;
                            rectangles[j] = previousRect;
                            merged = true;
                            break;
                        }
                    }

                    if (!merged)
                    {
                        rectangles.Add(currentRect);
                    }
                }
            }

            return rectangles;
        }
    }
}
