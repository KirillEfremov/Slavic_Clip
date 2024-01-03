using UnityEngine;
using Mirror;

namespace Builds
{
    public class Animate : NetworkBehaviour
    {
        public Animator _anim;
        
        private void Start()
        {
            _anim = GetComponent<Animator>();
        }
        private void Update()
        {    
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
    }
}
