using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class FullExport 
{
	[MenuItem("Export/MyExport")]
	public static void ExportPackage() {
		//AssetDatabase.ExportPackage(AssetDatabase.GetAllAssetPaths(), "MagicGameKit2020 V4.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);

		string[] projectContent = new string[] { "Assets", "ProjectSettings/TagManager.asset", "ProjectSettings/InputManager.asset", "ProjectSettings/ProjectSettings.asset", "Packages/com.unity.postprocessing", "Packages/com.unity.probuilder" };
		AssetDatabase.ExportPackage(projectContent, "MagicGameKit2020 V5.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

	}
}
