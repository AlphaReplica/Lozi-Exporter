using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Lozi.helpers;

namespace Lozi
{
	public class LoziMeshMorpher : IDisposable
	{
		private SkinnedMeshRenderer renderer;
		private string       	    objectId;
		private string            objectName;
		private List<MorphObject>  morphList;
		public LoziMeshMorpher(SkinnedMeshRenderer meshRenderer)
		{
			renderer 	 = meshRenderer;
			objectName   = renderer.sharedMesh.name;
		//	objectId     = UniThreeD.ThreeJS.generateGUID("Morph");
			morphList    = new List<MorphObject>();
			parseMeshBlends();
		}

		private void parseMeshBlends()
		{
			if(meshObject.blendShapeCount>0)
			{
				Mesh mesh  = new Mesh();
				
				for(int num=0; num<meshObject.blendShapeCount; num++)
				{
					MorphObject obj = new MorphObject();
					obj.vertices    = new List<float>();

					obj.name        = meshObject.GetBlendShapeName(num);

					renderer.SetBlendShapeWeight(num,100);
					renderer.BakeMesh(mesh);
					
					foreach(Vector3 vertice in mesh.vertices)
					{
						obj.vertices.Add(vertice.x);
						obj.vertices.Add(vertice.y);
						obj.vertices.Add(vertice.z);
					}

					morphList.Add(obj);
					renderer.SetBlendShapeWeight(num,0);
				}
				mesh.Clear();
				mesh = null;
			}
		}

		public string id
		{
			get{return objectId;}
		}
		
		public string name
		{
			get{return objectName;}
		}
		
		public Mesh meshObject
		{
			get{return renderer.sharedMesh;}
		}
		
		public List<MorphObject> morphs
		{
			get{return morphList;}
		}
		
		public List<Dictionary<string,object>> morphsDictionary
		{
			get
			{
				List<Dictionary<string,object>> arr = new List<Dictionary<string, object>>();

				for(int num = 0; num < morphList.Count; num++)
				{
					Dictionary<string,object> dict = new Dictionary<string, object>();
					dict["name"    ] = morphList[num].name;
					dict["vertices"] = morphList[num].vertices;
					arr.Add(dict);
				}
				return arr;
			}
		}

		public void Dispose()
		{
			for(int num = 0; num < morphList.Count; num++)
			{
				morphList[num].Dispose();
			}
			morphList.Clear();

			renderer   = null;
			objectId   = null;
			objectName = null;
			morphList  = null;
		}
	}
}	