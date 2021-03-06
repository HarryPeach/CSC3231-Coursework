using Assets.DayNite;
using UnityEngine;

namespace DayNite
{
	[ExecuteAlways]
	public class LightingController : MonoBehaviour
	{
		[SerializeField] private new bool enabled;
		[SerializeField] private Light directionalLight;
		[SerializeField] private LightingPresets lightingPreset;
		[SerializeField] private float timeStretch = 2f;
		[SerializeField, Range(0, 24)] private float timeHours;

		private void Update()
		{
			if (!enabled) return;
			if (lightingPreset == null) return;

			if (Application.isPlaying)
			{
				timeHours += Time.deltaTime / timeStretch;
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
			directionalLight.transform.localRotation = Quaternion.Euler(new Vector3(timePercent * 360f - 90f, 170, 0));
		}

		private void OnValidate()
		{
			if (directionalLight != null) return;
			if (RenderSettings.sun != null)
				directionalLight = RenderSettings.sun;
		}
	}
}
