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
        private GameBehaviour _text;
        private GameObject BG;
        public AudioSource _gameOver;
        private GameBehaviour _isPlayedWin;
        private GameBehaviour _showWinScreen;
        [SerializeField]
        private GameObject _player1;
        [SerializeField]
        private GameObject _player2;


        private void Start()
        {
            _timerText.text = _timerStart.ToString();
            _text = GameObject.Find("GameManager").GetComponent<GameBehaviour>();
            BG = GameObject.Find("BG");
            _isPlayedWin = GameObject.Find("GameManager").GetComponent<GameBehaviour>();
            _showWinScreen = GameObject.Find("GameManager").GetComponent<GameBehaviour>();
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
                        BG.SetActive(false);
                        Time.timeScale = 0f;
                    }

                    if (_showWinScreen._showWinScreen == true)
                        if (_isPlayedWin._isPlayedWin == false)
                            BG.SetActive(false);
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
