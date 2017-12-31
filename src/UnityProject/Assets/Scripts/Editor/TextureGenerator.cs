using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hover
{
	public class TextureGenerator : EditorWindow
	{
		[MenuItem("Tools/Texture Generator")]
		private static void Open()
		{
			GetWindow<TextureGenerator>();
		}

		private Color m_color = Color.white;

		private void OnGUI()
		{
			m_color = EditorGUILayout.ColorField(m_color);
			if (GUILayout.Button("Generate", EditorStyles.miniButton))
			{
				string path = EditorUtility.SaveFilePanel("Save Texture", "", "NewTexture", "png");
				//path = string.Format("Assets{0}", path.Replace(Application.dataPath, ""));

				if (!string.IsNullOrEmpty(path))
				{
					Texture2D texture = new Texture2D(1, 1);
					texture.SetPixel(0, 0, m_color);
					texture.Apply();

					byte[] data = texture.EncodeToPNG();
					File.WriteAllBytes(path, data);

					AssetDatabase.Refresh();
				}
			}
		}
	}
}