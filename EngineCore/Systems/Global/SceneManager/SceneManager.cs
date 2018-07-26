using ECS;
using ECS.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Global.SceneManager
{
    public class SceneManager: IExecuteSystem
    {
        private Dictionary<string, Scene> _scenes;
        private Scene _currentScene;
        public SceneManager(Dictionary<string, Scene> scenes)
        {
            _scenes = scenes;
        }

        public void Execute()
        {
            _currentScene?.Execute();
        }

        public bool ChangeScene(string sceneName)
        {
            if(_scenes.TryGetValue(sceneName, out Scene scene))
            {
                _currentScene?.CleanUp();
                _currentScene = scene;
                return true;
            }
            return false;
        }
    }
}
