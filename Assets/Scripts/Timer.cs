using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Builds
{
    public class Timer : MonoBehaviour
    {
        private bool _showLossScreen = false;
        private bool _isPlayedLoss = true;
        [SerializeField]
        private float _timerStart = 300f;
        [SerializeField]
        private Text _timerText;
       // private PlayerControl _text;
        public GameObject _BG;
        public AudioSource _gameOver;
       // private PlayerControl _isPlayedWin;
       // private PlayerControl _showWinScreen;
       
        public GameObject endGameScreen;
        
        private PlayerControl _playerControl;
        private SetGunsIntoCamera _gunsInto;

        private void Start()
        {
            _timerText.text = _timerStart.ToString();
            //_text = GetComponent<PlayerControl>();
            _playerControl = GetComponent<PlayerControl>();
            //_isPlayedWin = GetComponent<PlayerControl>();
            //_showWinScreen = GetComponent<PlayerControl>();
           
        }

        private void Update()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length >= 2)
            {
                _timerStart -= Time.deltaTime;
                _timerText.text = Mathf.Round(_timerStart).ToString();
                if (_timerStart <= 0)
                {
                    _showLossScreen = true;
                    _BG.SetActive(false);
                    Time.timeScale = 0f;
                    endGameScreen.SetActive(true);
                    _gunsInto.DisableFireGun();
                }

                if (_playerControl._showWinScreen == true)
                    if (_playerControl._isPlayedWin == false)
                        _BG.SetActive(false);
            }
        }        
        private void OnGUI()
        {
            if (_showLossScreen)
            {
                if (_isPlayedLoss)
                {
                    _gameOver.Play();
                }
                _playerControl.LabelText = "Ïðîñòðàíñòâî âðåìåíè ëîïíóëî!";
                if (GUI.Button(new Rect(Screen.width / 2 - 250,
                Screen.height / 2 - 50, 400, 100), "ÍÓ ÊÀÊ ÆÅ ÒÀÊ, ÁÎÃÀÒÛÐÜ!"))
                {
                    SceneManager.LoadScene(0);
                    Time.timeScale = 1.0f;
                }
                _isPlayedLoss = false;
            }
        }
    }
}
