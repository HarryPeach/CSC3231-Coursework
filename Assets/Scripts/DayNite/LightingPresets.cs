using System;
using UnityEngine;

namespace Assets.DayNite
{
	[Serializable]
	[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Lighting/Lighting Preset")]
	public class LightingPresets : ScriptableObject
	{
		public Gradient AmbientColor;
		public Gradient DirectionalColor;
		public Gradient FogColor;
	}
}
