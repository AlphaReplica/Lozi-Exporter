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
	public class LoziSoundSource : IDisposable
	{
		private AudioClip     clip;
		private AudioSource   source;
		private GameObject    obj;

		public LoziSoundSource (GameObject target)
		{
			obj    = target;
			source = obj.GetComponent<AudioSource>();
			clip   = source.clip;
		}
		
		public AudioClip soundClip
		{
			get
			{
				return clip;
			}
		}
		
		public AudioSource audioSource
		{
			get
			{
				return source;
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

		
		public Dictionary<string,object> sourceProperties
		{
			get
			{
				if(clip!=null)
				{
					Dictionary<string,object> dict = new Dictionary<string, object>();
					dict["id" 		  ] = source.GetInstanceID();
					dict["loop"		  ] = source.loop;
					dict["mute"		  ] = source.mute;
					dict["spread"     ] = source.spread;
					dict["clip"       ] = clip.GetInstanceID();
					dict["autoPlay"   ] = source.playOnAwake;
					
					return dict;
				}
				return null;
			}
		}

		public string name
		{
			get
			{
				return source.name;
			}
		}

		public void Dispose()
		{
		}

		public static bool hasClip(GameObject obj)
		{
			if(hasSound(obj))
			{
				if(obj.GetComponent<AudioSource>().clip!=null)
				{
					return true;
				}
			}
			return false;
		}

		public static bool hasSound(GameObject obj)
		{
			if(obj.GetComponent<AudioSource>())
			{
				return true;
			}
			return false;
		}
	}
}

