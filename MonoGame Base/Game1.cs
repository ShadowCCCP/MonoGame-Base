using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame_Base.Project.Utility.Basics;
using System;

namespace MonoGame_Base
{
    public class Game1 : Game
    {
        // Screen size...
        int screenWidth = 1024;
        int screenHeight = 768;

        // To make debug info visible...
        private readonly bool _debugging = true;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameManager _gameManager;

        public Game1()
        {
            Globals.Debugging = _debugging;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Globals.ScreenWidth = screenWidth;
            Globals.ScreenHeight = screenHeight;

            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight;
            _graphics.ApplyChanges();

            Globals.GraphicsDevice = GraphicsDevice;
            Globals.Content = Content;

            _gameManager = new GameManager();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = _spriteBatch;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.Update(gameTime);
            _gameManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Beige);

            _gameManager.Draw();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            Globals.OnGameClosed?.Invoke(this, new EventArgs());

            base.UnloadContent();
        }
    }
}