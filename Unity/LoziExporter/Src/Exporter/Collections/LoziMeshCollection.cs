/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Meshes collection, stores all Meshes of object and its children
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziMeshCollection : IDisposable
	{
		private List<LoziMesh> meshObjects;

		public LoziMeshCollection(GameObject obj)
		{
			meshObjects = new List<LoziMesh>();

			foreach(Transform transform in obj.GetComponentsInChildren<Transform>()) 
			{
				Mesh mesh = LoziMesh.getMesh(transform.gameObject);
				if(mesh!=null && canAddInArray(mesh))
				{
					meshObjects.Add(new LoziMesh(transform.gameObject));
				}
			}
		}

		public void generate()
		{
			foreach(LoziMesh mesh in meshObjects) 
			{
				mesh.generate();
			}
		}

		private bool canAddInArray(Mesh mesh)
		{
			foreach(LoziMesh threeMesh in meshObjects)
			{
				if(threeMesh.isSameMesh(mesh))
				{
					return false;
				}
			}
			return true;
		}
		
		public bool exportColliders
		{
			set
			{
				for(int num = 0; num < meshes.Count; num++)
				{
					meshes[num].exportColliders = value;
				}
			}
		}
		
		public bool exportScriptProperties
		{
			set
			{
				for(int num = 0; num < meshes.Count; num++)
				{
					meshes[num].exportScriptProperties = value;
				}
			}
		}

		public void generateScriptProperties()
		{
			for(int num = 0; num < meshes.Count; num++)
			{
				meshes[num].generateScriptProperties();
			}
		}


		public List<LoziMesh> meshes
		{
			get{return meshObjects;}
		}

		// returns generated meshes data dictionary
		public List<Dictionary<string,object>> meshesProperties
		{
			get
			{
				List<Dictionary<string,object>> arr = new List<Dictionary<string, object>>();

				for(int num = 0; num < meshObjects.Count; num++)
				{
					arr.Add(meshObjects[num].meshProperties);
				}

				return arr;
			}
		}
		
		public T[] rootBoneIDArray<T>()
		{
			List<T> ids = new List<T>();
			
			for(int num = 0; num < meshObjects.Count; num++)
			{
				if(meshObjects[num].boneProperties!=null)
				{
					ids.Add((T) Convert.ChangeType(meshObjects[num].defaultRootBoneID, typeof(T)));
				}
			}
			return ids.ToArray();
		}

		public bool isBonesUsed(int rootBoneID)
		{
			for(int num = 0; num < meshObjects.Count; num++)
			{
				if(meshObjects[num].boneProperties!=null)
				{
					if(meshObjects[num].rootBoneID == rootBoneID)
					{
						return true;
					}
				}
			}
			return false;
		}

		public List<Dictionary<string,object>> boneProperties
		{
			get
			{
				List<Dictionary<string,object>> arr = new List<Dictionary<string, object>>();
				
				for(int num = 0; num < meshObjects.Count; num++)
				{
					if(meshObjects[num].boneProperties!=null && isBonesUsed(meshObjects[num].defaultRootBoneID))
					{
						arr.Add(meshObjects[num].boneProperties);
					}
				}
				
				return arr;
			}
		}
		
		public LoziMesh getMeshByMesh(Mesh mesh)
		{
			foreach(LoziMesh meshObj in meshObjects)
			{
				if(meshObj.isSameMesh(mesh))
				{
					return meshObj;
				}
			}
			return null;
		}

		public LoziMesh getMeshByGameObject(GameObject obj)
		{
			return getMeshByMesh(LoziMesh.getMesh(obj));
		}

		public bool containsBone(Transform target)
		{
			foreach(LoziMesh mesh in meshObjects)
			{
				if(mesh.containsBone(target)==true)
				{
					return true;
				}
			}
			return false;
		}

		public void Dispose()
		{
			if(meshObjects!=null && meshObjects.Count>0)
			{
				for(int num = 0; num < meshObjects.Count; num++)
				{
					meshObjects[num].Dispose();
				}
				meshObjects.Clear();
			}
			meshObjects = null;
		}
	}
}
