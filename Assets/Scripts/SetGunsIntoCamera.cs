using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

namespace Builds
{
    public class SetGunsIntoCamera : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] guns;
        [SerializeField]
        private GameObject cam;
        private GameObject currentGun;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "Rail Gun")
            {
                Destroy(currentGun);
                cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                currentGun = Instantiate(guns[0], cam.transform);    
            }

            if (collision.gameObject.name == "Shotgun")
            {
                Destroy(currentGun);
                cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                currentGun = Instantiate(guns[1], cam.transform); 
            }

            if (collision.gameObject.name == "Beam Gun")
            {
                Destroy(currentGun);
                cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                currentGun = Instantiate(guns[2], cam.transform);
            }

            if (collision.gameObject.name == "M4")
            {
                Destroy(currentGun);
                cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                currentGun = Instantiate(guns[3], cam.transform);
            }

            if (collision.gameObject.name == "Pistol")
            {
                Destroy(currentGun);
                cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                currentGun = Instantiate(guns[4], cam.transform);
            }

            if (collision.gameObject.name == "Rocket Launcher")
            {
                Destroy(currentGun);
                cam.transform.localPosition = new Vector3(0, 1.7f, 0.7f);
                currentGun = Instantiate(guns[5], cam.transform);
            }
        }
    }
}
