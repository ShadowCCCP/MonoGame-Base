using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame_Base.Project.Game.Levels;
using MonoGame_Base.Project.Utility.Basics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame_Base.Project.Utility.Scenes
{
    public class SceneManager
    {
        public int sceneCount { get; private set; }

        public static int activeSceneIndex { get; private set; }
        private static readonly Dictionary<int, Scene> _scenes = new Dictionary<int, Scene>();

        private static int _queuedSceneChange = -1;

        public SceneManager(GameManager pGameManager)
        {
            AddScene(pGameManager, new Intro(pGameManager));
            AddScene(pGameManager, new Detailed(pGameManager));

            Globals.CurrentMap = _scenes[activeSceneIndex].GetMap();

            if (sceneCount > 0) { _scenes[activeSceneIndex].Activate(); }
        }

        public static void SwitchScene(int pSceneIndex)
        {
            _queuedSceneChange = pSceneIndex;
        }

        private void ActivateQueuedScene()
        {
            if (!_scenes.ContainsKey(_queuedSceneChange)) { return; }

            _scenes[activeSceneIndex].Deactivate();

            activeSceneIndex = _queuedSceneChange;
            _scenes[_queuedSceneChange].Activate();

            Globals.CurrentMap = _scenes[activeSceneIndex].GetMap();
        }

        public void Update()
        {
            _scenes[activeSceneIndex].Update();

            if (_queuedSceneChange != -1)
            {
                ActivateQueuedScene();  // Switch scenes after the current scene has updated
                _queuedSceneChange = -1;
            }

            Testing();
        }

        public RenderTarget2D GetFrame()
        {
            return _scenes[activeSceneIndex].GetFrame();
        }

        private void Testing()
        {
            if (InputManager.KeyPressed(Keys.D1))
            {
                if (activeSceneIndex == 0)
                {
                    SwitchScene(1);
                }
                else
                {
                    SwitchScene(0);
                }
            }
        }

        private void AddScene(GameManager pGameManager, Scene pScene)
        {
            _scenes.Add(sceneCount, pScene);
            sceneCount++;
        }

        public static List<GameObject> GetObjectsInRange(GameObject pComparedTo, float pRange)
        {
            return _scenes[activeSceneIndex].GetObjectsInRange(pComparedTo, pRange);
        }

        public static void AddGameObject(GameObject pGameObject)
        {
            _scenes[activeSceneIndex].AddGameObject(pGameObject);
        }

        public static void RemoveGameObject(GameObject pGameObject)
        {
            _scenes[activeSceneIndex].RemoveGameObject(pGameObject);
        }
    }
}
