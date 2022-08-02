using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool fire1;
		public bool fire2;
		public int warp;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnFire1(InputValue value)
        {
			Fire1Input(value.isPressed);
        }
		public void OnFire2(InputValue value)
		{
			Fire2Input(value.isPressed);
		}

		public void OnWarp1(InputValue value)
        {
			if(value.isPressed)
				WarpInput(1);
        }
		public void OnWarp2(InputValue value)
		{
			if (value.isPressed)
				WarpInput(2);
		}

		public void OnWarp3(InputValue value)
		{
			if (value.isPressed)
				WarpInput(3);
		}
		public void OnWarp4(InputValue value)
		{
			if (value.isPressed)
				WarpInput(4);
		}
		public void OnWarp5(InputValue value)
		{
			if (value.isPressed)
				WarpInput(5);
		}

#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void Fire1Input(bool newFire1)
        {
			fire1 = newFire1;
        }
		public void Fire2Input(bool newFire2)
		{
			fire2 = newFire2;
		}

		public void WarpInput(int newWarp)
		{
			warp = newWarp;

		}

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}