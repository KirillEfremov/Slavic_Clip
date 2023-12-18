using UnityEngine;
namespace Builds
{
    public class BunnerBehaviour : MonoBehaviour
    {
        private GameBehaviour gameManager;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                Destroy(this.transform.parent.gameObject);
                Debug.Log("A sacred symbol has been found!");
                gameManager.Bunner += 1;
            }
        }
    }
}
