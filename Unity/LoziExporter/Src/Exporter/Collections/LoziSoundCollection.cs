/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Textures collection, stores all textures of object and its children
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziSoundCollection : IDisposable
	{
		private bool includeSounds = true;
		private List<LoziSound> soundCollection;

		public LoziSoundCollection(GameObject obj)
		{
			soundCollection  = new List<LoziSound>();

			foreach(Transform transform in obj.GetComponentsInChildren<Transform>())
			{
				if(LoziSound.hasClip(transform.gameObject))
				{
					if(canAddInSoundsArray(LoziSound.getClip(transform.gameObject)))
					{
						soundCollection.Add(new LoziSound(transform.gameObject));
					}
				}
			}
		}

		public void generate(string path)
		{
			foreach(LoziSound snd in soundCollection) 
			{
				snd.generate(path);
			}
		}
		
		public List<LoziSound> sounds
		{
			get
			{
				return soundCollection;
			}
		}

		public List<Dictionary<string,object>> soundProperties
		{
			get
			{
				List<Dictionary<string,object>> arr = new List<Dictionary<string, object>>();

				for(int num = 0; num < soundCollection.Count; num++)
				{
					if(soundCollection[num].soundClip!=null)
					{
						arr.Add(soundCollection[num].soundProperties);
					}
				}
				return arr;
			}
		}

		public bool includeSoundsInFile
		{
			get
			{
				return includeSounds;
			}
			set
			{
				includeSounds = value;
				foreach(LoziSound snd in soundCollection)
				{
					snd.includeInData = includeSounds;
				}
			}
		}

		public bool canAddInSoundsArray(AudioClip clip)
		{
			if(clip!=null)
			{
				for(int num = 0; num < soundCollection.Count; num++)
				{
					if(clip.GetInstanceID() == soundCollection[num].soundClip.GetInstanceID())
					{
						return false;
					}
				}
			}
			return true;
		}

		public void Dispose()
		{ 
			for(int num = 0; num < soundCollection.Count; num++)
			{
				soundCollection[num].Dispose();
			}
			soundCollection.Clear();
			soundCollection = null;
		}
	}
}
