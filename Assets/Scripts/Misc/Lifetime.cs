using UnityEngine;

namespace Misc
{
	/// <summary>
	/// Destroys an object after a specified amount of time
	/// </summary>
	public class Lifetime : MonoBehaviour
	{
		[SerializeField] [Tooltip("How long the object should exist for")]
		private float existence = 3;

		private void Update()
		{
			if (existence > 0)
				existence -= Time.deltaTime;
			else
				Destroy(gameObject);
		}
	}
}