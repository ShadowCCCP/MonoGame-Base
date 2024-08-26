using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame_Base.Project.Utility.Basics.Colliders;
using MonoGame_Base.Project.Utility.Scenes;
using MonoGame_Base.Project.Game.Player;

namespace MonoGame_Base.Project.Utility.Basics
{
    public class GameManager
    {
        public Matrix translation { get; private set; }
        public Player player { get; private set; }
        private readonly SceneManager _sceneManager;

        public GameManager()
        {
            player = new Player(new Vector2(400, 400));
            player.ActivateCollider();
            _sceneManager = new SceneManager(this);
        }

        protected void CalculateTranslation()
        {
            // Center the camera on the player
            float x = Globals.ScreenWidth / 2 - (player.position.X + player._spriteSize.X / 2);
            if (Globals.CurrentMap != null) { x = MathHelper.Clamp(x, -Globals.CurrentMap.GetMapSize().X + Globals.ScreenWidth, 0); }

            float y = Globals.ScreenHeight / 2 - (player.position.Y + player._spriteSize.Y / 2);
            if (Globals.CurrentMap != null) { y = MathHelper.Clamp(y, -Globals.CurrentMap.GetMapSize().Y + Globals.ScreenHeight, 0); }

            translation = Matrix.CreateTranslation(x, y, 0);
        }

        public void Update()
        {
            CalculateTranslation();
            CollisionManager.CheckCollisions();
            InputManager.Update();

            _sceneManager.Update();
        }

        public void Draw()
        {
            RenderTarget2D sceneFrame = _sceneManager.GetFrame();  // Get the frame from the current scene

            Globals.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            Globals.SpriteBatch.Draw(sceneFrame, Vector2.Zero, Color.White);  // Draw the scene frame

            Globals.SpriteBatch.End();
        }
    }
}
