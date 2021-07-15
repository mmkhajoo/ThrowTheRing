#if UNITY_EDITOR

using System;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode
{
	/// <summary>
	/// Class with utility functions to help with ACTk migrations after updates.
	/// </summary>
	public class MigrateUtils
	{
		/// <summary>
		/// Checks all prefabs in project for old version of obscured types and tries to migrate values to the new version.
		/// </summary>
		[UnityEditor.MenuItem(ActEditorGlobalStuff.WindowsMenuPath + "Migrate obscured types on prefabs...", false, 1100)]
		public static void MigrateObscuredTypesOnPrefabs()
		{
			if (!EditorUtility.DisplayDialog("ACTk Obscured types migration",
				"Are you sure you wish to scan all prefabs in your project and automatically migrate values to the new format?",
				"Yes", "No"))
			{
				Debug.Log(ActEditorGlobalStuff.LogPrefix + "Obscured types migration was canceled by user.");
				return;
			}

			AssetDatabase.SaveAssets();

			var touchedCount = 0;
			try
			{
				var assets = AssetDatabase.FindAssets("t:ScriptableObject t:Prefab");
				var count = assets.Length;
				for (var i = 0; i < count; i++)
				{
					if (EditorUtility.DisplayCancelableProgressBar("Looking through objects", "Object " + (i + 1) + " from " + count,
						i / (float)count))
					{
						Debug.Log(ActEditorGlobalStuff.LogPrefix + "Obscured types migration was canceled by user.");
						break;
					}

					var guid = assets[i];
					var path = AssetDatabase.GUIDToAssetPath(guid);

					var objects = AssetDatabase.LoadAllAssetsAtPath(path);
					foreach (var unityObject in objects)
					{
						if (unityObject == null) continue;
						if (unityObject.name == "Deprecated EditorExtensionImpl") continue;

						var so = new SerializedObject(unityObject);
						var modified = MigrateObject(so, unityObject.name + "\n" + path);

						if (modified)
						{
							touchedCount++;
							so.ApplyModifiedProperties();
							EditorUtility.SetDirty(unityObject);
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
			finally
			{
				AssetDatabase.SaveAssets();
				EditorUtility.ClearProgressBar();
			}

			if (touchedCount > 0)
				Debug.Log(ActEditorGlobalStuff.LogPrefix + "Migrated obscured types on " + touchedCount + " objects.");
			else
				Debug.Log(ActEditorGlobalStuff.LogPrefix + "No objects were found for obscured types migration.");
		}

		/// <summary>
		/// Checks all scenes in project for old version of obscured types and tries to migrate values to the new version.
		/// </summary>
		[UnityEditor.MenuItem(ActEditorGlobalStuff.WindowsMenuPath + "Migrate obscured types in opened scene(s)...", false, 1101)]
		public static void MigrateObscuredTypesInScene()
		{
			var touchedCount = 0;
			try
			{
				var allTransformsInOpenedScenes = Resources.FindObjectsOfTypeAll<Transform>();
				var count = allTransformsInOpenedScenes.Length;
				var updateStep = Math.Max(count / 10, 1);

				for (var i = 0; i < count; i++)
				{
					var transform = allTransformsInOpenedScenes[i];
					if (i % updateStep == 0 && EditorUtility.DisplayCancelableProgressBar("Looking through objects", "Object " + (i + 1) + " from " + count,
						    i / (float)count))
					{
						Debug.Log(ActEditorGlobalStuff.LogPrefix + "Obscured types migration was canceled by user.");
						break;
					}

					if (transform == null) continue;

					var components = transform.GetComponents<Component>();
					foreach (var component in components)
					{
						if (component == null) continue;

						var so = new SerializedObject(component);
						var modified = MigrateObject(so, transform.name);

						if (modified)
						{
#if UNITY_5_3_OR_NEWER
							UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
#else
							EditorApplication.MarkSceneDirty();
#endif
							touchedCount++;
							so.ApplyModifiedProperties();
							EditorUtility.SetDirty(component);
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
			finally
			{
				if (touchedCount > 0)
				{
					EditorUtility.DisplayDialog(touchedCount + " objects migrated", "Objects with old obscured types migrated: " + touchedCount + ".\nPlease save your scenes to keep the changes.", "Fine");

#if UNITY_5_3_OR_NEWER
					UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
#else
					EditorApplication.SaveCurrentSceneIfUserWantsTo();
#endif
				}

				AssetDatabase.SaveAssets();
				EditorUtility.ClearProgressBar();
			}

			if (touchedCount > 0)
				Debug.Log(ActEditorGlobalStuff.LogPrefix + "Migrated obscured types on " + touchedCount + " objects in opened scene(s).");
			else
				Debug.Log(ActEditorGlobalStuff.LogPrefix + "No objects were found in opened scene(s) for obscured types migration.");
		}

		private static bool MigrateObject(SerializedObject so, string label)
		{
			var modified = false;

			var sp = so.GetIterator();
			if (sp == null) return false;

			while (sp.NextVisible(true))
			{
				if (sp.propertyType != SerializedPropertyType.Generic) continue;

				var type = sp.type;

				switch (type)
				{
					case "ObscuredDouble":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						if (hiddenValue == null) continue;

						var hiddenValueOldProperty = sp.FindPropertyRelative("hiddenValueOldByte8");
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

						if (oldValueExists)
						{
							var union = new LongBytesUnion {b8 = hiddenValueOld};
							union.b8.Shuffle();
							hiddenValue.longValue = union.l;

							hiddenValueOldProperty.FindPropertyRelative("b1").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b2").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b3").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b4").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b5").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b6").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b7").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b8").intValue = 0;

							Debug.Log(ActEditorGlobalStuff.LogPrefix + "Migrated property " + sp.displayName + ":" + type +
							          " at the object " + label);
							modified = true;
						}

						break;
					}
					case "ObscuredFloat":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						if (hiddenValue == null) continue;

						var hiddenValueOldProperty = sp.FindPropertyRelative("hiddenValueOldByte4");
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

						if (oldValueExists)
						{
							var union = new IntBytesUnion {b4 = hiddenValueOld};
							union.b4.Shuffle();
							hiddenValue.longValue = union.i;

							hiddenValueOldProperty.FindPropertyRelative("b1").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b2").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b3").intValue = 0;
							hiddenValueOldProperty.FindPropertyRelative("b4").intValue = 0;

							Debug.Log(ActEditorGlobalStuff.LogPrefix + "Migrated property " + sp.displayName + ":" + type +
							          " at the object " + label);
							modified = true;
						}

						/*var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						if (hiddenValue == null) continue;

						var hiddenValue1 = hiddenValue.FindPropertyRelative("b1");
						var hiddenValue2 = hiddenValue.FindPropertyRelative("b2");
						var hiddenValue3 = hiddenValue.FindPropertyRelative("b3");
						var hiddenValue4 = hiddenValue.FindPropertyRelative("b4");

						var hiddenValueOld = sp.FindPropertyRelative("hiddenValueOld");

						if (hiddenValueOld != null && hiddenValueOld.isArray && hiddenValueOld.arraySize == 4)
						{
							hiddenValue1.intValue = hiddenValueOld.GetArrayElementAtIndex(0).intValue;
							hiddenValue2.intValue = hiddenValueOld.GetArrayElementAtIndex(1).intValue;
							hiddenValue3.intValue = hiddenValueOld.GetArrayElementAtIndex(2).intValue;
							hiddenValue4.intValue = hiddenValueOld.GetArrayElementAtIndex(3).intValue;

							hiddenValueOld.arraySize = 0;

							Debug.Log(ActEditorGlobalStuff.LogPrefix + "Migrated property " + sp.displayName + ":" + type +
							          " at the object " + label);
							modified = true;
						}*/

						break;
					}
					/*
					case "ObscuredBool":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
						var fakeValue = sp.FindPropertyRelative("fakeValue");
						var fakeValueChanged = sp.FindPropertyRelative("fakeValueChanged");
						var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
						var inited = sp.FindPropertyRelative("inited");

						if (inited != null && inited.boolValue)
						{
							var currentCryptoKey = cryptoKey.intValue;
							var real = ObscuredBool.Decrypt(hiddenValue.intValue, (byte)currentCryptoKey);
							var fake = fakeValue.boolValue;

							if (real != fake)
							{
								Debug.Log(ActEditorGlobalStuff.LogPrefix + "Fixed property " + sp.displayName + ":" + type +
								          " at the object " + label);
								fakeValue.boolValue = real;
								if (fakeValueChanged != null) fakeValueChanged.boolValue = true;
								if (fakeValueActive != null) fakeValueActive.boolValue = true;
								modified = true;
							}
						}

						break;
					} 
					case "ObscuredInt":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
						var fakeValue = sp.FindPropertyRelative("fakeValue");
						var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
						var inited = sp.FindPropertyRelative("inited");

						if (inited != null && inited.boolValue)
						{
							var currentCryptoKey = cryptoKey.intValue;
							var real = ObscuredInt.Decrypt(hiddenValue.intValue, currentCryptoKey);
							var fake = fakeValue.intValue;

							if (real != fake)
							{
								Debug.Log(ActEditorGlobalStuff.LogPrefix + "Fixed property " + sp.displayName + ":" + type +
								          " at the object " + label);
								fakeValue.intValue = real;
								fakeValueActive.boolValue = true;
								modified = true;
							}
						}

						break;
					}
					case "ObscuredLong":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
						var fakeValue = sp.FindPropertyRelative("fakeValue");
						var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
						var inited = sp.FindPropertyRelative("inited");

						if (inited != null && inited.boolValue)
						{
							var currentCryptoKey = cryptoKey.longValue;
							var real = ObscuredLong.Decrypt(hiddenValue.longValue, currentCryptoKey);
							var fake = fakeValue.longValue;

							if (real != fake)
							{
								Debug.Log(ActEditorGlobalStuff.LogPrefix + "Fixed property " + sp.displayName + ":" + type +
								          " at the object " + label);
								fakeValue.longValue = real;
								fakeValueActive.boolValue = true;
								modified = true;
							}
						}

						break;
					}
					case "ObscuredShort":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
						var fakeValue = sp.FindPropertyRelative("fakeValue");
						var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
						var inited = sp.FindPropertyRelative("inited");

						if (inited != null && inited.boolValue)
						{
							var currentCryptoKey = (short)cryptoKey.intValue;
							var real = ObscuredShort.EncryptDecrypt((short)hiddenValue.intValue, currentCryptoKey);
							var fake = (short)fakeValue.intValue;

							if (real != fake)
							{
								Debug.Log(ActEditorGlobalStuff.LogPrefix + "Fixed property " + sp.displayName + ":" + type +
								          " at the object " + label);
								fakeValue.intValue = real;
								fakeValueActive.boolValue = true;
								modified = true;
							}
						}

						break;
					}
					case "ObscuredString":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
						var fakeValue = sp.FindPropertyRelative("fakeValue");
						var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
						var inited = sp.FindPropertyRelative("inited");

						if (inited != null && inited.boolValue)
						{
							var currentCryptoKey = cryptoKey.stringValue;
							var bytes = new byte[hiddenValue.arraySize];
							for (var j = 0; j < hiddenValue.arraySize; j++)
							{
								bytes[j] = (byte)hiddenValue.GetArrayElementAtIndex(j).intValue;
							}

							var real = ObscuredString.EncryptDecrypt(GetString(bytes), currentCryptoKey);
							var fake = fakeValue.stringValue;

							if (real != fake)
							{
								Debug.Log(ActEditorGlobalStuff.LogPrefix + "Fixed property " + sp.displayName + ":" + type +
								          " at the object " + label);
								fakeValue.stringValue = real;
								fakeValueActive.boolValue = true;
								modified = true;
							}
						}

						break;
					}
					case "ObscuredUInt":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
						var fakeValue = sp.FindPropertyRelative("fakeValue");
						var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
						var inited = sp.FindPropertyRelative("inited");

						if (inited != null && inited.boolValue)
						{
							var currentCryptoKey = (uint)cryptoKey.intValue;
							var real = ObscuredUInt.Decrypt((uint)hiddenValue.intValue, currentCryptoKey);
							var fake = (uint)fakeValue.intValue;

							if (real != fake)
							{
								Debug.Log(ActEditorGlobalStuff.LogPrefix + "Fixed property " + sp.displayName + ":" + type +
								          " at the object " + label);
								fakeValue.intValue = (int)real;
								fakeValueActive.boolValue = true;
								modified = true;
							}
						}

						break;
					}
					case "ObscuredULong":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
						var fakeValue = sp.FindPropertyRelative("fakeValue");
						var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
						var inited = sp.FindPropertyRelative("inited");

						if (inited != null && inited.boolValue)
						{
							var currentCryptoKey = (ulong)cryptoKey.longValue;
							var real = ObscuredULong.Decrypt((ulong)hiddenValue.longValue, currentCryptoKey);
							var fake = (ulong)fakeValue.longValue;

							if (real != fake)
							{
								Debug.Log(ActEditorGlobalStuff.LogPrefix + "Fixed property " + sp.displayName + ":" + type +
								          " at the object " + label);
								fakeValue.longValue = (long)real;
								fakeValueActive.boolValue = true;
								modified = true;
							}
						}

						break;
					}
					case "ObscuredVector2":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						if (hiddenValue == null) continue;

						var hiddenValueX = hiddenValue.FindPropertyRelative("x");
						var hiddenValueY = hiddenValue.FindPropertyRelative("y");

						var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
						var fakeValue = sp.FindPropertyRelative("fakeValue");
						var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
						var inited = sp.FindPropertyRelative("inited");

						if (inited != null && inited.boolValue)
						{
							var ev = new ObscuredVector2.RawEncryptedVector2();
							ev.x = hiddenValueX.intValue;
							ev.y = hiddenValueY.intValue;

							var currentCryptoKey = cryptoKey.intValue;
							var real = ObscuredVector2.Decrypt(ev, currentCryptoKey);
							var fake = fakeValue.vector2Value;

							if (real != fake)
							{
								Debug.Log(ActEditorGlobalStuff.LogPrefix + "Fixed property " + sp.displayName + ":" + type +
								          " at the object " + label);
								fakeValue.vector2Value = real;
								fakeValueActive.boolValue = true;
								modified = true;
							}
						}

						break;
					}

					case "ObscuredVector3":
					{
						var hiddenValue = sp.FindPropertyRelative("hiddenValue");
						if (hiddenValue == null) continue;

						var hiddenValueX = hiddenValue.FindPropertyRelative("x");
						var hiddenValueY = hiddenValue.FindPropertyRelative("y");
						var hiddenValueZ = hiddenValue.FindPropertyRelative("z");

						var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
						var fakeValue = sp.FindPropertyRelative("fakeValue");
						var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
						var inited = sp.FindPropertyRelative("inited");

						if (inited != null && inited.boolValue)
						{
							var ev = new ObscuredVector3.RawEncryptedVector3();
							ev.x = hiddenValueX.intValue;
							ev.y = hiddenValueY.intValue;
							ev.z = hiddenValueZ.intValue;

							var currentCryptoKey = cryptoKey.intValue;
							var real = ObscuredVector3.Decrypt(ev, currentCryptoKey);
							var fake = fakeValue.vector3Value;

							if (real != fake)
							{
								Debug.Log(ActEditorGlobalStuff.LogPrefix + "Fixed property " + sp.displayName + ":" + type +
								          " at the object " + label);
								fakeValue.vector3Value = real;
								fakeValueActive.boolValue = true;
								modified = true;
							}
						}

						break;
					}*/
				}
			}

			return modified;
		}

		private static void EncryptAndSetBytes(string val, SerializedProperty prop, string key)
		{
			var encrypted = ObscuredString.EncryptDecrypt(val, key);
			var encryptedBytes = GetBytes(encrypted);

			prop.ClearArray();
			prop.arraySize = encryptedBytes.Length;

			for (var i = 0; i < encryptedBytes.Length; i++)
			{
				prop.GetArrayElementAtIndex(i).intValue = encryptedBytes[i];
			}
		}

		private static byte[] GetBytes(string str)
		{
			var bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		private static string GetString(byte[] bytes)
		{
			var chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct LongBytesUnion
		{
			[FieldOffset(0)]
			public long l;

			[FieldOffset(0)]
			public ACTkByte8 b8;
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