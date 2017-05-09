/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Gets all skin data and stores
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Lozi.baseClasses;

namespace Lozi
{
	public class LoziMeshSkin : HierarchyManager<LoziBone>, IDisposable
	{
		private SkinnedMeshRenderer   renderer;
		private string       	      objectId;
		private List<int>  	  	       indices;
		private List<float>            weights;
		private List<BoneWeight> sortedWeights;
		private Vector3		  	   rootBoneRot;

		public LoziMeshSkin(SkinnedMeshRenderer meshRenderer):base()
		{
			setTarget(meshRenderer.transform);
			renderer 	 = meshRenderer;
			generate();
		}

		// gets bones and sorts by hierarchy index
		public void generate()
		{
			weights = new List<float>();
			indices = new List<int>();
			unsortedObjects = new List<LoziBone>();

			if(renderer.rootBone!=null)
			{
				rootBoneRot = renderer.rootBone.localEulerAngles;

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

				sortObjectsByHierarchyIndex(rootObject,null);

				sortedWeights = new List<BoneWeight>();
				sortBoneWeights();

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
		}

		// sorts weight by sorted bones
		private void sortBoneWeights()
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

		public bool containsTransformInBones(Transform target)
		{
			if(sortedObjects!=null)
			{
				for(int num = 0; num < sortedObjects.Count; num++)
				{
					if(sortedObjects[num].obj == target || sortedObjects[num].obj.name == target.name)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void generateScriptProperties()
		{
			for(int num = 0; num < sortedObjects.Count; num++)
			{
				sortedObjects[num].generateScriptProperties();
			}
		}
		
		public bool exportColliders
		{
			set
			{
				for(int num = 0; num < sortedObjects.Count; num++)
				{
					sortedObjects[num].includeColliders = value;
				}
			}
		}
		
		public bool exportScriptProperties
		{
			set
			{
				for(int num = 0; num < sortedObjects.Count; num++)
				{
					sortedObjects[num].includeScripts = value;
				}
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

		// returns sorted bones data array as dictionary array
		public List<Dictionary<string,object>> bonesDictionary
		{
			get
			{
				List<Dictionary<string,object>> bonesArr = new List<Dictionary<string, object>>();

				if(sortedObjects!=null)
				{
					for(int num = 0; num < sortedObjects.Count; num++)
					{
						bonesArr.Add((sortedObjects[num] as LoziBone).boneDictionary);
					}
				}
				return bonesArr;
			}
		}

		public int rootBoneID
		{
			get{return rootObject.objectId;}
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

		public Vector3 rootBoneRotation
		{
			get
			{
				return rootBoneRot;
			}
		}

		private void clear()
		{
			if(weights!=null)
			{
				weights.Clear();
			}
			if(indices!=null)
			{
				indices.Clear();
			}
			if(sortedObjects!=null)
			{
				sortedWeights.Clear();
			}
		}

		override public void Dispose()
		{
			base.Dispose();
			clear();

			renderer	  = null;
			objectId	  = null;
			indices 	  = null;
			weights 	  = null;
			sortedWeights = null;
		}
	}
}
