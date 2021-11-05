using System;
using UnityEngine;

namespace Player
{
	/// <summary>
	/// Misc player controls such as debug controls, environment changes
	/// </summary>
	public class PlayerMiscController : MonoBehaviour
	{
		[Header("Terrain Deformation")] [SerializeField]
		private GameObject explosionPrefab;

		[SerializeField] private float blastSize = 20f;
		[SerializeField] private float scorchFactor = 1.5f;

		[Header("Debug")]
		[SerializeField]
		[Tooltip("Whether to draw debug information to the screen")]
		private bool debugDraw;

		private const int DebugFontSize = 12;

		private RaycastHit _hit;

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
			if (!Physics.Raycast(transform.position, transform.forward, out _hit)) return;
			// ReSharper disable once Unity.UnknownTag
			if (!_hit.collider.CompareTag("Terrain")) return;
			Vector3 position = transform.position;
			Debug.DrawLine(position, _hit.point, Color.red);
		}

		private void OnGUI()
		{
			if (!debugDraw) return;
			GUI.skin.label.fontSize = DebugFontSize;
			GUI.Label(
				new Rect(DebugFontSize, Screen.height - (DebugFontSize * 2), Screen.width, 100),
				$"Ray distance: {_hit.distance}");
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

			// Spawn an explosion effect
			explosionPrefab.transform.localScale = new Vector3(blastSize / 5f, blastSize / 5f, blastSize / 5f);
			Instantiate(explosionPrefab, hit.point, Quaternion.identity);

			// Perform terrain deformation
			hit.collider.gameObject.GetComponent<TerrainDeformation>()
				.ExplodeTerrain(hit.point, blastSize, scorchFactor);
		}
	}
}