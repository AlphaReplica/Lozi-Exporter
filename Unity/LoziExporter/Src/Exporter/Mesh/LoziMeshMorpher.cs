/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Gets morph targets if exits and stores as array
 */

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
		private string            objectName;
		private List<MorphObject>  morphList;
		public LoziMeshMorpher(SkinnedMeshRenderer meshRenderer)
		{
			renderer 	 = meshRenderer;
			objectName   = renderer.sharedMesh.name;
		}

		public void generate()
		{
			clear();
			morphList    = new List<MorphObject>();

			if(meshObject.blendShapeCount>0)
			{
				Mesh mesh  	  = new Mesh();

				for(int num=0; num<meshObject.blendShapeCount; num++)
				{
					renderer.SetBlendShapeWeight(num,0);
				}

				Transform parent = renderer.transform.parent;
				Vector3 scale    = renderer.transform.localScale;
				Bounds bounds 	 = meshObject.bounds;

				renderer.transform.parent = null;
				meshObject.RecalculateBounds();

				for(int num=0; num<meshObject.blendShapeCount; num++)
				{
					MorphObject obj = new MorphObject();
					obj.vertices    = new List<float>();

					obj.name        = meshObject.GetBlendShapeName(num);

					renderer.SetBlendShapeWeight(num,100);
					renderer.BakeMesh(mesh);

					foreach(Vector3 vertice in mesh.vertices)
					{
						obj.vertices.Add(-(vertice.x) / renderer.transform.localScale.x);
						obj.vertices.Add(  vertice.y  / renderer.transform.localScale.y);
						obj.vertices.Add(  vertice.z  / renderer.transform.localScale.z);
					}

					morphList.Add(obj);
					renderer.SetBlendShapeWeight(num,0);
				}

				renderer.transform.parent = parent;
				renderer.transform.localScale = scale;
				meshObject.RecalculateBounds();

				mesh.Clear();
				mesh = null;
			}
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

		private void clear()
		{
			if(morphList!=null)
			{
				for(int num = 0; num < morphList.Count; num++)
				{
					morphList[num].Dispose();
				}
				morphList.Clear();
			}
		}

		public void Dispose()
		{
			clear();

			renderer   = null;
			objectName = null;
			morphList  = null;
		}
	}
}	