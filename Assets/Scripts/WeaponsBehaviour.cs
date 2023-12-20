using UnityEngine;
namespace Builds
{
    public class WeaponsBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlayerControl _weapons;

        void OnCollisionEnter(Collision collision)
        {
            _weapons = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
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
