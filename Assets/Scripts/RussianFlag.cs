using UnityEngine;

namespace Builds
{

    public class RussianFlag : MonoBehaviour
    {
        private Animate _anim;
        private bool _animBool;
        private string _text;
        private PlayerControl _currentHealth;
        private bool _isTrig;
        [SerializeField]
        private AudioSource _buff;

        private void Start()
        {
            _currentHealth = GetComponent<PlayerControl>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                _anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animate>();
                _currentHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
                _text = "Остановись, путник, и нажми клавишу F заморскую. Получишь силушку богатырскую!";
                _isTrig = true;
            }
        }
        void OnGUI()
        {
            GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height - 200,
            300, 150), _text);
        }

        private void OnTriggerExit(Collider other)
        {
             _text = "";
             _isTrig = false;
        }

        private void Update()
        {
            if (_isTrig)
                if (Input.GetKeyDown(KeyCode.F))
                {
                    _anim._anim.SetBool("isBuff", true);
                    _animBool = true;
                    if (_animBool)
                    {
                        _currentHealth.CurrentHealth = 1000;
                        _buff.Play();
                    }
                }
                else
                {
                    _anim._anim.SetBool("isBuff", false);
                    _animBool = false;
                }     
        }
    }
}