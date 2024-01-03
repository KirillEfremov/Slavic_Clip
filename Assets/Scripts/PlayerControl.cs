using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace Builds
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerControl : NetworkBehaviour
    {
        public string _labelText = "�������� � ������� ������� ����� �����, ����� ���������� ���� ��� ������� ������ ������� ����������� �������������, ��������, �� ����� ������ ����� ����������, ���������� ����� ������ �����������������!";
        private Animate _anim;
        public float _speed = 5f;
        [SerializeField]
        private float _lookspeed = 3f;
        private PlayerMotor _motor;
        public AudioClip _itemClip;
        public AudioClip _death;
        private AudioSource _audioSource;
        private int _startingHealth = 1000;
        private int _currentHealth;
        private int _maxBunner = 1;
        private int _bunnerCollected = 0;
        public bool _showWinScreen = false;
        public bool _isPlayedWin = true;
        [SerializeField]
        private AudioSource _gameWin;
        private int _weaponsCollected = 0;
        private int _maxItems = 6;
        public GameObject _BG;
        [SerializeField]
        private AudioSource _gameOver;
        private bool _isPlayedDie = true;
        public bool _showLossScreen = false;
        [SerializeField]
        private GameObject _player;

        private void Start()
        {
            _anim = GetComponent<Animate>();
            _motor = GetComponent<PlayerMotor>();
            _audioSource = GetComponent<AudioSource>();
            _currentHealth = _startingHealth;
            _player = GameObject.FindWithTag("Player");
            if (isLocalPlayer)
            {
                _BG.SetActive(true);
            }
        }

        //�������� ������ ��� ������������ � �������
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "Rail Gun")
            {
                _audioSource.PlayOneShot(_itemClip);
                Debug.Log("Here it is the weapon of the Russian land, in your hands!");
                Weapons += 1;
            }
            if (collision.gameObject.name == "Shotgun")
            {
                _audioSource.PlayOneShot(_itemClip);
                Debug.Log("Here it is the weapon of the Russian land, in your hands!");
                Weapons += 1;
            }
            if (collision.gameObject.name == "Beam Gun")
            {
                _audioSource.PlayOneShot(_itemClip);
                Debug.Log("Here it is the weapon of the Russian land, in your hands!");
                Weapons += 1;
            }
            if (collision.gameObject.name == "M4")
            {
                _audioSource.PlayOneShot(_itemClip);
                Debug.Log("Here it is the weapon of the Russian land, in your hands!");
                Weapons += 1;
            }
            if (collision.gameObject.name == "Pistol")
            {
                _audioSource.PlayOneShot(_itemClip);
                Debug.Log("Here it is the weapon of the Russian land, in your hands!");
                Weapons += 1;
            }
            if (collision.gameObject.name == "Rocket Launcher")
            {
                _audioSource.PlayOneShot(_itemClip);
                Debug.Log("Here it is the weapon of the Russian land, in your hands!");
                Weapons += 1;
            }
        }
        private void Update()
        {
            #region PlayerMove
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
                _anim._anim.SetBool("isFastRunning", true);
                _speed = 10f;
            }

            else
            {
                _anim._anim.SetBool("isFastRunning", false);
                _speed = 5f;
            }
            #endregion

            LossFell();
        }
        //�������� ������
        public int CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value;
            }
        }
        //���������� ����� 
        public int Bunner
        {
            get { return _bunnerCollected; }
            set
            {
                _bunnerCollected = value;
                BunnerWin();
            }
        }
        //������ ������ 
        public int Weapons
        {
            get { return _weaponsCollected; }
            set
            {
                _weaponsCollected = value;
                WeaponsWin();
            }
        }
        //��������� ������� ������
        private void ShowWinScreen()
        {
            _showWinScreen = true;
            Time.timeScale = 0f;
            GetComponent<SetGunsIntoCamera>().DisableFireGun();
        }
        //����� ���� - �������
        private void BunnerWin()
        {
            Debug.LogFormat("The symbol is sacred:{0}", _bunnerCollected);
            if (_bunnerCollected >= _maxBunner)
            {
                _labelText = "������ ������ ���������!";
                ShowWinScreen();
            }
        }
        //����� ������ - �������
        private void WeaponsWin()
        {
            Debug.LogFormat("Weapons:{0}", _weaponsCollected);
            if (_weaponsCollected >= _maxItems)
            {
                _labelText = "��������� ������ �������� ���� ����� ������ �������������!";
                ShowWinScreen();
            }
            else
            {
                _labelText = "����� ���� ��� ������ " +
                (_maxItems - _weaponsCollected) + ". ������, ������!";
            }
        }
        //�������� �����
        public void WinScreen()
        {
            if (_showWinScreen)
            {
                Win();
            }
        }
        //����� ������
        private void Win()
        {
            if (_isPlayedWin)
            {
                _BG.SetActive(false);
                _gameWin.Play();
            }
            _isPlayedWin = false;
            if (GUI.Button(new Rect(Screen.width / 2 - 250,
              Screen.height / 2 - 50, 400, 100), "������� ����-�������! �� ��������!"))
            {
                SceneManager.LoadScene(0);
                Time.timeScale = 1.0f;
            }
        }
        //���� - ��������
        private void LossFell()
        {
            if (_player.transform.position.y <= -20 || gameObject.transform.position.y <= -20)
            {
                _showLossScreen = true;
                if (_player.transform.position.y <= -120)
                {
                    Time.timeScale = 0f;
                }
            }
        }
        //����� ���������
        public void LossScreen()
        {
            if (_showLossScreen)
            {
                Loss();
            }          
        }
        //����� ���������
        private void Loss()
        {
            _anim._anim.SetBool("isDeath", true);
            if (_isPlayedDie)
            {
                _BG.SetActive(false);
                _gameOver.Play();
            }
            _labelText = "����������-������ ������!";
            if (GUI.Button(new Rect(Screen.width / 2 - 250,
            Screen.height / 2 - 50, 400, 100), "�� ��� �� ���, ��������!"))
            {
                SceneManager.LoadScene(0);
                Time.timeScale = 1.0f;
            }
            _isPlayedDie = false;
        }
        //��������� ���������� ��������� ������, ������ ������ � ���������
        void OnGUI()
        {
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 30;
            myStyle.fontStyle = FontStyle.BoldAndItalic;
            myStyle.normal.textColor = Color.white;

            GUI.Box(new Rect(1350, 90, 250, 50),
            "������ ��������������������: " + Weapons, myStyle);
            GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height - 100,
            300, 150), _labelText);

            WinScreen();
            LossScreen();

            if (GUI.Button(new Rect(Screen.width / 2 - 650,
             Screen.height / 2 - 500, 150, 50), "����� �� ����"))
            {
                Application.Quit();
            }
        }
    }
}
