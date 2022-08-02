using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 20.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 40.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float MaxAcceleration = 10.0f;
		public float MaxVelocity = 10.0f;
		public float MaxAirVelocity = 10.0f;
		public float acceleration = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("Amount of air boosts left")]
		public int AirBoost = 1;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		private PlayerInput _playerInput;
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;


		//Weapons
		public float PrimaryCooldown = 0;
		public float SecondaryCooldown = 0;
		public float busy = 0;


		public GameObject Hand;
		public GameObject Weapon;
		private Animator animator;


		private int BackForwardHash;
		private int LeftRightHash;
		private int IsIdleHash;
		private int IsGroundedHash;
		private int JumpHash;



		private float warping = 0;
		public float Health = 100;
		private float Rally = 100;
		public float RallyMult = 0.1f;
		public float RallyDecay = 10;
		public int bombs = 0;


		public UIScript UI;

		private const float _threshold = 0.01f;
		
		private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;

			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			_playerInput = GetComponent<PlayerInput>();

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			animator = GetComponent<Animator>();
			BackForwardHash = Animator.StringToHash("BackForward");
			LeftRightHash = Animator.StringToHash("LeftRight");
			IsIdleHash = Animator.StringToHash("IsIdle");
			IsGroundedHash = Animator.StringToHash("IsGrounded");
			JumpHash = Animator.StringToHash("Jump");

			Rally = Health;

		}

		private void Update()
		{
			Warp();
			Move();
			CheckCooldowns();
			CheckWeaponFire();
			CheckRally();
			Animate();
			UI.UpdateCD(SecondaryCooldown);
		}

        private void FixedUpdate()
        {
			JumpAndGravity();
			GroundedCheck();
		}


        private void LateUpdate()
		{
			CameraRotation();
		}

		private void GroundedCheck()
		{
			bool before = Grounded;
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
			
			if(before != Grounded)
            {
				animator.SetBool(IsGroundedHash, Grounded);
            }
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
            if (warping > 0) {
				warping -= Time.deltaTime;
				if (warping < 0) warping = 0;
				return;
			}


			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;


			// a reference to the players current horizontal velocity
			Vector3 velocity = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z);
			Vector3 inputDirection = Vector3.zero;

			if (_input.move != Vector2.zero)
			{
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
				inputDirection.Normalize();
			}

			float currentSpeed = Vector3.Dot(velocity, inputDirection);
			float addSpeed = targetSpeed - currentSpeed;


			if (addSpeed <= 0) addSpeed = 0;
			if (addSpeed > MaxAcceleration) addSpeed = MaxAcceleration;

			addSpeed *= Time.deltaTime * acceleration;

			//this improves controll on ground
			if (Grounded) velocity *= 0.99f;


			velocity += (inputDirection * addSpeed);


			if(Grounded && velocity.magnitude > MaxVelocity)
            {
				velocity.Normalize();
				velocity *= MaxVelocity;
			}
			else if (!Grounded && velocity.magnitude > MaxAirVelocity)
			{
				velocity.Normalize();
				velocity *= MaxAirVelocity;
			}


			if (Grounded && _input.move == Vector2.zero)
			{
				velocity *= 0.95f;
			}
			if (!Grounded && _input.move == Vector2.zero)
			{
				velocity *= 0.99f;
			}


			_controller.Move(velocity * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


		}

		private void JumpAndGravity()
		{


			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;
				AirBoost = 1;
				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
					animator.SetTrigger(JumpHash);
				}

				_input.jump = false;

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}



				// Air Boost
				if (_input.jump && _jumpTimeoutDelta <= 0.0f && AirBoost > 0)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity * 1.5f);

					AirBoost--;

					animator.SetTrigger(JumpHash);
				}
				_input.jump = false;

				if (_input.sprint && _jumpTimeoutDelta <= 0.0f && AirBoost > 0)
				{

				}

			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}
		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}


		public void CheckCooldowns()
        {
			if (PrimaryCooldown > 0)
			{
				PrimaryCooldown -= Time.deltaTime;
				if (PrimaryCooldown < 0) PrimaryCooldown = 0;
			}
			if (SecondaryCooldown > 0)
            {
				SecondaryCooldown -= Time.deltaTime;
				if (SecondaryCooldown < 0) SecondaryCooldown = 0;
			}

		}


		public void CheckWeaponFire()
        {

			if (busy <= 0)
			{
				if (_input.fire1) PrimaryFire();
				if (_input.fire2) SecondaryFire();

			}
			else busy -= Time.deltaTime;


			_input.fire1 = false;
			_input.fire2 = false;

		}



		public void PrimaryFire()
		{

			if (Weapon == null) return;
			if (PrimaryCooldown > 0) return;
			PrimaryCooldown = Weapon.GetComponent<WeaponScript>().fireRate;
			Weapon.GetComponent<WeaponScript>().PrimaryFire();


		}

		public void SecondaryFire()
		{
			if (Weapon == null) return;
			if (SecondaryCooldown > 0) return;

			busy = Weapon.GetComponent<WeaponScript>().actionTime;
			SecondaryCooldown = Weapon.GetComponent<WeaponScript>().secondaryCooldown;
			Weapon.GetComponent<WeaponScript>().SecondaryFire();



		}


		public void ChangeWeapon()
        {
			Weapon = Hand.transform.GetChild(Hand.transform.childCount - 1).gameObject;
        }



		public void Animate()
        {

			if(_input.move == Vector2.zero) animator.SetBool(IsIdleHash, true);
			else animator.SetBool(IsIdleHash, false);

			animator.SetFloat(BackForwardHash, _input.move.y);
			animator.SetFloat(LeftRightHash, _input.move.x);

		}

		public void Warp()
        {

			if(_input.warp > 0 && _input.warp < 5)
            {
				warping = 0.05f;

				GameObject warp = GameObject.FindGameObjectsWithTag("WarpPoints")[_input.warp - 1];

				this.transform.position = warp.transform.position;

				_input.warp = 0;
            }
			if(_input.warp == 5)
            {
				GrabBomb();
				_input.warp = 0;
            }


        }



		public void CheckRally()
        {
			if(Health <= 0)
            {
				SceneManager.LoadScene(3);
            }


			if (Rally == Health) return;
			if (Rally < Health)
            {
				Rally = Health;
				return;
            }

			Rally -= Time.deltaTime * RallyDecay;
			UI.UpdateRally(Rally);

        }

		public void Damage(float dmg)
        {
			Health -= dmg;
			UI.UpdateHealth(Health);
        }

		public void GrabBomb()
        {
			bombs++;
			UI.AddBomb();
        }


		public void OnHitEffects(float damage)
        {
			ProcRally(damage);

        }

		public void ProcRally(float damage)
        {
			if(Health < Rally)
            {
				Health += damage * RallyMult;
				if (Health > Rally) Health = Rally;

				UI.UpdateHealth(Health);

            }
        }






	}
}