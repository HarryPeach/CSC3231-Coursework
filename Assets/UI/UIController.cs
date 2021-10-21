using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	[SerializeField] private Text textEl;

	[SerializeField] private float fpsMeasurePeriod = 0.5f;
	private float _fpsNextPeriod;
	private int _fpsAccumulator = 0;
	private int _currentFps;

	private void Start()
	{
		_fpsNextPeriod = Time.realtimeSinceStartup * fpsMeasurePeriod;
		textEl.text = "FPS: Calculating...";
	}

	private void Update()
	{
		getFps();
	}

	private void getFps()
	{
		_fpsAccumulator++;
		if (!(Time.realtimeSinceStartup > _fpsNextPeriod)) return;

		_currentFps = (int)(_fpsAccumulator / fpsMeasurePeriod);
		_fpsAccumulator = 0;
		_fpsNextPeriod += fpsMeasurePeriod;
		textEl.text = $"FPS: {_currentFps}";
	}
}
