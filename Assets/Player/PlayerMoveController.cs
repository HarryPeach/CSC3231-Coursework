using UnityEngine;

namespace Assets.Player
{
	/**
	 * Controls movement of the player
	 */
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMoveController : MonoBehaviour
	{
		[SerializeField] private CharacterController controller;

		[SerializeField] private float speed = 15f;
		[SerializeField] private float gravity = 9.81f;
		[SerializeField] private float jumpHeight = 3f;

		[SerializeField] private Transform groundCheck;
		[SerializeField] private float groundDistance;
		[SerializeField] private LayerMask groundMask;

		private bool _isGrounded;
		private Vector3 _velocity;
		private float _originSpeed;

		private void Start()
		{
			_originSpeed = speed;
		}

		private void Update()
		{
			GroundCheck();
			MovementControl();
		}

		/// <summary>
		/// Checks and sets if the player is on the ground and if so, sets the velocity to a minimum amount
		/// </summary>
		private void GroundCheck()
		{
			_isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

			if (_isGrounded && _velocity.y < 0)
			{
				_velocity.y = -2f;
			}
		}

		/// <summary>
		/// Controls player movement
		/// </summary>
		private void MovementControl()
		{
			float deltaX = Input.GetAxis("Horizontal");
			float deltaZ = Input.GetAxis("Vertical");

			Transform playerTransform = transform;
			Vector3 movementVec = (playerTransform.right * deltaX) + (playerTransform.forward * deltaZ);

			controller.Move(movementVec * (speed * Time.deltaTime));

			if (Input.GetKey("left shift"))
				speed = _originSpeed * 2f;
			else
				speed = _originSpeed;

			if (Input.GetButtonDown("Jump") && _isGrounded) _velocity.y = Mathf.Sqrt(jumpHeight * -2f * -gravity);

			_velocity.y -= gravity * Time.deltaTime;
			controller.Move(_velocity * Time.deltaTime);
		}
	}
}
