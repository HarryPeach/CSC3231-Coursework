using UnityEngine;

namespace Player
{
	/// <summary>
	/// Controls the first person players mouse look
	/// </summary>
	public class PlayerLookController : MonoBehaviour
	{
		[SerializeField] private float verticalSensitivity = 200f;

		[SerializeField] private float horizontalSensitivity = 300f;

		private float _rotationX;
		private float _rotationY;

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			float mouseX = Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
			float mouseY = Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

			_rotationX -= mouseY;
			_rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

			_rotationY += mouseX;

			transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
		}
	}
}
