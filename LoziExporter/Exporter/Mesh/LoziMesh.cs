using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziMesh : IDisposable
	{
		public enum MeshType {None = 0, Static=1, Skinned=2};

		public Vector3	 geometryRotOffset;
		public bool	    exportVertexColors;
		public bool[] 	   	     exportUvs;
		public bool 		  isFoldedInUI;

		private Mesh       			  mesh;
		private LoziMeshGeometry  geometry;
		private LoziMeshMorpher      morph;
		private LoziMeshSkin	      skin;
		private MeshType   			  type;
		private int		       	  objectId;
		private GameObject  		   obj;

		public LoziMesh(GameObject target)
		{
			exportUvs = new bool[2] { true, true };
			obj       = target;
			objectId  = obj.GetInstanceID();
			setMeshAndType();
		}

		public void generate()
		{
			if(type!=MeshType.None)
			{
				if(type==MeshType.Static)
				{
					geometry = new LoziMeshGeometry(mesh,geometryRotOffset,exportVertexColors,exportUvs);
				}
				if(type==MeshType.Skinned)
				{
					mesh = new Mesh();
					obj.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
					geometry = new LoziMeshGeometry(mesh,geometryRotOffset,exportVertexColors,exportUvs);
					morph    = new LoziMeshMorpher (obj.GetComponent<SkinnedMeshRenderer>());
					skin     = new LoziMeshSkin    (obj.GetComponent<SkinnedMeshRenderer>());
				}
			}
		}

		public Dictionary<string,object> meshProperties
		{
			get
			{
				if(mesh!=null)
				{
					Dictionary<string,object> meshDict  = new Dictionary<string, object>();
					meshDict["id"      ] = id;

					Dictionary<string,object> geomDict  = new Dictionary<string, object>();
					
					geomDict["id"      ] = geometry.id;
					geomDict["name"    ] = geometry.name;
					geomDict["vertices"] = geometry.vertices;
					geomDict["normals" ] = geometry.normals;
					geomDict["uv" 	   ] = geometry.uvs;
					geomDict["faces"   ] = geometry.faces;

					meshDict["geometry"] = geomDict;

					if(skin!=null)
					{
						Dictionary<string,object> skinDict  = new Dictionary<string, object>();
						
						skinDict["id"         ] = skin.id;
						skinDict["bones"      ] = skin.bonesDictionary;
						skinDict["skinIndices"] = skin.skinIndices;
						skinDict["skinWeights"] = skin.skinWeights;

						meshDict["skin"       ] = skinDict;
					}
					if(morph!=null)
					{
						Dictionary<string,object> morphDict = new Dictionary<string, object>();

						morphDict["id"         ] = morph.id;
						morphDict["blendShapes"] = morph.morphsDictionary;
						
						meshDict["morph"       ] = morphDict;
					}

					return meshDict;
				}
				else
				{
					return null;
				}
			}
		}
		
		public int id
		{
			get{return objectId;}
		}

		public string name
		{
			get{return mesh.name;}
		}

		public MeshType meshType
		{
			get{return type;}
		}

		public bool isSameMesh(Mesh meshObj)
		{
			if(mesh==meshObj)
			{
				return true;
			}
			return false;
		}
		
		public string getBoneIdByTarget(Transform target)
		{
			if(skin!=null)
			{
				return skin.getBoneIdByTarget(target);
			}
			return null;
		}

		private void setMeshAndType()
		{
			type = MeshType.None;
			if(obj.GetComponent<SkinnedMeshRenderer>()!=null && 
			   obj.GetComponent<SkinnedMeshRenderer>().sharedMesh!=null)
			{
				geometryRotOffset = obj.transform.localEulerAngles;

				type = MeshType.Skinned;
				mesh = obj.GetComponent<SkinnedMeshRenderer>().sharedMesh;
			}
			if(obj.GetComponent<MeshFilter>()!=null && 
			   obj.GetComponent<MeshFilter>().sharedMesh!=null)
			{
				geometryRotOffset = obj.transform.localEulerAngles;

				type = MeshType.Static;
				mesh = obj.GetComponent<MeshFilter>().sharedMesh;
			}
		}

		public void Dispose()
		{
			geometry.Dispose();
			if(skin!=null)
			{
				skin.Dispose();
			}
			if(morph!=null)
			{
				morph.Dispose();
			}

			exportUvs = null;
			mesh 	  = null;
			geometry  = null;
			morph	  = null;
			skin	  = null;
			obj       = null;
		}

		public static Mesh getMesh(GameObject gameObj)
		{
			if(gameObj.GetComponent<SkinnedMeshRenderer>()!=null && 
			   gameObj.GetComponent<SkinnedMeshRenderer>().sharedMesh!=null)
			{
				return gameObj.GetComponent<SkinnedMeshRenderer>().sharedMesh;
			}
			if(gameObj.GetComponent<MeshFilter>()!=null && 
			   gameObj.GetComponent<MeshFilter>().sharedMesh!=null)
			{
				return gameObj.GetComponent<MeshFilter>().sharedMesh;
			}
			return null;
		}

		public static bool hasMesh(GameObject gameObj)
		{
			return (getMesh(gameObj)==null) ? false : true;
		}
	}
}
