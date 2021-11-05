using System;
using Terrain;
using UnityEngine;

namespace Player
{
	/// <summary>
	/// Misc player controls such as debug controls, environment changes
	/// </summary>
	public class PlayerMiscController : MonoBehaviour
	{
		[Header("Volcano")] [SerializeField] private ParticleSystem volcanoParticleSystem;
		
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

			if (Input.GetKeyUp("1"))
			{
				TryExplode();
			}

			if (Input.GetKeyUp("2"))
			{
				TryEruptVolanco();
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
				new Rect(DebugFontSize, Screen.height - DebugFontSize * 5, Screen.width, 100),
				"Press 1 for explosion");
			GUI.Label(
				new Rect(DebugFontSize, Screen.height - DebugFontSize * 4, Screen.width, 100),
				"Press 2 for eruption");
			GUI.Label(
				new Rect(DebugFontSize, Screen.height - DebugFontSize * 3, Screen.width, 100),
				$"Ray distance: {_hit.distance}");
			GUI.Label(
				new Rect(DebugFontSize, Screen.height - DebugFontSize * 2, Screen.width, 100),
				$"Ray point: {_hit.point}");
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

		/// <summary>
		/// Sets off an eruption at the volcano in the scene
		/// </summary>
		private void TryEruptVolanco()
		{
			volcanoParticleSystem.Stop();
			volcanoParticleSystem.Clear();
			volcanoParticleSystem.Play();
		}
	}
}