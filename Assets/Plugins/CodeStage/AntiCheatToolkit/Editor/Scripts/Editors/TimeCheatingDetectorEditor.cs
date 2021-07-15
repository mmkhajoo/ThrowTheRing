#if UNITY_EDITOR
using CodeStage.AntiCheat.Detectors;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.Editors
{
	[CustomEditor(typeof(TimeCheatingDetector))]
	internal class TimeCheatingDetectorEditor : ActDetectorEditor
	{
#if !UNITY_WINRT && !ACTK_PREVENT_INTERNET_PERMISSION
		private SerializedProperty interval;
		private SerializedProperty threshold;

		protected override void FindUniqueDetectorProperties()
		{
			interval = serializedObject.FindProperty("interval");
			threshold = serializedObject.FindProperty("threshold");
		}

		protected override void DrawUniqueDetectorProperties()
		{
			EditorGUILayout.PropertyField(interval);
			EditorGUILayout.PropertyField(threshold);

			GUILayout.Label("<b>Needs Internet connection!</b>", ActEditorGUI.RichMiniLabel);

		}
#else
#if ACTK_PREVENT_INTERNET_PERMISSION
		protected override void DrawUniqueDetectorProperties()
		{
			GUILayout.Label("<b>ACTK_PREVENT_INTERNET_PERMISSION flag disables this detector!</b>", ActEditorGUI.RichLabel);
		}
#else
		protected override void DrawUniqueDetectorProperties()
		{
			GUILayout.Label("<b>Not supported on Universal Windows Platform!</b>", ActEditorGUI.RichLabel);
		}
#endif

#endif

	}
}
#endif