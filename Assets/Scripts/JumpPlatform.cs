﻿using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour {

	public float JumpMagnitude=30.0f;


	public void ControllerEnter2D(CharacterController2D controller){
		
		controller.SetVerticalForce (JumpMagnitude);
	}
}
