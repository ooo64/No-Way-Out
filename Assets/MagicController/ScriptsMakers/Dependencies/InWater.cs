using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;

namespace Invector.CharacterController {
	public class InWater : MonoBehaviour {
		// Start is called before the first frame update
		[TagSelector]
		public string tagWater;


		private void OnTriggerStay(Collider other) {
			if (other.tag == tagWater && gameObject.GetComponentInParent<vThirdPersonMotor>().isflying == false) {
				gameObject.GetComponentInParent<vThirdPersonMotor>().swimming = true;
			}
		}

		private void OnTriggerExit(Collider other) {
			if (other.tag == tagWater) {
				gameObject.GetComponentInParent<vThirdPersonMotor>().swimming = false;
				gameObject.GetComponentInParent<vThirdPersonMotor>().jumpCounter = 0f;
			}
		}
	}
}