using System;
using System.Collections;
using UnityEngine;
using GPHive.Core;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
public class SwerveController : MonoBehaviour
{
	private float lastFingerPosX;
	private float swerveAmount;
	private float difference;

	private Vector3 swerveInput;

	[SerializeField] private bool useLimitTransform;

	[ShowIf("useLimitTransform")] [SerializeField]
	private Transform leftLimit,rightLimit;

	private Rigidbody rigidbody;

	private Coroutine              inputResetCoroutine;
	public  SwerveControllerConfig swerveControllerConfig;

	private Vector3 velocity;

	private void Start() { rigidbody=GetComponent<Rigidbody>(); }

	private void Update()
	{
		if(GameManager.Instance.GameState==GameState.Playing)
			Swerve();
	}

	private void FixedUpdate()
	{
		if(GameManager.Instance.GameState==GameState.Playing)
			Movement();
	}

	private void Swerve()
	{
		if(Input.GetMouseButton(0))
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(inputResetCoroutine!=null)
					StopCoroutine(inputResetCoroutine);

				lastFingerPosX=Input.mousePosition.x;
				swerveAmount  =0;
				swerveInput   =Vector3.zero;
			}

			difference=(Input.mousePosition.x-lastFingerPosX)/Screen.width;
			swerveAmount=Mathf.Clamp(
			difference*swerveControllerConfig.Sensitivity*Time.fixedDeltaTime,
			-swerveControllerConfig.SwerveLimit,
			swerveControllerConfig.SwerveLimit);
			swerveInput=Vector3.SmoothDamp(
			swerveInput,
			new Vector3(swerveAmount,0,0),
			ref velocity,
			swerveControllerConfig.Smooth);
			lastFingerPosX=Input.mousePosition.x;
		}

		if(Input.GetMouseButtonUp(0))
		{
			inputResetCoroutine=StartCoroutine(ResetInput(swerveControllerConfig.InputResetTime));

		}
	}

	private Coroutine resetInputCoroutine;

	private void OnDisable() { swerveInput=Vector3.zero; }


	private IEnumerator ResetInput(float time)
	{
		var _time=0f;

		while (_time<time)
		{
			_time      +=Time.deltaTime;
			swerveInput= Vector3.Lerp(swerveInput,Vector3.zero,_time/time);
			yield return null;
		}

		swerveInput=Vector3.zero;
	}

	private void Movement()
	{
		var _verticalMovement  =swerveControllerConfig.forwardSpeed*Time.deltaTime*transform.forward;
		var _horizontalMovement=swerveInput*swerveControllerConfig.HorizontalSpeed;

		_horizontalMovement=
			swerveInput.magnitude<swerveControllerConfig.Threshold ? Vector3.zero : _horizontalMovement;

		var _finalMovement=_verticalMovement+_horizontalMovement;

		if(useLimitTransform)
			if((rigidbody.position+_finalMovement).x<leftLimit.position.x ||
			   (rigidbody.position+_finalMovement).x>rightLimit.position.x)
				_finalMovement.x=0;

		rigidbody.MovePosition(rigidbody.position+_finalMovement);
	}
}
