using UnityEngine;

namespace Builds
{
    public class IsTriggerAngel : MonoBehaviour
    {
        [SerializeField]
        private Collider _isTriggerAngel;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
                _isTriggerAngel.isTrigger = true;
            else
                _isTriggerAngel.isTrigger = false;
        }
    }
}
