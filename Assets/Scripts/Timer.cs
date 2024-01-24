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
        private PlayerControl _text;
        private GameObject _BG;
        public AudioSource _gameOver;
        private PlayerControl _isPlayedWin;
        private PlayerControl _showWinScreen;
        [SerializeField]
        private GameObject _player1;
        [SerializeField]
        private GameObject _player2;

        private void Start()
        {
            _timerText.text = _timerStart.ToString();
            _text = GetComponent<PlayerControl>();
            _BG = GameObject.Find("BG");
            _isPlayedWin = GetComponent<PlayerControl>();
            _showWinScreen = GetComponent<PlayerControl>();
            _player1 = GameObject.Find("Player1");
            _player2 = GameObject.Find("Player2");
        }

        private void Update()
        {
            if (_player1 = GameObject.Find("Player1"))
            {
                if (_player2 = GameObject.Find("Player2"))
                {
                    _timerStart -= Time.deltaTime;
                    _timerText.text = Mathf.Round(_timerStart).ToString();
                    if (_timerStart <= 0)
                    {
                        _showLossScreen = true;
                        _BG.SetActive(false);
                        Time.timeScale = 0f;
                    }

                    if (_showWinScreen._showWinScreen == true)
                        if (_isPlayedWin._isPlayedWin == false)
                            _BG.SetActive(false);
                }
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
                _text._labelText = "Ïðîñòðàíñòâî âðåìåíè ëîïíóëî!";
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
