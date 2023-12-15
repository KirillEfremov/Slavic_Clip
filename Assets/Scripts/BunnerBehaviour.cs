using UnityEngine;
namespace Builds
{
    public class BunnerBehaviour : MonoBehaviour
    {
        private GameBehaviour gameManager;
        void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameBehaviour>();
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "Player [connId=0]")
            {
                Destroy(this.transform.parent.gameObject);
                Debug.Log("A sacred symbol has been found!");
                gameManager.Bunner += 1;
            }
        }
    }
}
