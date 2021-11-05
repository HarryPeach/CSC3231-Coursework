using UnityEngine;

namespace Player
{
	/**
	 * Controls the players mouse look
	 */
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

			if (Input.GetMouseButtonDown(0)) TryExplode();
		}

		/// <summary>
		/// Try and explode where the player is looking
		/// </summary>
		private void TryExplode()
		{
			if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit)) return;
			if (!hit.collider.CompareTag("Terrain")) return;
			
			hit.collider.gameObject.GetComponent<TerrainDeformation>()
				.ExplodeTerrain(hit.point, 10f, 1.5f);
		}

		private void FixedUpdate()
		{
			if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit)) return;
			if (!hit.collider.CompareTag("Terrain")) return;
			Vector3 position = transform.position;
			Debug.DrawLine(position, hit.point, Color.red);
		}
	}
}
