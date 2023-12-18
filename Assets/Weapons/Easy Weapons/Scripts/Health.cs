using System.Collections;
using UnityEngine;
using Mirror;

namespace Builds
{
	public class Health : NetworkBehaviour
	{
		public bool _canDie = true;                  

        private int _startingHealth = 1000;       
        private int _maxHealth = 1000;
        [SyncVar]                               
        private int _currentHealth;                

		public bool _replaceWhenDead = false;        
		public GameObject _deadReplacement;          
		public bool _makeExplosion = false;          
		public GameObject _explosion;                

		public bool _isPlayer = false;               
		public GameObject _deathCam;                 

		private bool dead = false;                  
		private Animate _lossScreen;
		private bool _lossTrue;
       
        void Start()
		{
			_currentHealth = _startingHealth;
			_lossScreen = GetComponent<Animate>();  
        }

        public int CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value;
            }
        }

        public void ChangeHealth(int amount)
		{
			// Change the health by the amount specified in the amount variable
			_currentHealth += amount;

			// If the health runs out, then Die.
			if (_currentHealth <= 0 && !dead && _canDie)
				Die();

			// Make sure that the health never exceeds the maximum health
			else if (_currentHealth > _maxHealth)
				_currentHealth = _maxHealth;
		}

		public void Die()
		{
			// This GameObject is officially dead.  This is used to make sure the Die() function isn't called again
			dead = true;

			// Make death effects
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
            "Силушка богатырская:" + _currentHealth, myStyle);
            if (_lossTrue)
			{
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
