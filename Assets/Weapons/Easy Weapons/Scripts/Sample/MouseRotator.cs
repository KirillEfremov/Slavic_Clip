﻿using UnityEngine;

namespace Builds
{
	public class MouseRotator : MonoBehaviour
	{

		// A mouselook behaviour with constraints which operate relative to
		// this gameobject's initial rotation.

		// Only rotates around local X and Y.

		// Works in local coordinates, so if this object is parented
		// to another moving gameobject, its local constraints will
		// operate correctly
		// (Think: looking out the side window of a car, or a gun turret
		// on a moving spaceship with a limited angular range)

		// to have no constraints on an axis, set the rotationRange to 360 or greater.

		public Vector2 rotationRange = new Vector3(70, 70);
		public float rotationSpeed = 10;
		public float dampingTime = 0.2f;
		public bool autoZeroVerticalOnMobile = true;
		public bool autoZeroHorizontalOnMobile = false;
		public bool relative = true;
		Vector3 targetAngles;
		Vector3 followAngles;
		Vector3 followVelocity;
		Quaternion originalRotation;


		// Use this for initialization
		void Start()
		{
			originalRotation = transform.localRotation;
		}

		// Update is called once per frame
		void Update()
		{

			// we make initial calculations from the original local rotation
			transform.localRotation = originalRotation;

			// read input from mouse or mobile controls
			float inputH = 0;
			float inputV = 0;
			if (relative)
			{

				inputH = Input.GetAxis("Mouse X");
				inputV = Input.GetAxis("Mouse Y");

				// wrap values to avoid springing quickly the wrong way from positive to negative
				if (targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; }
				if (targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x -= 360; }
				if (targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
				if (targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }

				// with mouse input, we have direct control with no springback required.
				targetAngles.y += inputH * rotationSpeed;
				targetAngles.x += inputV * rotationSpeed;

				// clamp values to allowed range
				targetAngles.y = Mathf.Clamp(targetAngles.y, -rotationRange.y * 0.5f, rotationRange.y * 0.5f);
				targetAngles.x = Mathf.Clamp(targetAngles.x, -rotationRange.x * 0.5f, rotationRange.x * 0.5f);

			}
			else
			{

				inputH = Input.mousePosition.x;
				inputV = Input.mousePosition.y;

				// set values to allowed range
				targetAngles.y = Mathf.Lerp(-rotationRange.y * 0.5f, rotationRange.y * 0.5f, inputH / Screen.width);
				targetAngles.x = Mathf.Lerp(-rotationRange.x * 0.5f, rotationRange.x * 0.5f, inputV / Screen.height);



			}





			// smoothly interpolate current values to target angles
			followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, dampingTime);

			// update the actual gameobject's rotation
			transform.localRotation = originalRotation * Quaternion.Euler(-followAngles.x, followAngles.y, 0);

		}


	}
}
