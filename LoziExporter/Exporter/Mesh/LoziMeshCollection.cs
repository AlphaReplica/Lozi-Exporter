using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniThreeD;

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

		public void generateGeometry()
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

		public List<LoziMesh> meshes
		{
			get{return meshObjects;}
		}

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
		
		public int getMeshIdByMesh(Mesh mesh)
		{
			LoziMesh threeMesh = getMeshByMesh(mesh);

			if(threeMesh!=null)
			{
				return threeMesh.id;
			}
			return 0;
		}
		
		public int getMeshIdByGameObject(GameObject obj)
		{
			return getMeshIdByMesh(LoziMesh.getMesh(obj));
		}
		
		public LoziMesh getMeshByGameObject(GameObject obj)
		{
			return getMeshByMesh(LoziMesh.getMesh(obj));
		}
		
		public LoziMesh getMeshByGameObjectOrParent(GameObject obj)
		{
			LoziMesh mesh = getMeshByMesh(LoziMesh.getMesh(obj));
			if(mesh==null)
			{
				for(int num = 0; num < obj.transform.childCount; num++)
				{
					mesh = getMeshByMesh(LoziMesh.getMesh(obj.transform.GetChild(num).gameObject));
					if(mesh!=null)
					{
						return mesh;
					}
				}
			}
			return null;
		}

		public string getBoneIdByTarget(Transform target)
		{
			foreach(LoziMesh threeMesh in meshObjects)
			{
				string id = threeMesh.getBoneIdByTarget(target);
				if(id!=null)
				{
					return id;
				}
			}
			return null;
		}

		public void Dispose()
		{
			for(int num = 0; num < meshObjects.Count; num++)
			{
				meshObjects[num].Dispose();
			}
			meshObjects.Clear();
			meshObjects = null;
		}
	}
}
