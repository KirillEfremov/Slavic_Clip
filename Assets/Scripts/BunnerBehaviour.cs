using UnityEngine;
namespace Builds
{
    public class BunnerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlayerControl _bunnerCollected;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                _bunnerCollected = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
                Destroy(this.transform.parent.gameObject);
                Debug.Log("A sacred symbol has been found!");
                _bunnerCollected.Bunner += 1;
            }
        }
    }
}
