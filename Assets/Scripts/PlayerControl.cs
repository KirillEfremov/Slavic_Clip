using UnityEngine;
using Mirror;

namespace Builds
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerControl : MonoBehaviour
    {
        private Animator _anim;
        public float _speed = 5f;
        [SerializeField]
        private float _lookspeed = 3f;
        private PlayerMotor _motor;
        public AudioClip _itemClip;
        public AudioClip _death;
        private AudioSource _audioSource;


        private void Start()
        {
            _anim = GetComponent<Animator>();
            _motor = GetComponent<PlayerMotor>();
            _audioSource = GetComponent<AudioSource>();
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "Rail Gun")
            {
                _audioSource.PlayOneShot(_itemClip);
            }
            if (collision.gameObject.name == "Shotgun")
            {
                _audioSource.PlayOneShot(_itemClip);
            }
            if (collision.gameObject.name == "Beam Gun")
            {
                _audioSource.PlayOneShot(_itemClip);
            }
            if (collision.gameObject.name == "M4")
            {
                _audioSource.PlayOneShot(_itemClip);
            }
            if (collision.gameObject.name == "Pistol")
            {
                _audioSource.PlayOneShot(_itemClip);
            }
            if (collision.gameObject.name == "Rocket Launcher")
            {
                _audioSource.PlayOneShot(_itemClip);
            }
        }

        [Client]
        private void Update()
        {
            float xMove = Input.GetAxisRaw("Horizontal");
            float zMove = Input.GetAxisRaw("Vertical");

            Vector3 moveHor = transform.right * xMove;
            Vector3 moveVer = transform.forward * zMove;

            Vector3 velocity = (moveHor + moveVer).normalized * _speed;

            _motor.Move(velocity);

            float yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 rotation = new Vector3(0f, yRotation, 0f) * _lookspeed;

            _motor.Rotate(rotation);

            float xRotation = Input.GetAxisRaw("Mouse Y");
            Vector3 canRotation = new Vector3(xRotation, 0f, 0f) * _lookspeed;

            if (Input.GetKey(KeyCode.W) && (Input.GetKey(KeyCode.LeftShift)))
            {
                _anim.SetBool("isFastRunning", true);
                _speed = 10f;
            }

            else
            {
                _anim.SetBool("isFastRunning", false);
                _speed = 5f;
            }

        }

    }
}
