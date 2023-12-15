using UnityEngine;
namespace Builds
{
    public class WeaponsBehaviour : MonoBehaviour
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
                DestroyMethod();
            }

            if (collision.gameObject.name == "Player [connId=1]")
            {
                DestroyMethod();
            }

            if (collision.gameObject.name == "Player [connId=2]")
            {
                DestroyMethod();
            }

            if (collision.gameObject.name == "Player [connId=3]")
            {
                DestroyMethod();
            }
        }

        public void DestroyMethod()
        {
            Destroy(transform.gameObject);
            Debug.Log("Here it is the weapon of the Russian land, in your hands!");
            gameManager.Weapons += 1;
        }
    }
}
