using Microsoft.Xna.Framework;
using MonoGame_Base.Project.Game.Objects.Misc;
using MonoGame_Base.Project.Utility.Basics;
using MonoGame_Base.Project.Utility.Scenes;
using MonoGame_Base.Project.Utility.TileMap;

namespace MonoGame_Base.Project.Game.Levels
{
    public class Intro : Scene
    {
        private SmileyBall _smileyBall;
        private Platform _platform;

        public Intro(GameManager pGameManager) : base(pGameManager) { }

        protected override void Load()
        {
            _map = new Map("Intro");

            _smileyBall = new SmileyBall(new Vector2(600, 600));
            AddGameObject(_smileyBall);

            _platform = new Platform(new Vector2(900, 150));
            AddGameObject(_platform);
        }

        public override void Activate()
        {
            base.Activate();

            gameManager.player.position = new Vector2(200, 100);
            _smileyBall.position = new Vector2(600, 600);
        }
    }
}
