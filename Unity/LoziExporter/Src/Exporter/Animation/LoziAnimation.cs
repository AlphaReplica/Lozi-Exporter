/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Animation Object, stores animation clips array
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Lozi.baseClasses;

namespace Lozi
{
	public class LoziAnimation : IDisposable
	{
		public  bool 	   				isFoldedInUI;
		public  bool      				isSkinAnimation;
		public  string     				objName;
		private GameObject 				obj;
		private int		   				objectId;
		private List<LoziAnimationClip> clips;

		public LoziAnimation (GameObject target)
		{
			isSkinAnimation = false;
			obj      		= target;
			objName  		= obj.name;
			clips    		= new List<LoziAnimationClip>();
			objectId 		= target.GetComponent<Animation>().GetInstanceID();

			checkAndAddClips(target.GetComponent<Animation>());
		}

		// Generates object from animation clips
		public void generate()
		{
			foreach(LoziAnimationClip clip in clips) 
			{
				clip.generate();
			}
		}

		// checks and adds clip if clip not exits in array
		public void checkAndAddClips(Animation anim)
		{
			AnimationClip[] animClips = AnimationUtility.GetAnimationClips (anim.gameObject);
			for(int num = 0; num < animClips.Length; num++)
			{
				if(animClips[num]!=null && !hasClip(animClips[num]))
				{
					clips.Add(new LoziAnimationClip(animClips[num],obj.transform));
				}
			}
		}
		
		public bool isSameAnimation(Animation anim)
		{
			if(anim!=null)
			{
				AnimationClip[] animClips = AnimationUtility.GetAnimationClips (anim.gameObject);
				for(int num = 0; num < animClips.Length; num++)
				{
					if(hasClip(animClips[num]))
					{
						return true;
					}
				}
			}
			return false;
		}
		
		public bool hasClip(AnimationClip clip)
		{
			for(int num = 0; num < clips.Count; num++)
			{ 
				if(clips[num].clip == clip)
				{
					return true;
				}
			}
			return false;
		}

		// returns dictionary of generated objects 
		public Dictionary<string,object> animationsDictionary
		{
			get
			{
				Dictionary<string,object> dict = new Dictionary<string,object>();
				List<Dictionary<string,object>> arr = new List<Dictionary<string, object>>();
				for(int num = 0; num < clips.Count; num++)
				{
					arr.Add(clips[num].getClipData(isSkinAnimation));
				}
				dict["id"   ] = id;
				dict["clips"] = arr;
				return dict;
			}
		}

		public int id
		{
			get{return objectId;}
		}

		public string name
		{
			get
			{
				return objName;
			}
		}

		public GameObject animationObject
		{
			get{return obj;}
		}

		public List<LoziAnimationClip> animationClips
		{
			get{return clips;}
		}

		public static bool hasAnimation(GameObject obj)
		{
			if(obj.GetComponent<Animation>()!=null)
			{
				return true;
			}
			return false;
		}

		public void Dispose()
		{
			if(clips!=null)
			{
				for(int num = 0; num < clips.Count; num++)
				{
					clips[num].Dispose();
				}
				clips.Clear();
			}

			clips   = null;
			obj     = null;
			objName = null;
		}
	}
}

