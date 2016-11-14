/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Sound object
 */

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziSound : IDisposable
	{
		public  bool 		  isFoldedInUI;
		public  bool          includeInData = true;
		public  string		  soundData;
		private AudioClip     clip;
		private GameObject    obj;
		private string		  pathToExport;
		private string		  directory;

		public LoziSound (GameObject target)
		{
			obj  = target;
			clip = obj.GetComponent<AudioSource>().clip;
		}

		public void generate(string path)
		{
			pathToExport = path+"/";
			string[] arr = path.Split('/');
			directory    = arr[arr.Length-1];

			if(directory.Length>=2)
			{
				directory    = arr[arr.Length-2]+"/"+arr[arr.Length-1];
			}
			soundData = "";
			setSoundData();
		}

		private string getSoundData(AudioClip clip,string clipName)
		{
			string path = Application.dataPath+AssetDatabase.GetAssetPath(clip).Remove(0,6);

			if(includeInData)
			{
				if (File.Exists(path))
				{
					return "data:audio/wav;base64,"+Convert.ToBase64String(File.ReadAllBytes(path));
				}
				return "";

			}
			else
			{
				if (File.Exists(path))
				{
					string   ext  = "";
					string[] arr  = path.Split('.');
					
					if(arr.Length>0)
					{
						ext = arr[arr.Length-1];
					}

					SaveSoundToFile(File.ReadAllBytes(path),clipName+"."+ext);

					return directory+"/"+clipName+"."+ext;
				}
				return "";
			}
		}

		private void SaveSoundToFile(byte[] bytes,string fileName)
		{
			BinaryWriter binary = new BinaryWriter(File.Open(pathToExport+fileName,FileMode.Create));
			binary.Write(bytes);
		}

		private void setSoundData()
		{
			if(clip!=null)
			{
				soundData = getSoundData(clip,clip.name);
			}
		}

		public Dictionary<string,object> soundProperties
		{
			get
			{
				if(clip!=null)
				{
					Dictionary<string,object> dict = new Dictionary<string, object>();
					dict["id"  ] = clip.GetInstanceID();
					dict["name"] = clip.name;
					dict["data"] = soundData;
					
					return dict;
				}
				return null;
			}
		}
		
		public AudioClip soundClip
		{
			get
			{
				return clip;
			}
		}

		public string soundClipName
		{
			get
			{
				if(clip!=null)
				{
					return clip.name;
				}
				return "";
			}
		}

		public void Dispose()
		{
		}

		public static bool hasClip(GameObject obj)
		{
			if(obj.GetComponent<AudioSource>()!=null)
			{
				if(obj.GetComponent<AudioSource>().clip!=null)
				{
					return true;
				}
			}
			return false;
		}

		public static AudioClip getClip(GameObject obj)
		{
			if(obj.GetComponent<AudioSource>()!=null)
			{
				return obj.GetComponent<AudioSource>().clip;
			}
			return null;
		}
	}
}

