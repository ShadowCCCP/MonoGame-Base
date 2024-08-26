using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using MonoGame_Base.Project.Utility.TileMap;

namespace MonoGame_Base.Project.Utility.Basics
{
    public delegate void GameClosedEventHandler(object sender, EventArgs e);

    public static class Globals
    {
        private static Random _rand = new Random();

        public static bool Debugging { get; set; }
        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
        public static Vector2 PlayerPosition { get; set; }
        public static Map CurrentMap { get; set; }
        public static float TotalSeconds { get; set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static GraphicsDevice GraphicsDevice { get; set; }
        public static GameClosedEventHandler OnGameClosed { get; set; }

        public static RenderTarget2D GetNewRenderTarget()
        {
            return new RenderTarget2D(GraphicsDevice, ScreenWidth, ScreenHeight, false, SurfaceFormat.Alpha8, DepthFormat.None);
        }

        public static void Update(GameTime pGameTime)
        {
            TotalSeconds = (float)pGameTime.ElapsedGameTime.TotalSeconds;
        }

        public static float NextFloat(float minValue, float maxValue)
        {
            return (float)(minValue + (_rand.NextDouble() * (maxValue - minValue)));
        }

        public static int NextInt(int minValue, int maxValue)
        {
            // In order to make it consistent, we make the maxValue inclusive...
            return _rand.Next(minValue, maxValue + 1);
        }

        public static Color GetRandomColor()
        {
            int r = Globals.NextInt(0, 255);
            int g = Globals.NextInt(0, 255);
            int b = Globals.NextInt(0, 255);

            return new Color(r, g, b);
        }
    }
}
