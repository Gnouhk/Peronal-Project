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
	private Player player;

	private bool isSweeping;
	private float sweepHeight;
	private float targetRadius;

	//Graphics buffer (store target list and add to VFX buffer)
	List<Vector3> targetList;
	GraphicsBuffer positionBuffer;
	static readonly int VFXPositionBufferProperty = Shader.PropertyToID("TargetBuffer");
	public int bufferInitialCapacity = 200;                                                     //Buffer size.

    private void Start()
    {
        cam = Camera.main;
		audioSource = GetComponent<AudioSource>();
		input = GetComponent<PlayerInput>();
		manager = GetComponent<ScanManager>();
		targetRadius = baseConfig.radius;
		player = GetComponent<Player>();	
		CreateNewVFX();
    }

    private void Update()
    {
		ScannerController();

		if (isSweeping)
			HandleSweeping();

		if(targetRadius != baseConfig.radius)
			baseConfig.radius = Mathf.Lerp
			(
			baseConfig.radius, targetRadius, 
			baseConfig.scrollConfig.radiusPower
			);
    }

    private void LateUpdate()
    {
		SetLaser();
    }


    private void ScannerController()
    {
		//Left click scan
		if(Input.actions["Scan"].ReadValue<float>() > 0 && !isSweeping)
        {
			if (!audioSource.isPlaying)
				StartCoroutine(manager.PlaySound(audioSource, scanVolumn));
        }

		//Right click scan
		else if(Input.actions["ScanLine"].ReadValue<float>() > 0 && !isSweeping)
        {
			ScanLine();
			if (!audioSource.isPlaying)
				StartCoroutine(manager.PlaySound(audioSource, scanVolumn));
        }
		else if (audioSource.isPlaying && !isSweeping)
				StartCoroutine(manager.StopSound(audioSource));

		//Sweep scan
		if(Input.actions["Sweep"].triggered && !isSweeping)
        {
			player.enabled = false;
			isSweeping = true;
			sweepHeight = 1f;
        }

		//Change scan radius with mouse scroll
		var _axisPad = input.actions["ScrollPad"].ReadValue<float>();
		if(input.actions["ScrollPad"].triggered)
        {
			var _power = baseConfig.scrollConfig.radiusPower;

			//Smaller
			if(baseConfig.scrollConfig.invertScroll)
				_power = -_power;

			//Add radius
			if(_axisPad > 0)
				targetRadius -= _power;
			else if(_axisPad < 0)
				targetRadius += _power;

			//Change radius
			targetRadius = Mathf.Clamp(targetRadius, baseConfig.scrollConfig.minRadius, baseConfig.scrollConfig.maxRadius);
        }

		//Switch the color palette
		if(input.actions["Change Mode"].triggered)
        {
			manager.SwitchPalette();
        }
    }

    //3 type of scan.
    #region Scanner Type

    private void ScanCircle() 
    {
		for(int i = 0; i < baseConfig.particleNumber; i++)
        {
			//Circle raycast on screen
			RaycastHit _hit;
			Vector3 randomPoint;
			if (baseConfig.useSpread)
				randomPoint = (Random.insideUnitSphere * baseConfig.radius * 10) + (-cam.transform.position * 10) + cam.transform.position;
			else
				randomPoint = (Random.insideUnitSphere.normalized * baseConfig.radius * 9.5f) + (-cam.transform.position * 10) + cam.transform.position;

			Vector3 dir = (cam.transform.position - randomPoint).normalized;
			if (Physics.Raycast(cam.transform.position, dir, out _hit, baseConfig.range - (baseConfig.radius * baseConfig.rangeMul)))
				SetScan(_hit);
			else if (baseConfig.laserIsVisible)
				AddList(cam.transform.position + dir * (baseConfig.range - (baseConfig.radius * baseConfig.rangeMul)));
        }
    }

	private void ScanLine()
    {
		for (int i = 0; i < lineConfig.particleNumber; i++)
		{
			//Line raycast on screen
			var _pos = cam.pixelWidth / 2;
			var _random = Random.Range(-lineConfig.lineConfig.randomHeight, lineConfig.lineConfig.randomHeight);
			RaycastHit _hit;
			Vector3 randomPoint = new Vector3(Random.Range
			(
				_pos - (lineConfig.lineConfig.lineWidth * _pos),
				_pos + (lineConfig.lineConfig.lineWidth * _pos)),
				(cam.pixelHeight / 2f) + _random, 0
			);

			Ray ray = cam.ScreenPointToRay(randomPoint);
			if (Physics.Raycast(cam.transform.position, ray.direction, out _hit, lineConfig.range))
				SetScan(_hit);
			else if (lineConfig.laserIsVisible)
				AddList(cam.transform.position + ray.direction * lineConfig.range);
		}
	}

	private void ScanSweep()
    {
		for (int i = 0; i < sweepConfig.particleNumber; i++)
		{
			//Line raycast on screen
			var _pos = cam.pixelWidth / 2;
			var _random = Random.Range(-sweepConfig.lineConfig.randomHeight, sweepConfig.lineConfig.randomHeight);
			RaycastHit _hit;
			Vector3 randomPoint = new Vector3(Random.Range
			(
				_pos - (sweepConfig.lineConfig.lineWidth * _pos),
				_pos + (sweepConfig.lineConfig.lineWidth * _pos)),
				(cam.pixelHeight * sweepHeight) + _random, 0
			);

			Ray ray = cam.ScreenPointToRay(randomPoint);
			if (Physics.Raycast(cam.transform.position, ray.direction, out _hit, sweepConfig.range))
				SetScan(_hit);
			else if (sweepConfig.laserIsVisible)
				AddList(cam.transform.position + ray.direction * sweepConfig.range);
		}
	}

    #endregion


    
    private void HandleSweeping()
    {
		sweepHeight -= sweepConfig.lineConfig.sweepSpeed * Time.deltaTime;
		ScanSweep();
		
		if(sweepHeight <= 0)
			isSweeping = false;
		if (!audioSource.isPlaying)
			StartCoroutine(manager.PlaySound(audioSource, scanVolumn));
    }

	//Send the scan to ScanManager
	private void SetScan(RaycastHit _hit)
    {
		manager.AddParticle(_hit);
		AddList(_hit.point);
    }

	//Add target to list
	private void AddList(Vector3 _pos)
    {
		targetList.Add(_pos);
    }

	private void CreateNewVFX()
    {
		laserVFX = Instantiate(laserVFX, start.transform);
		targetList = new List<Vector3>(bufferInitialCapacity);
		manager.EnsureBufferCapacity(ref positionBuffer, bufferInitialCapacity, 12, laserVFX, VFXPositionBufferProperty);
    }

	private void SetLaser()
    {
		if (targetList.Count <= 0)
			return;

		manager.EnsureBufferCapacity(ref positionBuffer, targetList.Count, 12, laserVFX, VFXPositionBufferProperty);
		positionBuffer.SetData(targetList);
		laserVFX.SetFloat("LifeTime", 0.05f);
		laserVFX.SetInt("Count", baseConfig.particleNumber / baseConfig.laserDivider);
		laserVFX.Play();
		targetList = new List<Vector3>(bufferInitialCapacity);
    }

    private void OnDestroy()
    {
        manager.ReleaseBuffer(ref positionBuffer);
    }

    //Scanner Config
    #region Config

    [System.Serializable]
	public class ScannerConfig 
	{
		[Header("Global scan")]
		[Range(1, 500)] public int particleNumber;											//Number of particle created.
		[Range(1, 50)] public int laserDivider;												//Divide the number of lasers created by particle.
		public float range;																	//Laser range.
		public bool laserIsVisible;															//Laser only visible when hit something.

		[Header("Base scan")]
		public float radius;																//Radius.	
		public float rangeMul;																//Range based on radius
		public bool useSpread;																//Scanner is spreading or not
		public ScrollConfig scrollConfig;													//Scanner config.

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
		public bool invertScroll;															//Invert mouse scoll
		public float radiusPower;															//Radius power each roll
		public float radiusSpeed;															//Radius speed
		public float minRadius;																//Minimum radius
		public float maxRadius;																//Maximum radius
	}

    #endregion
}
