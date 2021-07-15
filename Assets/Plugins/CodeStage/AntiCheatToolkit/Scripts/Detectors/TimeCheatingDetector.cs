#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
#define ACTK_DEBUG_ENABLED
#endif

#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0)
#define ACTK_UWP
#endif

#define ACTK_DETECTOR_ENABLED

// disabling this detector if ACTK_PREVENT_INTERNET_PERMISSION compilation flag was set
#if ACTK_PREVENT_INTERNET_PERMISSION
#undef ACTK_DETECTOR_ENABLED
#endif

// disabling this detector if target platform is Windows Universal Platform
// and scripting backend is not an IL2CPP
#if ACTK_UWP && !ENABLE_IL2CPP
#undef ACTK_DETECTOR_ENABLED
#endif

// disabling this detector at WebGL since sockets do not work at WebGL:
// https://docs.unity3d.com/Manual/webgl-networking.html
#if UNITY_WEBGL || UNITY_WEBGL_API
#undef ACTK_DETECTOR_ENABLED
#endif


#if NET_4_6 && !UNITY_WEBGL && !ACTK_UWP
#define ASYNC_AWAIT_ENABLED
#endif

#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using System;
using System.Collections;
using CodeStage.AntiCheat.Common;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if ACTK_DETECTOR_ENABLED
using System.Net;
using System.Net.Sockets;
#endif

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Allows to detect time cheating using time servers. Needs Internet connection.
	/// </summary>
	/// Doesn't detects cheating if there is no Internet connection or if it's too weak to gather time from time servers.<br/>
	/// Just add it to any GameObject as usual or through the "GameObject > Create Other > Code Stage > Anti-Cheat Toolkit"
	/// menu to get started.<br/>
	/// You can use detector completely from inspector without writing any code except the actual reaction on cheating.
	/// 
	/// Avoid using detectors from code at the Awake phase.<br/>
	/// 
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly WebGL and UWP with .NET backend are not supported!</strong>
	[AddComponentMenu(MenuPath + ComponentName)]
	[HelpURL(ACTkConstants.DocsRootUrl + "class_code_stage_1_1_anti_cheat_1_1_detectors_1_1_time_cheating_detector.html")]
	public class TimeCheatingDetector : ActDetectorBase
	{
		public enum TimeCheatingDetectorResult
		{
			Unknown = 0,
			CheckPassed = 5,
			CheatDetected = 10,
			Error = 15
		}

		public enum ErrorKind
		{
			NoError = 0,
			CantResolveHost = 5,
			Unknown = 10
		}

		internal const string ComponentName = "Time Cheating Detector";
		private const string LogPrefix = ACTkConstants.LogPrefix + ComponentName + ": ";

#if ACTK_DETECTOR_ENABLED
		private static int instancesInScene;
		private const int NtpDataBufferLength = 48;

		#region public fields and properties

		/// <summary>
		/// Event to raise when error occurs while making a cheat check.
		/// </summary>
		public event System.Action<ErrorKind> Error;

		/// <summary>
		/// Event to raise when cheat check passes successfully and there is no cheat detected.
		/// </summary>
		public event System.Action CheckPassed;

		/// <summary> 
		/// Time (in minutes) between detector checks. Set to 0 to disable automatic time checks and use
		/// ForceCheck(), ForceCheckEnumerator() or ForceCheckTask() to manually run a check.
		/// </summary>
		[Tooltip("Time (in minutes) between detector checks.")]
		[Range(0f, 60)]
		public float interval = 1;

		/// <summary>
		/// Maximum allowed difference between online and offline time, in minutes.
		/// </summary>
		[Tooltip("Maximum allowed difference between online and offline time, in minutes.")]
		public int threshold = 65;

		/// <summary>
		/// Time server to use for the time checks.
		/// </summary>
		public string timeServer = "pool.ntp.org";

		/// <summary>
		/// Allows to check if cheating check is currently in process.
		/// </summary>
		public bool IsCheckingForCheat { get; private set; }

		/// <summary>
		/// Last check error is exists.
		/// </summary>
		public ErrorKind LastError { get; private set; }

		/// <summary>
		/// Last check result. Check #LastError if this has value of Error.
		/// </summary>
		public TimeCheatingDetectorResult LastResult { get; private set; }

		#endregion

		#region private and protected fields
		private readonly DateTime date1900 = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private readonly WaitForEndOfFrame cachedEndOfFrame = new WaitForEndOfFrame();

		private Socket asyncSocket;
		private byte[] ntpData = new byte[NtpDataBufferLength];
		private byte[] targetIP;
		private IPEndPoint targetEndpoint;
		
		private SocketAsyncEventArgs connectArgs;
		private SocketAsyncEventArgs sendArgs;
		private SocketAsyncEventArgs receiveArgs;

		private float timeElapsed;
		private double lastOnlineTime;
		private bool gettingOnlineTimeAsync;
		private ErrorKind asyncError;

		#endregion

		#region static instance

		/// <summary>
		/// Allows reaching public properties from code.
		/// Can be null if detector does not exist in scene or if accessed at Awake phase.
		/// </summary>
		public static TimeCheatingDetector Instance { get; private set; }

		private static TimeCheatingDetector GetOrCreateInstance
		{
			get
			{
				if (Instance != null)
					return Instance;

				if (detectorsContainer == null)
				{
					detectorsContainer = new GameObject(ContainerName);
				}
				Instance = detectorsContainer.AddComponent<TimeCheatingDetector>();
				return Instance;
			}
		}

		#endregion

		#region public static methods
		[Obsolete("Please use StartDetection(int, ...) instead.")]
		public static void StartDetection(System.Action detectionCallback, int interval)
		{
			StartDetection(interval, detectionCallback);
		}

		[Obsolete("Please use StartDetection(int, ...) instead.")]
		public static void StartDetection(System.Action detectionCallback, System.Action<ErrorKind> errorCallback, int interval)
		{
			StartDetection(interval, detectionCallback, errorCallback);
		}

		/// <summary>
		/// Creates new instance of the detector at scene if it doesn't exists. Make sure to call NOT from Awake phase.
		/// </summary>
		/// <returns>New or existing instance of the detector.</returns>
		public static TimeCheatingDetector AddToSceneOrGetExisting()
		{
			return GetOrCreateInstance;
		}

		/// <summary>
		/// Starts detection with specified callback.
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="detectionCallback">Method to call after detection. Pass null if you wish to use event, set in detector inspector.</param>
		/// <param name="errorCallback">Method to call if detector will be not able to retrieve online time.</param>
		/// <param name="checkPassedCallback">Method to call after successful cheat check pass.</param>
		public static void StartDetection(System.Action detectionCallback = null, System.Action<ErrorKind> errorCallback = null, System.Action checkPassedCallback = null)
		{
			if (detectionCallback == null)
			{
				if (Instance != null)
				{
					Instance.StartDetectionInternal(Instance.interval, null, checkPassedCallback, errorCallback);
				}
				else
				{
					Debug.LogError(LogPrefix + "can't be started since it doesn't exists in scene or not yet initialized!");
				}
			}
			else
			{
				StartDetection(GetOrCreateInstance.interval, detectionCallback, errorCallback, checkPassedCallback);
			}
		}

		/// <summary>
		/// Starts detection with specified callback using passed interval.<br/>
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="intervalMinutes">Time in minutes between checks. Overrides #interval property.</param>
		/// <param name="detectionCallback">Method to call after detection.</param>
		/// <param name="errorCallback">Method to call if detector will be not able to retrieve online time.</param>
		/// <param name="checkPassedCallback">Method to call after successful cheat check pass.</param>
		public static void StartDetection(float intervalMinutes, System.Action detectionCallback = null, System.Action<ErrorKind> errorCallback = null, System.Action checkPassedCallback = null)
		{
			GetOrCreateInstance.StartDetectionInternal(intervalMinutes, detectionCallback, checkPassedCallback, errorCallback);
		}

		/// <summary>
		/// Stops detector. Detector's component remains in the scene. Use Dispose() to completely remove detector.
		/// </summary>
		public static void StopDetection()
		{
			if (Instance != null) Instance.StopDetectionInternal();
		}

		[Obsolete("Please use Instance.Error event instead.", true)]
		public static void SetErrorCallback(System.Action<ErrorKind> errorCallback)
		{
			
		}

		/// <summary>
		/// Stops and completely disposes detector component.
		/// </summary>
		/// On dispose Detector follows 2 rules:
		/// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically 
		/// destroyed if no other Detectors left attached regardless of any other components or children;<br/>
		/// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
		/// if it has neither other components nor children attached;
		public static void Dispose()
		{
			if (Instance != null) Instance.DisposeInternal();
		}

		#endregion

		#region Unity messages
#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void Awake()
		{
			instancesInScene++;
			if (Init(Instance, ComponentName))
			{
				Instance = this;
			}

#if UNITY_5_4_OR_NEWER
			SceneManager.sceneLoaded += OnLevelWasLoadedNew;
#endif
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		protected override void OnDestroy()
		{
			base.OnDestroy();
			instancesInScene--;
		}

#if UNITY_5_4_OR_NEWER
		private void OnLevelWasLoadedNew(Scene scene, LoadSceneMode mode)
		{
			OnLevelLoadedCallback();
		}
#else
#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnLevelWasLoaded(int level)
		{
			OnLevelLoadedCallback();
		}
#endif

		private void OnLevelLoadedCallback()
		{
			if (instancesInScene < 2)
			{
				if (!keepAlive)
				{
					DisposeInternal();
				}
			}
			else
			{
				if (!keepAlive && Instance != this)
				{
					DisposeInternal();
				}
			}
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnApplicationPause(bool pauseStatus)
		{
			if (!started) return;

			if (pauseStatus)
			{
				PauseDetector();
			}
			else
			{
				ResumeDetector();
			}
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void Update()
		{
			if (!started || !isRunning) return;

			if (interval > 0)
			{
				timeElapsed += Time.unscaledDeltaTime;
				if (timeElapsed >= interval * 60)
				{
					timeElapsed = 0;
					StartCoroutine(CheckForCheat());
				}
			}
		}

		#endregion

		#region public instance methods
		/// <summary>
		/// Allows to manually execute cheating check. Restarts #interval.
		/// </summary>
		/// Listen for detector events to know about check result.
		public bool ForceCheck()
		{
			if (!started || !isRunning)
			{
				Debug.LogWarning(LogPrefix + "Detector should be started to use ForceCheck().");
				LastError = ErrorKind.Unknown;
				LastResult = TimeCheatingDetectorResult.Error;
				return false;
			}

			if (IsCheckingForCheat)
			{
				Debug.LogWarning(LogPrefix + "Can't force cheating check since another check is already in progress.");
				LastError = ErrorKind.Unknown;
				LastResult = TimeCheatingDetectorResult.Error;
				return false;
			}

			if (!DetectorHasCallbacks())
			{
				Debug.LogWarning(LogPrefix + "Can't force cheating check since you have no any callbacks set to know check result.");
				LastError = ErrorKind.Unknown;
				LastResult = TimeCheatingDetectorResult.Error;
				return false;
			}

			timeElapsed = 0;
			StartCoroutine(CheckForCheat());

			return true;
		}

		/// <summary>
		/// Allows to manually execute cheating check and wait for the completion within coroutine. Restarts #interval.
		/// </summary>
		/// Use inside of the coroutine and check #LastResult property after yielding this method.
		/// Detector events will be invoked too.
		/// Example:
		/// \code
		/// StartCoroutine(MakeForcedCheck());
		/// private IEnumerator MakeForcedCheck()
		/// {
		///	    yield return TimeCheatingDetector.Instance.ForceCheckEnumerator();
		///	    // check TimeCheatingDetector.Instance.LastResult
		///	    // ...
		/// }
		/// \endcode
		public IEnumerator ForceCheckEnumerator()
		{
			if (!started || !isRunning)
			{
				Debug.LogWarning(LogPrefix + "Detector should be started to use ForceCheck().");
				LastError = ErrorKind.Unknown;
				LastResult = TimeCheatingDetectorResult.Error;
				yield break;
			}

			if (IsCheckingForCheat)
			{
				while (IsCheckingForCheat)
				{
					yield return cachedEndOfFrame;
				}
			}

			if (!DetectorHasCallbacks())
			{
				Debug.LogWarning(LogPrefix + "Can't force cheating check since you have no any callbacks set to know check result.");
				LastError = ErrorKind.Unknown;
				LastResult = TimeCheatingDetectorResult.Error;
				yield break;
			}

			timeElapsed = 0;
			yield return StartCoroutine(CheckForCheat());
		}

#if ASYNC_AWAIT_ENABLED
		/// <summary>
		/// Allows to manually execute cheating check and wait for the completion within async method. Restarts #interval.
		/// </summary>
		/// Use inside of the async method and check #LastResult property after awaiting this method.
		/// Detector events will be invoked too.
		/// Doesn't exists for WebGL platform.
		/// \code
		/// MakeForcedCheckAsync();
		/// private async void MakeForcedCheckAsync()
		/// {
		///	    var result = await TimeCheatingDetector.Instance.ForceCheckTask();
		///	    // check result or TimeCheatingDetector.Instance.LastResult
		///	    // ...
		/// }
		/// \endcode
		public async System.Threading.Tasks.Task<TimeCheatingDetectorResult> ForceCheckTask()
		{
			if (!started || !isRunning)
			{
				Debug.LogWarning(LogPrefix + "Detector should be started to use ForceCheck().");
				LastError = ErrorKind.Unknown;
				LastResult = TimeCheatingDetectorResult.Error;
				return LastResult;
			}

			if (IsCheckingForCheat)
			{
				while (IsCheckingForCheat)
				{
					await System.Threading.Tasks.Task.Delay(10);
				}
			}

			if (!DetectorHasCallbacks())
			{
				Debug.LogWarning(LogPrefix + "Can't force cheating check since you have no any callbacks set to know check result.");
				LastError = ErrorKind.Unknown;
				LastResult = TimeCheatingDetectorResult.Error;
				return LastResult;
			}

			timeElapsed = 0;
			StartCoroutine(CheckForCheat());

			await System.Threading.Tasks.Task.Delay(10);

			if (IsCheckingForCheat)
			{
				while (IsCheckingForCheat)
				{
					await System.Threading.Tasks.Task.Delay(100);
				}
			}

			return LastResult;
		}
#endif

		#endregion

		private void StartDetectionInternal(float checkInterval, System.Action detectionCallback, System.Action checkPassedCallback, System.Action<ErrorKind> errorCallback)
		{
			if (isRunning)
			{
				Debug.LogWarning(LogPrefix + "already running!", this);
				return;
			}

			if (!enabled)
			{
				Debug.LogWarning(
					LogPrefix + "disabled but StartDetection still called from somewhere (see stack trace for this message)!",
					this);
				return;
			}

			if (detectionCallback != null && detectionEventHasListener)
			{
				Debug.LogWarning(
					LogPrefix +
					"has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?",
					this);
			}

			if (detectionCallback == null && !detectionEventHasListener)
			{
				Debug.LogWarning(
					LogPrefix +
					"was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.",
					this);
				enabled = false;
				return;
			}

			timeElapsed = 0;
			CheatDetected += detectionCallback;
			if (errorCallback != null) Error += errorCallback;
			if (checkPassedCallback != null) CheckPassed += checkPassedCallback;
			interval = checkInterval;

			started = true;
			isRunning = true;
		}

		protected override void StartDetectionAutomatically()
		{
			StartDetectionInternal(interval, null, null, null);
		}

		protected override bool DetectorHasCallbacks()
		{
			return base.DetectorHasCallbacks() || Error != null || CheckPassed != null;
		}

		protected override void PauseDetector()
		{
			base.PauseDetector();

			timeElapsed = 0;
		}

		protected override void StopDetectionInternal()
		{
			base.StopDetectionInternal();

			Error = null;
			CheckPassed = null;
			CloseSocket();
		}

		protected override void DisposeInternal()
		{
			if (Instance == this) Instance = null;
			
			base.DisposeInternal();
		}

		private IEnumerator CheckForCheat()
		{
			if (!isRunning || IsCheckingForCheat) yield break;

			IsCheckingForCheat = true;

			LastError = ErrorKind.NoError;
			LastResult = TimeCheatingDetectorResult.Unknown;

			yield return StartCoroutine(GetOnlineTimeInternal());

			if (!started || !isRunning)
			{
				LastError = ErrorKind.Unknown;
			}

			if (lastOnlineTime <= 0 && LastError == ErrorKind.NoError)
			{
				LastError = ErrorKind.Unknown;
			}

			if (LastError != ErrorKind.NoError)
			{
				LastResult = TimeCheatingDetectorResult.Error;
				if (Error != null) Error.Invoke(LastError);
				IsCheckingForCheat = false;
				yield break;
			}

			var offlineTime = GetLocalTime();
			var onlineTimeSpan = new TimeSpan((long)lastOnlineTime * TimeSpan.TicksPerMillisecond);
			var offlineTimeSpan = new TimeSpan((long)offlineTime * TimeSpan.TicksPerMillisecond);

			var minutesDifference = onlineTimeSpan.TotalMinutes - offlineTimeSpan.TotalMinutes;
			if (Math.Abs(minutesDifference) > threshold)
			{
				LastResult = TimeCheatingDetectorResult.CheatDetected;
				OnCheatingDetected();
			}
			else
			{
				LastResult = TimeCheatingDetectorResult.CheckPassed;
				if (CheckPassed != null) CheckPassed.Invoke();
			}

			IsCheckingForCheat = false;
		}

		private IEnumerator GetOnlineTimeInternal()
		{
			gettingOnlineTimeAsync = false;
			lastOnlineTime = 0;
			asyncError = ErrorKind.NoError;

			IPAddress[] addresses = null;
			try
			{
				addresses = Dns.GetHostEntry(timeServer).AddressList;
			}
			catch (Exception exception)
			{
				Debug.LogWarning(LogPrefix + "Could not resolve host " + timeServer + " =/\n" + exception);
				LastError = ErrorKind.CantResolveHost;
			}

			if (addresses == null || addresses.Length == 0)
			{
				Debug.LogWarning(LogPrefix + "Could not resolve IP from the host " + timeServer + " =/");
				LastError = ErrorKind.CantResolveHost;
			}

			if (LastError != ErrorKind.NoError)
			{
				yield break;
			}

		#region socket create and connect

			var timeBeforeAsyncCall = Time.unscaledTime;

			try
			{
				if (asyncSocket == null)
				{
					asyncSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					asyncSocket.SendTimeout = 3000;
					asyncSocket.ReceiveTimeout = 3000;
				}

				var ip = addresses[0];
				var ipBytes = ip.GetAddressBytes();

				if (ipBytes != targetIP)
				{
					targetEndpoint = new IPEndPoint(ip, 123);
					targetIP = ipBytes;
				}

				if (connectArgs == null)
				{
					connectArgs = new SocketAsyncEventArgs();
					connectArgs.Completed += OnSocketConnectedOrSent;
				}

				connectArgs.RemoteEndPoint = targetEndpoint;
				
				gettingOnlineTimeAsync = true;
				if (!asyncSocket.ConnectAsync(connectArgs))
				{
					OnSocketConnectedOrSent(asyncSocket, connectArgs);
				}
			}
			catch (Exception exception)
			{
				HandleSocketException(exception);
			}

			while (gettingOnlineTimeAsync)
			{
				if (Time.unscaledTime - timeBeforeAsyncCall > 4)
				{
					Debug.LogWarning(LogPrefix + "Socket didn't respond in time.");
					LastError = ErrorKind.Unknown;
					CloseSocket();
					yield break;
				}

				yield return cachedEndOfFrame;
			}

			if (asyncError != ErrorKind.NoError)
			{
				LastError = asyncError;
			}

			if (LastError != ErrorKind.NoError)
			{
				CloseSocket();
				yield break;
			}

		#endregion

		#region socket send

			ntpData[0] = 0x1B;

			if (sendArgs == null)
			{
				sendArgs = new SocketAsyncEventArgs();
				sendArgs.Completed += OnSocketConnectedOrSent;
				sendArgs.UserToken = asyncSocket;
				sendArgs.SetBuffer(ntpData, 0, NtpDataBufferLength);
			}

			sendArgs.RemoteEndPoint = targetEndpoint;

			timeBeforeAsyncCall = Time.unscaledTime;

			try
			{
				gettingOnlineTimeAsync = true;
				if (!asyncSocket.SendAsync(sendArgs))
				{
					OnSocketConnectedOrSent(asyncSocket, sendArgs);
				}
			}
			catch (Exception exception)
			{
				HandleSocketException(exception);
			}

			while (gettingOnlineTimeAsync)
			{
				if (Time.unscaledTime - timeBeforeAsyncCall > 4)
				{
					Debug.LogWarning(LogPrefix + "Socket didn't respond in time.");
					LastError = ErrorKind.Unknown;
					CloseSocket();
					yield break;
				}

				yield return cachedEndOfFrame;
			}

			if (asyncError != ErrorKind.NoError)
			{
				LastError = asyncError;
			}

			if (LastError != ErrorKind.NoError)
			{
				CloseSocket();
				yield break;
			}

		#endregion

		#region socket receive

			if (receiveArgs == null)
			{
				receiveArgs = new SocketAsyncEventArgs();
				receiveArgs.Completed += OnSocketReceive;
				receiveArgs.UserToken = asyncSocket;
				receiveArgs.SetBuffer(ntpData, 0, NtpDataBufferLength);
			}

			receiveArgs.RemoteEndPoint = targetEndpoint;

			timeBeforeAsyncCall = Time.unscaledTime;

			try
			{
				gettingOnlineTimeAsync = true;
				if (!asyncSocket.ReceiveAsync(receiveArgs))
				{
					OnSocketReceive(asyncSocket, receiveArgs);
				}
			}
			catch (Exception exception)
			{
				HandleSocketException(exception);
			}

			while (gettingOnlineTimeAsync)
			{
				if (Time.unscaledTime - timeBeforeAsyncCall > 4)
				{
					Debug.LogWarning(LogPrefix + "Socket didn't respond in time.");
					LastError = ErrorKind.Unknown;
					CloseSocket();
					yield break;
				}

				yield return cachedEndOfFrame;
			}

			if (asyncError != ErrorKind.NoError)
			{
				LastError = asyncError;
			}

			if (LastError != ErrorKind.NoError)
			{
				CloseSocket();
				yield break;
			}

		#endregion

			var intc = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
			var frac = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];

			lastOnlineTime = intc * 1000d + frac * 1000d / 0x100000000L;
		}

		private void OnSocketConnectedOrSent(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				if (e.SocketError != SocketError.Success)
				{
					Debug.LogWarning(LogPrefix + "Could not get NTP time from " + timeServer + " =/\n" + e.SocketError);
					asyncError = ErrorKind.Unknown;
				}
			}
			catch (Exception exception)
			{
				Debug.LogWarning(LogPrefix + "Could not get NTP time from " + timeServer + " =/\n" + exception);
				asyncError = ErrorKind.Unknown;
			}
			finally
			{
				gettingOnlineTimeAsync = false;
			}
		}

		/*private void OnSocketSend(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				if (e.SocketError != SocketError.Success)
				{
					Debug.LogWarning(LogPrefix + "Could not get NTP time from " + timeServer + " =/\n" + e.SocketError);
					asyncError = ErrorKind.Unknown;
				}
			}
			catch (Exception exception)
			{
				Debug.LogWarning(LogPrefix + "Could not get NTP time from " + timeServer + " =/\n" + exception);
				asyncError = ErrorKind.Unknown;
			}
			finally
			{
				gettingOnlineTimeAsync = false;
			}
		}*/

		private void OnSocketReceive(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				if (e.SocketError == SocketError.Success)
				{
					ntpData = e.Buffer;
				}
				else
				{
					Debug.LogWarning(LogPrefix + "Could not get NTP time from " + timeServer + " =/\n" + e.SocketError);
					asyncError = ErrorKind.Unknown;
				}
			}
			catch (Exception exception)
			{
				Debug.LogWarning(LogPrefix + "Could not get NTP time from " + timeServer + " =/\n" + exception);
				asyncError = ErrorKind.Unknown;
			}
			finally
			{
				gettingOnlineTimeAsync = false;
			}
		}

		private void CloseSocket()
		{
			if (asyncSocket != null)
			{
				asyncSocket.Shutdown(SocketShutdown.Both);
				asyncSocket.Close();
				asyncSocket = null;
			}

			gettingOnlineTimeAsync = false;
		}

		private void HandleSocketException(Exception exception)
		{
			Debug.LogWarning(LogPrefix + "Could not get NTP time from " + timeServer + " =/\n" + exception);
			LastError = ErrorKind.Unknown;

			var socketException = exception as SocketException;
			if (socketException != null)
			{
				if (socketException.SocketErrorCode == SocketError.HostNotFound)
				{
					LastError = ErrorKind.CantResolveHost;
				}
			}

			gettingOnlineTimeAsync = false;
		}

		private double GetLocalTime()
		{
			return DateTime.UtcNow.Subtract(date1900).TotalMilliseconds;
		}

		/* just an utility method for those who wish
		   to get online time for own further processing 
		   without starting detector 
		*/

		/// <summary>
		/// Retrieves NTP time from the specified time server, e.g. "pool.ntp.org".
		/// </summary>
		/// May block main thread on poor connection until gets data from time server, or until 3 sec timeout is met.
		/// <param name="server">NTP time server address, e.g. "pool.ntp.org"</param>
		/// <returns>NTP time in milliseconds or -1 if there was an error getting time.</returns>
		public static double GetOnlineTime(string server)
		{
			try
			{
				var ntpData = new byte[NtpDataBufferLength];

				ntpData[0] = 0x1B;

				var addresses = Dns.GetHostEntry(server).AddressList;
				var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				socket.Connect(new IPEndPoint(addresses[0], 123));
				socket.ReceiveTimeout = 3000;

				socket.Send(ntpData);
				socket.Receive(ntpData);
				socket.Close();

				var intc = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
				var frac = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];

				return intc * 1000d + frac * 1000d / 0x100000000L;
			}
			catch (Exception exception)
			{
				Debug.LogWarning(LogPrefix + "Could not get NTP time from " + server + " =/\n" + exception);
				return -1;
			}
		}

#else
		private const string ErrorMessage = LogPrefix + " is disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional or is not supported on current platform!";

		public static TimeCheatingDetector Instance
		{
			get
			{
				Debug.LogError(ErrorMessage);
				return null;
			}
		}

		public ErrorKind LastError
		{
			get
			{
				Debug.LogError(ErrorMessage);
				return ErrorKind.Unknown;
			}
		}

		public TimeCheatingDetectorResult LastResult
		{
			get
			{
				Debug.LogError(ErrorMessage);
				return TimeCheatingDetectorResult.Error;
			}
		}

		public static void StartDetection(System.Action detectionCallback = null, System.Action<ErrorKind> errorCallback = null, System.Action checkPassedCallback = null)
		{
			Debug.LogError(ErrorMessage);
		}

		public static void StartDetection(float interval, System.Action detectionCallback = null, System.Action<ErrorKind> errorCallback = null, System.Action checkPassedCallback = null)
		{
			Debug.LogError(ErrorMessage);
		}

		public static void StopDetection()
		{
			Debug.LogError(ErrorMessage);
		}
		
		public static void Dispose()
		{
			Debug.LogError(ErrorMessage);
		}

		public static double GetOnlineTime(string server)
		{
			Debug.LogError(ErrorMessage);
			return -1;
		}

		public IEnumerator ForceCheckEnumerator()
		{
			Debug.LogError(ErrorMessage);
			yield break;
		}

#if ASYNC_AWAIT_ENABLED
		public async System.Threading.Tasks.Task<TimeCheatingDetectorResult> ForceCheckTask()
		{
			Debug.LogError(errorMessage);
			return TimeCheatingDetectorResult.Error;
		}
#endif

		protected override void StartDetectionAutomatically()
		{
			Debug.LogError(ErrorMessage);
		}
#endif
	}
}