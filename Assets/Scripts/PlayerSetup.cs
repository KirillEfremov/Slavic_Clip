using Mirror;
using UnityEngine;

namespace Builds
{
    public class PlayerSetup : NetworkBehaviour
    {
        private Camera _sceneCamera;
        [SerializeField]
        Behaviour[] componentsToDisable;
        [SerializeField]
        private string remotePlayer = "RemotePlayer";
        [SerializeField]
        private string localPlayer = "LocalPlayer";

        private void Start()
        {
            if (!isLocalPlayer)
            {
                for (int i = 0; i < componentsToDisable.Length; i++)
                    componentsToDisable[i].enabled = false;
                gameObject.layer = LayerMask.NameToLayer(localPlayer);
            }
            else
            {
                _sceneCamera = Camera.main;
                if (_sceneCamera != null)
                    _sceneCamera.gameObject.SetActive(false);
                gameObject.layer = LayerMask.NameToLayer(remotePlayer);
            }

            RegisterPlayer();
        }

        void RegisterPlayer()
        {
            string _ID = "Player" + GetComponent<NetworkIdentity>().netId;
            transform.name = _ID;
        }

        private void OnDisable()
        {
            if (_sceneCamera != null)
                _sceneCamera.gameObject.SetActive(true);
        }
    }
}
