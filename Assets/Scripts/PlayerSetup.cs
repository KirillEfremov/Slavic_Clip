using Mirror;
using UnityEngine;

namespace Builds
{
    public class PlayerSetup : NetworkBehaviour
    {
        private Camera _sceneCamera;
        [SerializeField]
        Behaviour[] componentsToDisable;

        private void Start()
        {
            if (!isLocalPlayer)
                for (int i = 0; i < componentsToDisable.Length; i++)
                    componentsToDisable[i].enabled = false;
            else
            {
                _sceneCamera = Camera.main;
                if (_sceneCamera != null)
                    _sceneCamera.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (_sceneCamera != null)
                _sceneCamera.gameObject.SetActive(true);
        }
    }
}
