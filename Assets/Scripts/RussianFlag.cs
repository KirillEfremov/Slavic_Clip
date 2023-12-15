using UnityEngine;

namespace Builds
{

    public class RussianFlag : MonoBehaviour
    {
        private Animator _anim;
        private bool _animBool;
        private string _text;
        [SerializeField]
        private Health currentHealth;
        private bool _isTrig;
        [SerializeField]
        private AudioSource _buff;

        private void Start()
        {
            _anim = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "Russian flag")
            {
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
                    _anim.SetBool("isBuff", true);
                    _animBool = true;
                    if (_animBool)
                    {
                        currentHealth.CurrentHealth = 1000;
                        _buff.Play();
                    }
                }
                else
                {
                    _anim.SetBool("isBuff", false);
                    _animBool = false;
                }     
        }
    }
}