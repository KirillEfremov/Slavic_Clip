using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace Builds
{
    public class SetGunsIntoCamera : NetworkBehaviour
    {
        [SerializeField]
        private GameObject[] guns;
        [SerializeField]
        private List<GameObject> weapons = new List<GameObject>();
        [SerializeField]
        private GameObject _cam;
        [SerializeField]
        private GameObject _currentGun;
        private int weaponIndex = 0;

        void Start()
        {
            SetActiveWeapon(weaponIndex);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (isLocalPlayer)
            {
                if (collision.gameObject.name == "Rail Gun")
                {
                    _currentGun.SetActive(false);
                    Destroy(collision.gameObject);
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[0], _cam.transform);
                    weapons.Add(_currentGun);
                    _currentGun.SetActive(true);
                }

                if (collision.gameObject.name == "Shotgun")
                {
                    _currentGun.SetActive(false);
                    Destroy(collision.gameObject);
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[1], _cam.transform);
                    weapons.Add(_currentGun);
                    _currentGun.SetActive(true);
                }

                if (collision.gameObject.name == "Beam Gun")
                {
                    _currentGun.SetActive(false);
                    Destroy(collision.gameObject);
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[2], _cam.transform);
                    weapons.Add(_currentGun);
                    _currentGun.SetActive(true);
                }

                if (collision.gameObject.name == "M4")
                {
                    _currentGun.SetActive(false);
                    Destroy(collision.gameObject);
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[3], _cam.transform);
                    weapons.Add(_currentGun);
                    _currentGun.SetActive(true);
                }

                if (collision.gameObject.name == "Pistol")
                {
                    _currentGun.SetActive(false);
                    Destroy(collision.gameObject);
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[4], _cam.transform);
                    weapons.Add(_currentGun);
                    _currentGun.SetActive(true);
                }

                if (collision.gameObject.name == "Rocket Launcher")
                {
                    _currentGun.SetActive(false);
                    Destroy(collision.gameObject);
                    _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                    _currentGun = Instantiate(guns[5], _cam.transform);
                    weapons.Add(_currentGun);
                    _currentGun.SetActive(true);
                }
            }
        }

        private void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                NextWeapon();
            
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                PreviousWeapon();
            
        }

        public void NextWeapon()
        {
            weaponIndex++;
            if (weaponIndex >= weapons.Count)
                weaponIndex = 0;
            SetActiveWeapon(weaponIndex);
        }
        
        public void PreviousWeapon()
        {
            weaponIndex--;
            if (weaponIndex <= 0)
                weaponIndex = weapons.Count-1;
            SetActiveWeapon(weaponIndex);
        }
        
        public void SetActiveWeapon(int index)
        {     
            weaponIndex = index;
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].SetActive(false);
            }
            weapons[index].SetActive(true);
        }
        
        public void DisableFireGun()
        {
            _currentGun.GetComponent<Weapon>().enabled = false;
        }
        
    }
}
