using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

/*===========================================================================
 * 10:57am GMT + 7, 12/10/2023
 * 
 * Scanner script
 * =========================================================================*/

public class Scanner : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] ScannerConfig baseConfig;
	[SerializeField] ScannerConfig lineConfig;
	[SerializeField] ScannerConfig sweepConfig;

	[Header("Sound Config")] 
	[SerializeField] float scanVolumn;

	[Header("Reference")]
	[SerializeField] Transform start;
	[SerializeField] VisualEffect laserVFX;

	public PlayerInput Input {get => input; set => input = value; }

	private ScanManager manager;
	private Camera cam;
	private AudioSource audioSource;
	private PlayerInput input;



	[System.Serializable]
	public class ScannerConfig 
	{
		[Header("Global scan")]
		[Range(1, 500)] public int particleNumber;		//Number of particle created.
		[Range(1, 50)] public int laserDivider;			//Divide the number of lasers created by particle.
		public float range;								//Laser range.
		public bool laserIsVisible;						//Laser only visible when hit something.

		[Header("Base scan")]
		public float radius;							//Radius.	
		public float rangeMul;							//Range based on radius
		public bool isSpread;							//Scanner is spreading or not
		public ScrollConfig scrollConfig;				//Scanner config.

		[Header("Line & Sweep")]
		public ScannerLineConfig lineConfig;			
	}

	[System.Serializable]
	public class ScannerLineConfig
	{
		[Range(0f, 1f)] public float lineWidth;
		public float randomHeight;
		public float sweepSpeed;
	}

	[System.Serializable]
	public class ScrollConfig 
	{
		public bool invertScroll;						//Invert mouse scoll
		public float radiusPower;						//Radius power each roll
		public float radiusSpeed;						//Radius speed
		public float minRadius;							//Minimum radius
		public float maxRadius;							//Maximum radius
	}
}
