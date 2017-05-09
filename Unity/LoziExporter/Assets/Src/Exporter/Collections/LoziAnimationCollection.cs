/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Animations collection, stores all animations of object and its children
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziAnimationCollection : IDisposable
	{
		private List<LoziAnimation> animationObjects;

		public LoziAnimationCollection (GameObject obj)
		{
			animationObjects = new List<LoziAnimation>();

			foreach(Animator animation in obj.GetComponentsInChildren<Animator>()) 
			{
                if (animation.runtimeAnimatorController != null)
                {
                    if (animation.runtimeAnimatorController.animationClips.Length > 0)
                    {
                        LoziAnimation anm = hasInArray(animation);
                        if (anm != null)
                        {
                            anm.checkAndAddClips(animation);
                        }
                        else
                        {
                            animationObjects.Add(new LoziAnimation(animation.gameObject));
                        }
                    }
                }
			}
		}

		public void generate()
		{
			foreach(LoziAnimation anim in animationObjects) 
			{
				anim.generate();
			}
		}

		public LoziAnimation getAnimationByGameObject(GameObject obj)
		{
			return hasInArray(obj.GetComponent<Animator>());
		}

		private LoziAnimation hasInArray(Animator animation)
		{
			foreach(LoziAnimation animationObj in animationObjects)
			{
				if(animationObj.isSameAnimation(animation))
				{
					return animationObj;
				}
			}
			return null;
		}

		public List<LoziAnimation> animations
		{
			get
			{
				return animationObjects;
			}
		}

		// returns all generated animation objects dictionary
		public Dictionary<string,object> animationsDictionary
		{
			get
			{
				List<Dictionary<string,object>> skins = new List<Dictionary<string,object>>();
				List<Dictionary<string,object>> trans = new List<Dictionary<string,object>>();
				for(int num = 0; num < animationObjects.Count; num++)
				{
					if(animationObjects[num].isSkinAnimation)
					{
						skins.Add(animationObjects[num].animationsDictionary);
					}
					else
					{
						trans.Add(animationObjects[num].animationsDictionary);
					}
				}

				Dictionary<string, object> dict = new Dictionary<string,object>();
				dict["skin"]	  = skins;
				dict["transform"] = trans;
				return dict;
			}
		}

		public void Dispose()
		{ 
			for(int num = 0; num < animationObjects.Count; num++)
			{
				animationObjects[num].Dispose();
			}
			animationObjects.Clear();
			animationObjects = null;
		}
	}
}

