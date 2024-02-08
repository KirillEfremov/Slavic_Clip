using Mirror;
using System.Collections.Generic;
using UnityEngine;
namespace Builds
{
    public class WeaponsBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlayerControl _weapons;
        private List<string> players = new List<string>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                if (!players.Contains(other.gameObject.name))
                {
                    _weapons = other.gameObject.GetComponent<PlayerControl>();
                    DestroyMethod(other.gameObject);
                }
            }
        }       
        public void DestroyMethod(GameObject player)
        {
            players.Add(player.name);
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                transform.GetChild(3).gameObject.SetActive(false);
            }
            Debug.Log("Here it is the weapon of the Russian land, in your hands!");
            _weapons.Weapons += 1;
        }
    }
}
