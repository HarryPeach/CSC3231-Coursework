using System;
using UnityEditor.Presets;
using UnityEngine;

namespace Assets.DayNite
{
	[ExecuteAlways]
	public class LightingController : MonoBehaviour
	{
		[SerializeField] private Light directionalLight;
		[SerializeField] private LightingPresets lightingPreset;
		[SerializeField, Range(0, 24)] private float timeHours;

		private void Update()
		{
			if (lightingPreset == null) return;

			if (Application.isPlaying)
			{
				timeHours += Time.deltaTime;
				timeHours %= 24;
				UpdateLighting(timeHours / 24f);
			}
			else
			{
				UpdateLighting(timeHours / 24f);
			}
		}

		private void UpdateLighting(float timePercent)
		{
			RenderSettings.ambientLight = lightingPreset.AmbientColor.Evaluate(timePercent);
			RenderSettings.fogColor = lightingPreset.AmbientColor.Evaluate(timePercent);

			directionalLight.color = lightingPreset.DirectionalColor.Evaluate(timePercent);
			directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0));
		}

		private void OnValidate()
		{
			if (directionalLight != null) return;
			if (RenderSettings.sun != null)
				directionalLight = RenderSettings.sun;
			else
			{
				Debug.Log("Directional Light was not set, and sun was not found, finding the first directional light in scene.");
				Light[] lights = GameObject.FindObjectsOfType<Light>();
				foreach (Light foundLight in lights)
				{
					if (foundLight.type != LightType.Directional) continue;
					directionalLight = foundLight;
					return;
				}
			}
		}
	}
}
