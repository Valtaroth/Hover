using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Valtaroth.Hover
{
	public class ScriptCreationProcessor : UnityEditor.AssetModificationProcessor
	{
		public static void OnWillCreateAsset(string path)
		{
			string scriptPath = path.Replace(".meta", "");
			if (!scriptPath.EndsWith(".cs"))
			{
				return;
			}

			Debug.LogFormat("Processing {0}", scriptPath);

			int index = Application.dataPath.LastIndexOf("Assets");
			string absoluteScriptPath = Application.dataPath.Substring(0, index) + scriptPath;
			if (!File.Exists(absoluteScriptPath))
			{
				return;
			}

			string file = ReplaceScriptTemplateVariables(scriptPath, File.ReadAllText(absoluteScriptPath));

			File.WriteAllText(absoluteScriptPath, file);
			AssetDatabase.Refresh();
		}

		private static string ReplaceScriptTemplateVariables(string scriptPath, string content)
		{
			content = ReplaceNamespaceVariable(scriptPath, content);
			return content;
		}

		private static string ReplaceNamespaceVariable(string scriptPath, string content)
		{
			string nameSpace = typeof(ScriptCreationProcessor).Namespace;
			string[] spaces = scriptPath.Split('/');
			if (spaces.Length < 4)
			{
				return content;
			}

			for (int i = 2; i < spaces.Length - 1; i++)
			{
				nameSpace += string.Format(".{0}", spaces[i]);
			}

			return content.Replace("#NAMESPACE#", nameSpace);
		}
	}
}