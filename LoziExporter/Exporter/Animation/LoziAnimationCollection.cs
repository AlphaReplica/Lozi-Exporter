using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UniThreeD;

namespace Lozi
{
	public class LoziAnimationCollection
	{
		private List<LoziAnimation> animationObjects;

		public LoziAnimationCollection (GameObject obj)
		{
			animationObjects = new List<LoziAnimation>();

			foreach(Animation animation in obj.GetComponentsInChildren<Animation>()) 
			{
				animationObjects.Add(new LoziAnimation(animation.gameObject));
			}
		}

		public LoziAnimation getAnimationByGameObject(GameObject obj)
		{
			foreach(LoziAnimation animation in animationObjects)
			{
				if(animation.animationObject == obj)
				{
					return animation;
				}
			}
			return null;
		}
		private bool canAddInArray(Mesh mesh)
		{
			foreach(LoziAnimation threeAnimation in animationObjects)
			{
				//if(threeAnimation.isSameMesh(mesh))
				//{
				//	return false;
				//}
			}
			return true;
		}

		public List<Dictionary<string,object>> animationsDictionary
		{
			get
			{
				List<Dictionary<string,object>> arr = new List<Dictionary<string, object>>();
				for(int num = 0; num < animationObjects.Count; num++)
				{
					Debug.Log(animationObjects[num].animationsDictionary);
				}
				
				return animationObjects[0].animationsDictionary;
			}
		}
	}
}

