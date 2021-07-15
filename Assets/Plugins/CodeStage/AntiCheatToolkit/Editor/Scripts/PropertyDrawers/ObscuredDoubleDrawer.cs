#if UNITY_EDITOR
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredDouble))]
	internal class ObscuredDoubleDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative("hiddenValue");

			var hiddenValueOldProperty = prop.FindPropertyRelative("hiddenValueOldByte8");
			var hiddenValueOld = default(ACTkByte8);
			var oldValueExists = false;

			if (hiddenValueOldProperty != null)
			{
				if (hiddenValueOldProperty.FindPropertyRelative("b1") != null)
				{
					hiddenValueOld.b1 = (byte)hiddenValueOldProperty.FindPropertyRelative("b1").intValue;
					hiddenValueOld.b2 = (byte)hiddenValueOldProperty.FindPropertyRelative("b2").intValue;
					hiddenValueOld.b3 = (byte)hiddenValueOldProperty.FindPropertyRelative("b3").intValue;
					hiddenValueOld.b4 = (byte)hiddenValueOldProperty.FindPropertyRelative("b4").intValue;
					hiddenValueOld.b5 = (byte)hiddenValueOldProperty.FindPropertyRelative("b5").intValue;
					hiddenValueOld.b6 = (byte)hiddenValueOldProperty.FindPropertyRelative("b6").intValue;
					hiddenValueOld.b7 = (byte)hiddenValueOldProperty.FindPropertyRelative("b7").intValue;
					hiddenValueOld.b8 = (byte)hiddenValueOldProperty.FindPropertyRelative("b8").intValue;

					if (hiddenValueOld.b1 != 0 ||
					    hiddenValueOld.b2 != 0 ||
					    hiddenValueOld.b3 != 0 ||
					    hiddenValueOld.b4 != 0 ||
					    hiddenValueOld.b5 != 0 ||
					    hiddenValueOld.b6 != 0 ||
					    hiddenValueOld.b7 != 0 ||
					    hiddenValueOld.b8 != 0)
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

			var currentCryptoKey = cryptoKey.longValue;

			var union = new LongBytesUnion();
			double val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.longValue = ObscuredDouble.cryptoKeyEditor;
				}

				inited.boolValue = true;

				union.l = ObscuredDouble.Encrypt(0, currentCryptoKey);
				hiddenValue.longValue = union.l;
			}
			else
			{
				if (oldValueExists)
				{
					union.b8 = hiddenValueOld;
					union.b8.Shuffle();
				}
				else
				{
					union.l = hiddenValue.longValue;
				}

				val = ObscuredDouble.Decrypt(union.l, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.DoubleField(position, label, val);
			if (EditorGUI.EndChangeCheck())
			{
				union.l = ObscuredDouble.Encrypt(val, currentCryptoKey);
				hiddenValue.longValue = union.l;

				if (oldValueExists)
				{
					hiddenValueOldProperty.FindPropertyRelative("b1").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b2").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b3").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b4").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b5").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b6").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b7").intValue = 0;
					hiddenValueOldProperty.FindPropertyRelative("b8").intValue = 0;
				}

				fakeValue.doubleValue = val;
				fakeValueActive.boolValue = true;
			}
			
			ResetBoldFont();
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct LongBytesUnion
		{
			[FieldOffset(0)]
			public long l;

			[FieldOffset(0)]
			public ACTkByte8 b8;
		}
	}
}
#endif