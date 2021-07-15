#if UNITY_EDITOR
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredFloat))]
	internal class ObscuredFloatDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative("hiddenValue");

			var hiddenValueOldProperty = prop.FindPropertyRelative("hiddenValueOldByte4");
			var hiddenValueOld = default(ACTkByte4);
			var oldValueExists = false;

			if (hiddenValueOldProperty != null)
			{
				if (hiddenValueOldProperty.FindPropertyRelative("b1") != null)
				{
					hiddenValueOld.b1 = (byte)hiddenValueOldProperty.FindPropertyRelative("b1").intValue;
					hiddenValueOld.b2 = (byte)hiddenValueOldProperty.FindPropertyRelative("b2").intValue;
					hiddenValueOld.b3 = (byte)hiddenValueOldProperty.FindPropertyRelative("b3").intValue;
					hiddenValueOld.b4 = (byte)hiddenValueOldProperty.FindPropertyRelative("b4").intValue;

					if (hiddenValueOld.b1 != 0 ||
					    hiddenValueOld.b2 != 0 ||
					    hiddenValueOld.b3 != 0 ||
					    hiddenValueOld.b4 != 0)
					{
						oldValueExists = true;
					}
				}
			}

			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			var inited = prop.FindPropertyRelative("inited");
			var fakeValue = prop.FindPropertyRelative("fakeValue");
			var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

			var currentCryptoKey = cryptoKey.intValue;

			var union = new IntBytesUnion();
			float val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.intValue = ObscuredFloat.cryptoKeyEditor;
				}

				inited.boolValue = true;

				union.i = ObscuredFloat.Encrypt(0, currentCryptoKey);
				hiddenValue.intValue = union.i;
			}
			else
			{
				if (oldValueExists)
				{
					union.b4 = hiddenValueOld;
					union.b4.Shuffle();
				}
				else
				{
					union.i = hiddenValue.intValue;
				}

				val = ObscuredFloat.Decrypt(union.i, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.FloatField(position, label, val);
			if (EditorGUI.EndChangeCheck())
			{
				union.i = ObscuredFloat.Encrypt(val, currentCryptoKey);
				hiddenValue.intValue = union.i;

				if (oldValueExists)
				{
					hiddenValueOldProperty.FindPropertyRelative("b1").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b2").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b3").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b4").intValue = 0;
				}

				fakeValue.floatValue = val;
				fakeValueActive.boolValue = true;
			}
			
			ResetBoldFont();
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct IntBytesUnion
		{
			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public ACTkByte4 b4;
		}
	}
}
#endif