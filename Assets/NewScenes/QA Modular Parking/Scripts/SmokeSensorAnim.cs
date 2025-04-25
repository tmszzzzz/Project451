using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeSensorAnim : MonoBehaviour {
	private MeshRenderer Diode; 
	public float EmissionIntensity = 4;
	public float FlashingInterval = 1;
	// Use this for initialization
	void Start () {
		Diode = GetComponent<MeshRenderer> ();
		InvokeRepeating ("DiodeFlashing", FlashingInterval, FlashingInterval);
	}

	void DiodeFlashing (){
		if (Diode.material.GetFloat ("_EmissionIntensity") == 0) {
			Diode.material.SetFloat ("_EmissionIntensity", EmissionIntensity);
		} else {
			Diode.material.SetFloat ("_EmissionIntensity", 0);
		}
	}

}
