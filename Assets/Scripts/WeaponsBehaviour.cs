using UnityEngine;
namespace Builds
{
    public class WeaponsBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlayerControl _weapons;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                _weapons = collision.gameObject.GetComponent<PlayerControl>();
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
