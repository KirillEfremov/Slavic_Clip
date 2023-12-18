using UnityEngine;
using UnityEngine.SceneManagement;

namespace Builds
{
    public class Animate : MonoBehaviour
    {
        private Animator _anim;
        [SerializeField]
        private GameObject _player;
        private GameObject BG;
        [SerializeField]
        private AudioSource _gameOver;
        private bool _isPlayedDie = true;
        private bool _showLossScreen = false;
        private GameBehaviour _text;

        private void Start()
        {
            _anim = GetComponent<Animator>();
            BG = GameObject.Find("BG");
            _text = GameObject.Find("GameManager").GetComponent<GameBehaviour>();
        }

        private void Update()
        {
            if (_player.transform.position.y <= -10)
            {
                _showLossScreen = true;
                if (_player.transform.position.y <= -100)
                {
                    Time.timeScale = 0f;
                }
            }
            
            if (Input.GetKey(KeyCode.W))
                _anim.SetBool("isRunning", true);

            else
                _anim.SetBool("isRunning", false);


            if (Input.GetKey(KeyCode.S))
                _anim.SetBool("isBackRunning", true);

            else
                _anim.SetBool("isBackRunning", false);

            if (Input.GetKey(KeyCode.A))
                _anim.SetBool("isLeft", true);

            else
                _anim.SetBool("isLeft", false);

            if (Input.GetKey(KeyCode.D))
                _anim.SetBool("isRight", true);

            else
                _anim.SetBool("isRight", false);

            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
                _anim.SetBool("isLeftBack", true);

            else
                _anim.SetBool("isLeftBack", false);

            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
                _anim.SetBool("isRightBack", true);

            else
                _anim.SetBool("isRightBack", false);
        }

        private void OnGUI()
        {
            if (_showLossScreen)
            {
                LossScreen();
            }
        }

        public void LossScreen()
        {
            _anim.SetBool("isDeath", true);
            BG.SetActive(false);
            if (_isPlayedDie)
                {
                    _gameOver.Play();
                }
                _text._labelText = "Áåñêîíå÷íî-âå÷íîå áëèçêî!";
                if (GUI.Button(new Rect(Screen.width / 2 - 250,
                Screen.height / 2 - 50, 400, 100), "ÍÓ ÊÀÊ ÆÅ ÒÀÊ, ÁÎÃÀÒÛÐÜ!"))
                {
                    SceneManager.LoadScene(0);
                    Time.timeScale = 1.0f;
                }
                _isPlayedDie = false;    
        }
    }
}
