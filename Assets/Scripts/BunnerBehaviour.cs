using UnityEngine;
namespace Builds
{
    public class BunnerBehaviour : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerControl>(out var player))
            {
                player.Bunner += 1;
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
