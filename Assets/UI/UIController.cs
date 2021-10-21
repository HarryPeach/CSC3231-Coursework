using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	[SerializeField] private Text fpsTextElement;
	[SerializeField] private Text memTextElement;
	[SerializeField] private ProfilerRecorder systemUsedMemoryProfilerRecorder;

	[SerializeField] private float fpsMeasurePeriod = 0.5f;
	private float _fpsNextPeriod;
	private int _fpsAccumulator = 0;
	private int _currentFps;

	private void Start()
	{
		_fpsNextPeriod = Time.realtimeSinceStartup * fpsMeasurePeriod;
		fpsTextElement.text = "FPS: Calculating...";
	}

	private void Update()
	{
		GetFps();
		GetMem();
	}

	private void OnEnable()
	{
		systemUsedMemoryProfilerRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
	}

	private void OnDisable()
	{
		systemUsedMemoryProfilerRecorder.Dispose();
	}

	/// <summary>
	/// Get the current FPS and set the fps label to display it
	/// </summary>
	private void GetFps()
	{
		_fpsAccumulator++;
		if (!(Time.realtimeSinceStartup > _fpsNextPeriod)) return;

		_currentFps = (int)(_fpsAccumulator / fpsMeasurePeriod);
		_fpsAccumulator = 0;
		_fpsNextPeriod += fpsMeasurePeriod;
		fpsTextElement.text = $"FPS: {_currentFps}";
	}

	/// <summary>
	/// Get the current memory usage and set the fps label to display it
	/// </summary>
	private void GetMem()
	{
		memTextElement.text = $"Total Memory Usage: {systemUsedMemoryProfilerRecorder.LastValue / (1024 * 1024)}MB";
	}
}
