using UnityEngine;

namespace Player
{
	/// <summary>
	/// Misc player controls such as debug controls, environment changes
	/// </summary>
	public class PlayerMiscController : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Whether to draw debug information to the screen")]
		private bool debugDraw;

		#region Unity Callbacks

		private void Update()
		{
			if (Input.GetKeyUp("f3"))
			{
				debugDraw = !debugDraw;
			}
			if (Input.GetKeyUp("right shift"))
			{
				TryExplode();
			}
		}
		
		private void FixedUpdate()
		{
			if (!debugDraw) return;
			if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit)) return;
			// ReSharper disable once Unity.UnknownTag
			if (!hit.collider.CompareTag("Terrain")) return;
			Vector3 position = transform.position;
			Debug.DrawLine(position, hit.point, Color.red);
		}

		private void OnGUI()
		{
			if (!debugDraw) return;
			GUI.Label(new Rect(100, 100, 100, 100), "Test");
		}

		#endregion
		
		/// <summary>
		/// Try and explode where the player is looking
		/// </summary>
		private void TryExplode()
		{
			if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit)) return;
			// ReSharper disable once Unity.UnknownTag
			if (!hit.collider.CompareTag("Terrain")) return;
			
			hit.collider.gameObject.GetComponent<TerrainDeformation>()
				.ExplodeTerrain(hit.point, 20f, 1.5f);
		}
	}
}