using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildCustomCastMOD : MonoBehaviour {

	[MenuItem("BuildCustomCastMOD/BuildMOD")]
	static void BuildMOD() {
		var projectDirectory = System.IO.Directory.GetCurrentDirectory();
		var outDirectory = projectDirectory + "/CustomCastMOD";
		var dataDirectory = projectDirectory + "/Assets/gamedata/_append";
		var bundleName = "";
		if(!System.IO.Directory.Exists(outDirectory)) {
			Directory.CreateDirectory(outDirectory);
		}
		BuildPipeline.BuildAssetBundles ("CustomCastMOD", BuildAssetBundleOptions.None,BuildTarget.Android);
		Debug.Log (BuildAlist(dataDirectory));
		String[] outFiles = Directory.GetFiles(outDirectory);
		foreach (String item in outFiles) {
			Debug.Log (item);
			if(item.IndexOf(".manifest") == -1 || item.IndexOf("CustomCastMOD") == -1) {
				if(item.IndexOf(".alist") == -1) {
					bundleName = Path.GetFileName(item);
					Debug.Log (bundleName);
				}
			}
		}
		StreamWriter sw = File.CreateText(outDirectory + "/" + bundleName + ".alist");
		sw.Write(BuildAlist(dataDirectory));
		sw.Close();
	}

	static String BuildAlist(String path) {
		String outAlist = "";
		if(System.IO.Directory.Exists(path)) {
			String[] directories = Directory.GetDirectories(path);
			String[] files = Directory.GetFiles(path);
			foreach (String item in files) {
				if(item.IndexOf("meta") == -1) {
					outAlist += BuildAlist(item);
				}
			}
			foreach (String item in directories) {
				outAlist += BuildAlist(item);
			}
		} else if(System.IO.File.Exists(path)) {
			String replaceString = path.Replace("\\", "/");
			String subString = replaceString.Substring(replaceString.IndexOf("Assets"), replaceString.Length - replaceString.IndexOf("Assets"));
			String assetsPath = subString.Replace(".bytes", "");
			String fileName = System.IO.Path.GetFileName(path).Replace(".bytes", "");
			System.IO.FileInfo fi = new System.IO.FileInfo(path);
			outAlist += "0," + fileName + "," + assetsPath + "," + fi.Length + "\n";
		}
		return outAlist;
	}
}
