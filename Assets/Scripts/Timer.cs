using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Builds
{
    public class Timer : MonoBehaviour
    {
        private bool showLossScreen = false;
        private bool isPlayedLoss = true;
        [SerializeField]
        private float timerStart = 300f;
        [SerializeField]
        private Text timerText;
        private GameBehaviour Text;
        private GameObject BG;
        public AudioSource gameOver;
        private GameBehaviour isPlayedWin;
        private GameBehaviour showWinScreen;
        [SerializeField]
        private Button _buttonLoss;

        private void Start()
        {
            timerText.text = timerStart.ToString();
            Text = GameObject.Find("GameManager").GetComponent<GameBehaviour>();
            BG = GameObject.Find("BG");
            isPlayedWin = GameObject.Find("GameManager").GetComponent<GameBehaviour>();
            showWinScreen = GameObject.Find("GameManager").GetComponent<GameBehaviour>();
        }

        private void Update()
        {
            timerStart -= Time.deltaTime;
            timerText.text = Mathf.Round(timerStart).ToString();
            if (timerStart <= 0)
            {
                showLossScreen = true;
                BG.SetActive(false);
                Time.timeScale = 0f;      
            }

            if (showWinScreen.showWinScreen == true)
                if (isPlayedWin.isPlayedWin == false)
                    BG.SetActive(false);
        }        

        private void OnGUI()
        {
            if (showLossScreen)
            {
                if (isPlayedLoss)
                {
                    gameOver.Play();
                }
                Text.labelText = "Ïðîñòðàíñòâî âðåìåíè ëîïíóëî!";
                if (GUI.Button(new Rect(Screen.width / 2 - 250,
                Screen.height / 2 - 50, 400, 100), "ÍÓ ÊÀÊ ÆÅ ÒÀÊ, ÁÎÃÀÒÛÐÜ!"))
                {
                    SceneManager.LoadScene(0);
                    Time.timeScale = 1.0f;
                }
                isPlayedLoss = false;
            }
        }
       

    }
}
