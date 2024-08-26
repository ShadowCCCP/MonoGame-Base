using Microsoft.Xna.Framework;
using MonoGame_Base.Project.Utility.Basics;
using MonoGame_Base.Project.Utility.Scenes;
using MonoGame_Base.Project.Utility.TileMap;

namespace MonoGame_Base.Project.Game.Levels
{
    public class Detailed : Scene
    {
        public Detailed(GameManager pGameManager) : base(pGameManager) { }

        protected override void Load()
        {
            _map = new Map("Detailed");
        }

        public override void Activate()
        {
            base.Activate();

            gameManager.player.position = new Vector2(400, 400);
        }
    }
}
