using UnityEngine;
using Mirror;

namespace Builds
{
    public class CameraMove : MonoBehaviour
    {
        public float _mouseSens = 300f;
        public Transform _playerBody;
        float _xRotation = 0f;


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        [Client]
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
       
            float mouseX = Input.GetAxis("Mouse X") * _mouseSens * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSens * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            _playerBody.Rotate(Vector3.up * mouseX);

        }
    }
}
