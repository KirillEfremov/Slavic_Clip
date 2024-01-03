using System.Collections;
using UnityEngine;
using Mirror;

namespace Builds
{
	public class Health : NetworkBehaviour
	{
		public bool _canDie = true;                      
        private int _maxHealth = 1000;
		private PlayerControl _currentHealth;
        public bool _replaceWhenDead = false;        
		public GameObject _deadReplacement;          
		public bool _makeExplosion = false;          
		public GameObject _explosion;                
		public bool _isPlayer = false;               
		public GameObject _deathCam;
		private bool dead = false;                  
		private PlayerControl _lossScreen;
		private bool _lossTrue;
        private PlayerControl _showLossScreen;

        void Start()
		{		
			_lossScreen = GetComponent<PlayerControl>();  
			_currentHealth = GetComponent<PlayerControl>();
            _showLossScreen = GetComponent<PlayerControl>();
        }

        [Command(requiresAuthority = false)]
        private void ChangeHealth(int amount)
        {
            RpcChangeHealth(amount);
        }

        [ClientRpc]
        public void RpcChangeHealth(int amount)
		{
            // Измените текеущее здоровье на величину, указанную в переменной amount
            _currentHealth.CurrentHealth += amount;

            // Если здоровье иссякнет, то умрешь
            if (_currentHealth.CurrentHealth <= 0 && !dead && _canDie)
            {
                Die();
            }

            // Убедитесь, что запас здоровья никогда не превышает максимального значения
            else if (_currentHealth.CurrentHealth > _maxHealth)
                _currentHealth.CurrentHealth = _maxHealth;
		}

        public void Die()
		{
            // Этот игровой объект официально мертв. Это используется для того, чтобы убедиться, что функция Die() больше не вызывается
            dead = true;

            // Создавать смертельные эффекты
            if (_replaceWhenDead)
				Instantiate(_deadReplacement, transform.position, transform.rotation);
			if (_makeExplosion)
				Instantiate(_explosion, transform.position, transform.rotation);

			if (_isPlayer && _deathCam != null)
				_deathCam.SetActive(true);
			_lossTrue = true;
        }

        private void OnGUI()
        {
            GUIStyle myStyle = new GUIStyle();
			myStyle.fontSize = 40;
            myStyle.fontStyle = FontStyle.Bold;
            myStyle.normal.textColor = Color.white;
            GUI.Box(new Rect(1350, 25, 250, 50),
            "Силушка богатырская:" + _currentHealth.CurrentHealth, myStyle);
            if (_lossTrue)
			{
                _showLossScreen._showLossScreen = true;
                StartCoroutine(BadEnd());
                _lossScreen.LossScreen();
            }
        }
		IEnumerator BadEnd()
		{
			if (_lossTrue)
			{
                 yield return new WaitForSeconds(2);
                 Time.timeScale = 0f;
            }   
        }
    }
}
