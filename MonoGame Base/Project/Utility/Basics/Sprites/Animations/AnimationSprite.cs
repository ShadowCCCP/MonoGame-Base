using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame_Base.Project.Utility.Basics.Sprites.Animations
{
    public class AnimationSprite : Sprite
    {
        private readonly List<Rectangle> _sourceRectangles = new List<Rectangle>();
        private readonly int _frames;
        private int _frame;
        private readonly float _frameTime;
        private float _frameTimeLeft;
        private bool _active = true;

        private readonly bool _playOnce;
        private bool _finishedOnce;

        public AnimationSprite(Texture2D pTexture, int pTotalRows, int pTotalColumns, float pFrameTime, int pStartRow = 1, int pStartColumn = 1, int pFrameCount = 1, bool pPlayOnce = false) : base(pTexture)
        {
            _texture = pTexture;
            _frameTime = pFrameTime;
            _frameTimeLeft = _frameTime;
            _frames = pFrameCount;
            _playOnce = pPlayOnce;

            int _frameWidth = _texture.Width / pTotalColumns;
            int _frameHeight = _texture.Height / pTotalRows;

            int currentRow = pStartRow;
            int currentColumn = pStartColumn;

            for (int i = 0; i < _frames; i++)
            {
                if (currentColumn > pTotalColumns)
                {
                    currentRow++;
                    currentColumn = 1;
                }

                if (currentRow > pTotalRows)
                {
                    Debug.WriteLine("Animation: Unreachable row.");
                    return;
                }

                _sourceRectangles.Add(new Rectangle((currentColumn - 1) * _frameWidth, (currentRow - 1) * _frameHeight, _frameWidth, _frameHeight));
                currentColumn++;
            }
        }

        public void Stop()
        {
            _active = false;
        }

        public void Start()
        {
            _active = true;
        }

        public void Reset()
        {
            _frame = 0;
            _frameTimeLeft = _frameTime;
            _finishedOnce = false;
        }

        public void Update()
        {
            if (!_active || _finishedOnce) return;

            _frameTimeLeft -= Globals.TotalSeconds;

            if (_frameTimeLeft <= 0)
            {
                _frameTimeLeft += _frameTime;

                if (!_playOnce) { _frame = (_frame + 1) % _frames; }
                else
                {
                    _frame += 1;
                    if (_frame % _frames == 0)
                    {
                        _frame = _frames - 1;
                        _finishedOnce = true;
                    }
                }
            }
        }

        public int GetCurrentFrame()
        {
            return _frame;
        }

        public Rectangle GetFrameRect(int frame)
        {
            if (frame > -1 && frame < _sourceRectangles.Count)
            {
                return _sourceRectangles[0];
            }
            else { Debug.WriteLine("Animation: Selected frame doesn't exist..."); }

            return new Rectangle();
        }

        private Texture2D GetTextureOfFrame(Rectangle pArea)
        {
            // Create a new texture to hold the parsed area...
            Texture2D newTexture = new Texture2D(_texture.GraphicsDevice, pArea.Width, pArea.Height);

            // Get the color data from the source texture...
            Color[] sourceColorData = new Color[_texture.Width * _texture.Height];
            _texture.GetData(sourceColorData);

            // Extract the area from the source color data...
            Color[] newColorData = new Color[pArea.Width * pArea.Height];
            int index = 0;
            for (int y = pArea.Y; y < pArea.Y + pArea.Height; y++)
            {
                for (int x = pArea.X; x < pArea.X + pArea.Width; x++)
                {
                    newColorData[index++] = sourceColorData[y * _texture.Width + x];
                }
            }

            // Set the color data to the parsed texture...
            newTexture.SetData(newColorData);

            return newTexture;
        }

        public override void Draw(Vector2 pPos, Vector2 pScale, Vector2 pOrigin, Color pColor,
            float pRotation = 0, SpriteEffects pSpriteEffect = SpriteEffects.None, float pLayerDepth = 1)
        {
            Globals.SpriteBatch.Draw(_texture, pPos, _sourceRectangles[_frame], pColor, pRotation, pOrigin, pScale, pSpriteEffect, pLayerDepth);
        }
    }
}
