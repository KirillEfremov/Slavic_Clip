/// <summary>
/// Health.cs
/// Author: MutantGopher
/// This is a sample health script.  If you use a different script for health,
/// make sure that it is called "Health".  If it is not, you may need to edit code
/// referencing the Health component from other scripts
/// </summary>

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Builds
{
	public class Health : MonoBehaviour
	{
		public bool canDie = true;                  // Whether or not this health can die

		public int startingHealth = 1000;       // The amount of health to start with
		public int maxHealth = 1000;            // The maximum amount of health
		public int currentHealth;                // The current ammount of health

		public bool replaceWhenDead = false;        // Whether or not a dead replacement should be instantiated.  (Useful for breaking/shattering/exploding effects)
		public GameObject deadReplacement;          // The prefab to instantiate when this GameObject dies
		public bool makeExplosion = false;          // Whether or not an explosion prefab should be instantiated
		public GameObject explosion;                // The explosion prefab to be instantiated

		public bool isPlayer = false;               // Whether or not this health is the player
		public GameObject deathCam;                 // The camera to activate when the player dies

		private bool dead = false;                  // Used to make sure the Die() function isn't called twice
		private Animate _lossScreen;
		private bool _lossTrue;


        // Use this for initialization
        void Start()
		{
			currentHealth = startingHealth;
			_lossScreen = GetComponent<Animate>();
        }

        public int CurrentHealth
        {
            get { return currentHealth; }
            set
            {
                currentHealth = value;
            }
        }

        public void ChangeHealth(int amount)
		{
			// Change the health by the amount specified in the amount variable
			currentHealth += amount;

			// If the health runs out, then Die.
			if (currentHealth <= 0 && !dead && canDie)
				Die();

			// Make sure that the health never exceeds the maximum health
			else if (currentHealth > maxHealth)
				currentHealth = maxHealth;
		}

		public void Die()
		{
			// This GameObject is officially dead.  This is used to make sure the Die() function isn't called again
			dead = true;

			// Make death effects
			if (replaceWhenDead)
				Instantiate(deadReplacement, transform.position, transform.rotation);
			if (makeExplosion)
				Instantiate(explosion, transform.position, transform.rotation);

			if (isPlayer && deathCam != null)
				deathCam.SetActive(true);
			_lossTrue = true;
        }

        private void OnGUI()
        {
            GUIStyle myStyle = new GUIStyle();
			myStyle.fontSize = 40;
            myStyle.fontStyle = FontStyle.Bold;
            myStyle.normal.textColor = Color.white;
            GUI.Box(new Rect(1350, 25, 250, 50),
            "Силушка богатырская:" + currentHealth, myStyle);
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
