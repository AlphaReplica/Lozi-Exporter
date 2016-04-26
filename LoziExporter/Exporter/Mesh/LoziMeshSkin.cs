using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Lozi.baseClasses;

namespace Lozi
{
	public class LoziMeshSkin : HierarchyManager<LoziBone>, IDisposable
	{
		private GameObject                 obj;
		private SkinnedMeshRenderer   renderer;
		private string       	      objectId;
		private string              objectName;
		private List<int>  	  	       indices;
		private List<float>            weights;
		private List<BoneWeight> sortedWeights;
		public LoziMeshSkin(SkinnedMeshRenderer meshRenderer):base()
		{
			setTarget(meshRenderer.transform);
			obj          = meshRenderer.gameObject;
			renderer 	 = meshRenderer;
			objectName   = renderer.sharedMesh.name;
			getSkinProperties();
		}

		private void getSkinProperties()
		{
			weights = new List<float>();
			indices = new List<int>();
			unsortedObjects = new List<LoziBone>();
			Transform hierarchyParent = renderer.transform.parent;

			unsortedObjects.Add(new LoziBone());
			unsortedObjects[0].setObject(renderer.rootBone);
			unsortedObjects[0].isRoot = true;

			for(int num = 0; num < renderer.bones.Length; num++)
			{
				if(!hasInUnsortedArray(renderer.bones[num]))
				{
					LoziBone bone = new LoziBone();
					bone.setObject(renderer.bones[num]);
					unsortedObjects.Add(bone);
				}
			}

			sortObjectsByHierarchyIndex(rootObject);

			sortedWeights = new List<BoneWeight>();
			sortBoneWeitghts();

			for(int num = 0; num < sortedWeights.Count; num++)
			{
				BoneWeight weight = sortedWeights[num];
				weights.Add(weight.weight0);
				weights.Add(weight.weight1);
				weights.Add(weight.weight2);
				weights.Add(weight.weight3);
				
				indices.Add(weight.boneIndex0);
				indices.Add(weight.boneIndex1);
				indices.Add(weight.boneIndex2);
				indices.Add(weight.boneIndex3);
			} 
		}

		private void sortBoneWeitghts()
		{
			for(int num = 0; num < renderer.sharedMesh.boneWeights.Length; num++)
			{
				BoneWeight newWeight = new BoneWeight();
				BoneWeight weight    = renderer.sharedMesh.boneWeights[num];
				LoziBone bone1 = getUnsortedBoneByIndex(weight.boneIndex0);
				LoziBone bone2 = getUnsortedBoneByIndex(weight.boneIndex1);
				LoziBone bone3 = getUnsortedBoneByIndex(weight.boneIndex2);
				LoziBone bone4 = getUnsortedBoneByIndex(weight.boneIndex3);
				
				newWeight.boneIndex0 = getBoneIndex(bone1);
				newWeight.boneIndex1 = getBoneIndex(bone2);
				newWeight.boneIndex2 = getBoneIndex(bone3);
				newWeight.boneIndex3 = getBoneIndex(bone4);
				
				newWeight.weight0 = weight.weight0;
				newWeight.weight1 = weight.weight1;
				newWeight.weight2 = weight.weight2;
				newWeight.weight3 = weight.weight3;
				
				sortedWeights.Add(newWeight);
			}
		}

		public int getBoneIndex(LoziBone bone)
		{
			for(int num = 0; num < sortedObjects.Count; num++)
			{
				if(bone.obj==sortedObjects[num].obj)
				{
					return num;
				}
			}
			return -1;
		}

		public LoziBone getUnsortedBoneByIndex(int index)
		{
			Transform t = renderer.bones[index];
			
			for(int num = 0; num < unsortedObjects.Count; num++)
			{
				if(t==unsortedObjects[num].obj)
				{
					return unsortedObjects[num];
				}
			}
			return null;
		}
		
		public LoziBone getBoneByTarget(Transform target)
		{
			for(int num = 0; num < unsortedObjects.Count; num++)
			{
				if(unsortedObjects[num].obj == target)
				{
					return unsortedObjects[num] as LoziBone;
				}
			}
			return null;
		}

		public string getBoneIdByTarget(Transform target)
		{
			LoziBone bone = getBoneByTarget(target);
			if(bone!=null) 
			{
				return bone.objectId;
			}
			return null;
		}

		public List<Dictionary<string,object>> bonesDictionary
		{
			get
			{
				List<Dictionary<string,object>> bonesArr = new List<Dictionary<string, object>>();
				for(int num = 0; num < sortedObjects.Count; num++)
				{
					bonesArr.Add((sortedObjects[num] as LoziBone).boneDictionary);
				}
				return bonesArr;
			}
		}
		
		public string id
		{
			get{return objectId;}
		}

		public List<float> skinWeights
		{
			get
			{
				return weights;
			}
		}
		
		public List<int> skinIndices
		{
			get
			{
				return indices;
			}
		}

		public void Dispose()
		{
			indices.Clear();
			weights.Clear();
			sortedWeights.Clear();

			obj			  = null;
			renderer	  = null;
			objectId	  = null;
			objectName 	  = null;
			indices 	  = null;
			weights 	  = null;
			sortedWeights = null;
		}
	}
}
