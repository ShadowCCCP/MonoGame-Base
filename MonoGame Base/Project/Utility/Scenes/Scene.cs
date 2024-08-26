using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_Base.Project.Utility.Basics;
using MonoGame_Base.Project.Utility.Basics.Colliders;
using MonoGame_Base.Project.Utility.TileMap;
using System.Collections.Generic;

namespace MonoGame_Base.Project.Utility.Scenes
{
    public abstract class Scene
    {
        private List<GameObject> _deleteGameObjects = new List<GameObject>();
        private List<GameObject> _gameObjects = new List<GameObject>();

        protected Map _map;

        protected readonly RenderTarget2D target;
        protected readonly GameManager gameManager;

        private Texture2D backgroundTexture;

        public Scene(GameManager pGameManager)
        {
            gameManager = pGameManager;
            target = Globals.GetNewRenderTarget();
            backgroundTexture = CreateBackgroundTexture();
            Load();
        }

        protected abstract void Load();
        public virtual void Activate()
        {
            if (_map != null) { _map.ActivateColliders(); }
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.ActivateCollider();
            }
        }

        public virtual void Deactivate()
        {
            if (_map != null) { _map.DeactivateColliders(); }
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.DeactivateCollider();
            }
        }

        public virtual void Update()
        {
            DeleteGameObjects();

            gameManager.player.Update();

            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Update();
            }
        }

        protected virtual void Draw()
        {
            Globals.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, gameManager.translation);

            if (_map != null)
            {
                Point mapSize = new Point((int)_map.GetMapSize().X, (int)_map.GetMapSize().Y);
                Globals.SpriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, mapSize.X, mapSize.Y), Color.White);
                _map.Draw();
            }

            gameManager.player.Draw();

            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Draw();
            }

            CollisionManager.DrawColliders();

            Globals.SpriteBatch.End();
        }

        public virtual RenderTarget2D GetFrame()
        {
            Globals.GraphicsDevice.SetRenderTarget(target);
            Globals.GraphicsDevice.Clear(Color.Transparent);  // Clear with transparency

            Draw();

            Globals.GraphicsDevice.SetRenderTarget(null);  // Set back to the back buffer
            return target;
        }

        private void DeleteGameObjects()
        {
            foreach (GameObject gameObject in _deleteGameObjects)
            {
                _gameObjects.Remove(gameObject);
            }
        }

        public Map GetMap()
        {
            return _map;
        }

        public List<GameObject> GetObjectsInRange(GameObject pComparedTo, float pRange)
        {
            List<GameObject> objectsInRange = new List<GameObject>();

            // Get all objects in range...
            foreach (GameObject gameObject in _gameObjects)
            {
                if (pComparedTo == gameObject) { continue; }

                Vector2 distanceVector = gameObject.position - pComparedTo.position;
                float distance = distanceVector.Length();

                if (distance <= pRange)
                {
                    objectsInRange.Add(gameObject);
                }
            }

            // Sort all objects in range, from closest to furthest away...
            objectsInRange.Sort((a, b) =>
            {
                Vector2 distanceVectorA = a.position - pComparedTo.position;
                Vector2 distanceVectorB = b.position - pComparedTo.position;
                float distanceA = distanceVectorA.Length();
                float distanceB = distanceVectorB.Length();
                return distanceA.CompareTo(distanceB);
            });

            return objectsInRange;
        }

        public void AddGameObject(GameObject pGameObject)
        {
            _gameObjects.Add(pGameObject);
        }

        public void RemoveGameObject(GameObject pGameObject)
        {
            _deleteGameObjects.Add(pGameObject);
        }

        private Texture2D CreateBackgroundTexture()
        {
            Texture2D rectangleTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new[] { Color.Beige });

            return rectangleTexture;
        }
    }
}
