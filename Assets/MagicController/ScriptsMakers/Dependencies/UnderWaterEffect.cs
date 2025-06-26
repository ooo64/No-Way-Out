using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnderWaterEffect : MonoBehaviour
{
	public GameObject water_filter;
	[TagSelector]
	public string watertag = "<NoTag>";

	void Start() {
		water_filter.SetActive(false);

	}


	private void OnTriggerEnter(Collider other) {


		if (other.transform.tag == watertag) {

			if (water_filter != null) {

				water_filter.SetActive(true);
			}


		}

	}


	private void OnTriggerExit(Collider other) {


		if (other.transform.tag == watertag) {

			if (water_filter != null) {

				water_filter.SetActive(false);
			}


		}
	}
}
