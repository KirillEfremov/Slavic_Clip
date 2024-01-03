using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace Builds
{
    public class SetGunsIntoCamera : NetworkBehaviour
    {
        [SerializeField]
        private GameObject[] guns;
        private List<Weapon> weapons = new List<Weapon>();
        [SerializeField]
        private GameObject _cam;
        private GameObject _currentGun;

        void OnCollisionEnter(Collision collision)
        {
            if (isLocalPlayer)
            {
                if (collision.gameObject.name == "Rail Gun")
                {
                    Destroy(collision.gameObject);
                    DestroyGun();
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[0], _cam.transform);
                    weapons.Add(_currentGun.GetComponent<Weapon>());
                }

                if (collision.gameObject.name == "Shotgun")
                {
                    Destroy(collision.gameObject);
                    DestroyGun();
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[1], _cam.transform);
                    weapons.Add(_currentGun.GetComponent<Weapon>());
                }

                if (collision.gameObject.name == "Beam Gun")
                {
                    Destroy(collision.gameObject);
                    DestroyGun();
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[2], _cam.transform);
                    weapons.Add(_currentGun.GetComponent<Weapon>());
                }

                if (collision.gameObject.name == "M4")
                {
                    Destroy(collision.gameObject);
                    DestroyGun();
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[3], _cam.transform);
                    weapons.Add(_currentGun.GetComponent<Weapon>());
                }

                if (collision.gameObject.name == "Pistol")
                {
                    Destroy(collision.gameObject);
                    DestroyGun();
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[4], _cam.transform);
                    weapons.Add(_currentGun.GetComponent<Weapon>());
                }

                if (collision.gameObject.name == "Rocket Launcher")
                {
                    Destroy(collision.gameObject);
                    DestroyGun();
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[5], _cam.transform);
                    weapons.Add(_currentGun.GetComponent<Weapon>());
                }
            }
        }

        private void DestroyGun()
        {
            Destroy(_currentGun);
        }

        public void DisableFireGun()
        {
            _currentGun.GetComponent<Weapon>().enabled = false;
        }
    }
}
