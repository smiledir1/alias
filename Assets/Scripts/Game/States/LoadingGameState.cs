using Common.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.States
{
    public class LoadingGameState : GameState
    {
        protected override UniTask OnExitState()
        {
            var icon = FindGameObjectByName("Icon");
            if (icon != null) Object.Destroy(icon.gameObject);
            return UniTask.CompletedTask;
        }

        private GameObject FindGameObjectByName(string goName)
        {
            var activeScene = SceneManager.GetActiveScene();
            var rootObjects = activeScene.GetRootGameObjects();
            foreach (var go in rootObjects)
            {
                if (go.name == goName) return go;
                var findGo = FindGameObjectByName(go, goName);
                if (findGo != null) return findGo;
            }

            return null;
        }

        private GameObject FindGameObjectByName(GameObject root, string goName)
        {
            for (var i = root.transform.childCount - 1; i >= 0; i--)
            {
                var child = root.transform.GetChild(i);
                if (child.name == goName) return child.gameObject;
                var findGo = FindGameObjectByName(child.gameObject, goName);
                if (findGo != null) return findGo;
            }

            return null;
        }
    }
}