using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
	public VisualEffect effect;
	public float duration;

	private float timer;
	private void Start()
	{
		//effect.outputEventReceived += args => { timer = 0f; };
		effect.SetFloat("Duration", duration);
	}

	private void Update()
	{
		timer += Time.deltaTime;
		float progress =Mathf.Clamp01(timer / duration) ;
		
		effect.SetFloat("DissolveAmount", progress);
	}
}
