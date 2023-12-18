using UnityEngine;
namespace Builds
{
    public class WeaponsBehaviour : MonoBehaviour
    {
        private GameBehaviour _weapons;

        private void Start()
        {
            _weapons = GameObject.Find("GameManager").GetComponent<GameBehaviour>();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                DestroyMethod();
            }
        }

        public void DestroyMethod()
        {
            Destroy(transform.gameObject);
            Debug.Log("Here it is the weapon of the Russian land, in your hands!");
            _weapons.Weapons += 1;
        }
    }
}
