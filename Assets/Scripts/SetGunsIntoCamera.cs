using UnityEngine;

namespace Builds
{
    public class SetGunsIntoCamera : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] guns;
        [SerializeField]
        private GameObject _cam;
        private GameObject _currentGun;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "Rail Gun")
            {
                Destroy(_currentGun);
                _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                _currentGun = Instantiate(guns[0], _cam.transform);    
            }

            if (collision.gameObject.name == "Shotgun")
            {
                Destroy(_currentGun);
                _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                _currentGun = Instantiate(guns[1], _cam.transform); 
            }

            if (collision.gameObject.name == "Beam Gun")
            {
                Destroy(_currentGun);
                _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                _currentGun = Instantiate(guns[2], _cam.transform);
            }

            if (collision.gameObject.name == "M4")
            {
                Destroy(_currentGun);
                _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                _currentGun = Instantiate(guns[3], _cam.transform);
            }

            if (collision.gameObject.name == "Pistol")
            {
                Destroy(_currentGun);
                _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                _currentGun = Instantiate(guns[4], _cam.transform);
            }

            if (collision.gameObject.name == "Rocket Launcher")
            {
                Destroy(_currentGun);
                _cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                _currentGun = Instantiate(guns[5], _cam.transform);
            }
        }
    }
}
