using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Builds
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerControl : NetworkBehaviour
    {
        private Stats stats;
        public GameObject endGameScreen;
        public GameObject statsPrefab;
        public Text name;
        public Text score;
        public string LabelText = "Возьмите с землице русской шесть вещиц, чтобы супостатов бить или найдите символ величия государства православного, совершив, во Славу памяти места священного, славянский зажим ящерам перевоплотившимся!";
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
        public int _weaponsCollected = 0;
        private int _maxItems = 6;
        public GameObject _BG;
        [SerializeField]
        private AudioSource _gameOver;
        private bool _isPlayedDie = true;
        public bool _showLossScreen = false;
        [SerializeField]
        private GameObject _player;
        private string _text;
        private bool _isTrig;
        [SerializeField]
        private AudioSource _buff;

        private static readonly int IsDeath = Animator.StringToHash("isDeath");

        private void Start()
        {
            _anim = GetComponent<Animate>();
            _motor = GetComponent<PlayerMotor>();
            _audioSource = GetComponent<AudioSource>();
            _currentHealth = _startingHealth;
            _player = GameObject.FindWithTag("Player");
            stats = GameObject.FindGameObjectWithTag("Stats").GetComponent<Stats>();
            if (isLocalPlayer)
            {
                _BG.SetActive(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "RussianFlag")
            {
                _text = "Нажми клавишу F заморскую. Получишь силушку богатырскую!";
                _isTrig = true;
            }
        }

        //звуковой эффект при столкновении с оружием
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<IPickable>(out var item))
            {
                item.PlayOneShot(_itemClip);
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

            if (isServer)
            {
                stats.stats.Clear();
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < players.Length; i++)
                {
                    PlayerStats playerStats = new PlayerStats();
                    playerStats.name = players[i].name;
                    playerStats.count = players[i].GetComponent<PlayerControl>()._weaponsCollected;
                    stats.stats.Add(playerStats);
                }
            }

            if (_isTrig)
                if (Input.GetKeyDown(KeyCode.F))
                {
                    CurrentHealth = 1000;
                    _buff.Play();
                    _isTrig = false;
                }
        }
        //здоровье игрока
        public int CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value;
            }
        }
        //нахождение герба 
        public int Bunner
        {
            get { return _bunnerCollected; }
            set
            {
                _bunnerCollected = value;
                BunnerWin();
            }
        }
        //подбор оружия 
        public int Weapons
        {
            get { return _weaponsCollected; }
            set
            {
                _weaponsCollected = value;
                WeaponsWin();
            }
        }
        //открывает условие победы
        private void ShowWinScreen()
        {
            _showWinScreen = true;
            GetComponent<SetGunsIntoCamera>().DisableFireGun();
            Time.timeScale = 0f;
        }
        //нашел герб - выиграл
        private void BunnerWin()
        {
            Debug.LogFormat("The symbol is sacred:{0}", _bunnerCollected);
            if (_bunnerCollected >= _maxBunner)
            {
                LabelText = "Найден символ священный!";
                ShowWinScreen();
            }
        }
        //нашел оружие - выиграл
        private void WeaponsWin()
        {
            Debug.LogFormat("Weapons:{0}", _weaponsCollected);
            if (_weaponsCollected >= _maxItems)
            {
                LabelText = "Обладаешь теперь силушкой всех шести оружий огнестрельных!";
                ShowWinScreen();
            }
            else
            {
                LabelText = "Нужно тебе еще оружия " +
                (_maxItems - _weaponsCollected) + ". Вперед, братец!";
            }
        }
        //победный экран
        public void WinScreen()
        {
            if (_showWinScreen)
            {
                Win();
            }
        }
        //метод победы
        private void Win()
        {
            if (_isPlayedWin)
            {
                _BG.SetActive(false);
                _gameWin.Play();
            }
            _isPlayedWin = false;
            endGameScreen.SetActive(true);
            name.text = stats.name.text;
            score.text = stats.score.text;
            if (GUI.Button(new Rect(Screen.width / 2 - 250,
              Screen.height / 2 - 50, 400, 100), "СЛАВЬСЯ РУСЬ-МАТУШКА! МЫ ПОБЕДИЛИ!"))
            {
                SceneManager.LoadScene(0);
                Time.timeScale = 1.0f;
            }
        }
        //упал - проиграл
        private void LossFell()
        {
            if (_player.transform.position.y <= -20 || gameObject.transform.position.y <= -20)
            {
                _showLossScreen = true;
                if (_player.transform.position.y <= -120)
                {
                    Time.timeScale = 0f;
                    GetComponent<SetGunsIntoCamera>().DisableFireGun();
                    endGameScreen.SetActive(true);
                }
            }
        }
        //экран поражения
        public void LossScreen()
        {
            if (_showLossScreen)
            {
                Loss();
            }          
        }
        //метод поражения
        private void Loss()
        {
            _anim._anim.SetBool(IsDeath, true);
            if (_isPlayedDie)
            {
                _BG.SetActive(false);
                _gameOver.Play();
            }
            LabelText = "Бесконечно-вечное близко!";
            endGameScreen.SetActive(true);
            name.text = stats.name.text;
            score.text = stats.score.text;
            if (GUI.Button(new Rect(Screen.width / 2 - 250,
            Screen.height / 2 - 50, 400, 100), "НУ КАК ЖЕ ТАК, БОГАТЫРЬ!"))
            {
                SceneManager.LoadScene(0);
                Time.timeScale = 1.0f;
            }
            _isPlayedDie = false;
        }
        //изменение интерфейса коллекции оружия, методы победы и поражения
        void OnGUI()
        {
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 30;
            myStyle.fontStyle = FontStyle.BoldAndItalic;
            myStyle.normal.textColor = Color.white;

            GUI.Box(new Rect(1350, 90, 250, 50),
            "Оружия противосупостатского: " + Weapons, myStyle);
            GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height - 100,
            300, 150), LabelText);

            WinScreen();
            LossScreen();

            if (GUI.Button(new Rect(Screen.width / 2 - 650,
             Screen.height / 2 - 500, 150, 50), "Выход из игры"))
            {
                Application.Quit();
            }

            if (_isTrig)
            {
                GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height - 200,
                300, 150), _text);
            }
        }
    }
}
