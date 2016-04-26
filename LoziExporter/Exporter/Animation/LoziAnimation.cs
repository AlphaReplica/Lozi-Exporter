using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UniThreeD;
using Lozi.baseClasses;

namespace Lozi
{
	public class LoziAnimation
	{
		List<LoziAnimationClip> clips;
		private GameObject obj;
		private int		       	    objectId;
		public LoziAnimation (GameObject target)
		{
			obj   = target;
			clips = new List<LoziAnimationClip>();
			Animation animation   = target.GetComponent<Animation>();
			objectId = animation.GetInstanceID();
			AnimationClip[] animClips = AnimationUtility.GetAnimationClips (animation);

			for(int num = 0; num < animClips.Length; num++)
			{
				if(animClips[num]!=null)
				{
					clips.Add(new LoziAnimationClip(animClips[num],obj.transform));
				}
			}
		}

		public List<Dictionary<string,object>> animationsDictionary
		{
			get
			{
				List<Dictionary<string,object>> arr = new List<Dictionary<string, object>>();
				for(int num = 0; num < clips.Count; num++)
				{
					arr.Add(clips[num].clipDictionary);
				}
				return arr;
			}
		}

		public GameObject animationObject
		{
			get{return obj;}
		}

		public static bool hasAnimation(GameObject obj)
		{
			if(obj.GetComponent<Animation>()!=null)
			{
				return true;
			}
			return false;
		}
	}
}

