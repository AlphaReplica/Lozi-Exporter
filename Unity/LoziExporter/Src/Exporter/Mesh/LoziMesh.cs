/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * mesh object which divided into geometry, morph and skin
 * gets geometry from gameobject and generates data
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziMesh : IDisposable
	{
		public enum MeshType {None = 0, Static=1, Skinned=2};

		public bool	    exportVertexColors;
		public bool[] 	   	     exportUvs;
		public bool 		  isFoldedInUI;
		public bool			recieveShadows;
		public bool            castShadows;
		public int              rootBoneID;
		private Mesh       		   oldMesh;
		private Mesh       			  mesh;
		private LoziMeshGeometry  geometry;
		private LoziMeshMorpher      morph;
		private LoziMeshSkin	      skin;
		private MeshType   			  type;
		private GameObject  		   obj;
		private string			   objName;

		public LoziMesh(GameObject target)
		{
			exportUvs = new bool[2] { true, true };
			obj       = target;
			setMeshAndType();
			getComponentsInfo();
		}

		// Sets all mesh data, if SkinMeshRenderer provided created new Unity Mesh object to bake current pose of mesh
		// also it's stripped down to null parent to get all coordinates correctly
		// After snapshot it's assembled as provided
		private void getComponentsInfo()
		{
			if(type!=MeshType.None)
			{
				Transform parent 		 	   = obj.transform.parent; 
				Vector3 position 		 	   = obj.transform.localPosition;
				Vector3 euler	 		 	   = obj.transform.localEulerAngles;
				Vector3 scale 	 		 	   = obj.transform.localScale;
				obj.transform.parent	 	   = null;
				obj.transform.localPosition    = Vector3.zero;
				obj.transform.localEulerAngles = Vector3.zero;
				obj.transform.localScale 	   = Vector3.one;

				objName = mesh.name;
				if(type==MeshType.Static)
				{
					recieveShadows = obj.GetComponent<MeshRenderer>().receiveShadows; 
					castShadows	   = (obj.GetComponent<MeshRenderer>().shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off);
					geometry 	   = new LoziMeshGeometry(mesh,exportVertexColors,exportUvs);
				}
				if(type==MeshType.Skinned)
				{
					oldMesh = mesh;
					if(obj.GetComponent<SkinnedMeshRenderer>().rootBone!=null)
					{
						Transform rootBone		 			= obj.GetComponent<SkinnedMeshRenderer>().rootBone;
						Transform rootBoneParent 			= rootBone.parent;
						Vector3 rootBonePosition 			= rootBone.transform.localPosition;
						Vector3 rootBoneEuler	 			= rootBone.transform.localEulerAngles;
						Vector3 rootBoneScale 	 			= rootBone.transform.localScale;

						rootBone.parent 		 			= null;
						rootBone.transform.localPosition 	= rootBonePosition;
						rootBone.transform.localEulerAngles = rootBoneEuler;
						rootBone.transform.localScale    	= rootBoneScale;
						
						obj.transform.localPosition         = Vector3.zero;
						obj.transform.localEulerAngles      = Vector3.zero;
						obj.transform.localScale            = Vector3.one;

						recieveShadows						= obj.GetComponent<SkinnedMeshRenderer>().receiveShadows;
						castShadows							= (obj.GetComponent<SkinnedMeshRenderer>().shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off);

						mesh = new Mesh();
						obj.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);

						geometry = new LoziMeshGeometry(mesh,exportVertexColors,exportUvs);
						skin     = new LoziMeshSkin    (obj.GetComponent<SkinnedMeshRenderer>());
						morph    = new LoziMeshMorpher (obj.GetComponent<SkinnedMeshRenderer>());
						geometry.id = oldMesh.GetInstanceID();

						rootBone.parent 					= rootBoneParent;
						rootBone.transform.localPosition 	= rootBonePosition;
						rootBone.transform.localEulerAngles = rootBoneEuler;
						rootBone.transform.localScale    	= rootBoneScale;

						rootBoneID = skin.rootBoneID;
					}
					else
					{
						geometry = new LoziMeshGeometry(mesh,exportVertexColors,exportUvs);
						morph    = new LoziMeshMorpher (obj.GetComponent<SkinnedMeshRenderer>());
					}
					obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = oldMesh;
				}
				obj.transform.parent 	 	   = parent;
				obj.transform.localPosition    = position;
				obj.transform.localEulerAngles = euler;
				obj.transform.localScale 	   = scale;
			}
		}

		// Generates data by stored mesh
		public void generate()
		{
			if(geometry!=null)
			{
				geometry.generate(exportVertexColors,exportUvs);
			}
			if(morph!=null)
			{
				morph.generate();
			}
		}

		public void resetBoneID()
		{
			if(skin!=null)
			{
				rootBoneID = skin.rootBoneID;
			}
		}

		public void generateScriptProperties()
		{
			if(skin!=null)
			{
				skin.generateScriptProperties();
			}
		}


		// returns mesh data in dictionary
		public Dictionary<string,object> meshProperties
		{
			get
			{
				if(mesh!=null)
				{
					Dictionary<string,object> meshDict  = new Dictionary<string, object>();
					meshDict["id"      ] = id;
					meshDict["name"    ] = objName;

					Dictionary<string,object> geomDict  = new Dictionary<string, object>();

					geomDict["vertices"		 ] = geometry.vertices;
					geomDict["normals" 		 ] = geometry.normals;
					geomDict["uv" 	   		 ] = geometry.uvs;
					geomDict["faces"   		 ] = geometry.faces;
					geomDict["castShadows"	 ] = castShadows;
					geomDict["recieveShadows"] = recieveShadows;
					meshDict["geometry"		 ] = geomDict;

					if(skin!=null)
					{
						Dictionary<string,object> skinDict  = new Dictionary<string, object>();

						skinDict["bones"      ] = rootBoneID; //skin.bonesDictionary;
						skinDict["skinIndices"] = skin.skinIndices;
						skinDict["skinWeights"] = skin.skinWeights;

						meshDict["skin"       ] = skinDict;
					}
					if(morph!=null)
					{
						Dictionary<string,object> morphDict = new Dictionary<string, object>();

						morphDict["blendShapes"] = morph.morphsDictionary;
						meshDict ["morph"      ] = morphDict;
					}

					return meshDict;
				}
				else
				{
					return null;
				}
			}
		}

		public Dictionary<string,object> boneProperties
		{
			get
			{
				if(skin!=null)
				{
					Dictionary<string,object> dict  = new Dictionary<string, object>();
					dict["id"      ] = skin.rootBoneID;
					dict["bones"   ] = skin.bonesDictionary;

					return dict;
				}
				return null;
			}
		}

		public int defaultRootBoneID
		{
			get{return skin.rootBoneID;}
		}
		
		public bool exportColliders
		{
			set
			{
				if(skin!=null)
				{
					skin.exportColliders = value;
				}
			}
		}
		
		public bool exportScriptProperties
		{
			set
			{
				if(skin!=null)
				{
					skin.exportScriptProperties = value;
				}
			}
		}
		
		public int id
		{
			get
			{
				if(geometry!=null)
				{
					return geometry.id;
				}
				return 0;
			}
		}

		public string name
		{
			get
			{
				return objName;
			}
		}

		public MeshType meshType
		{
			get{return type;}
		}
 
		public bool isSameMesh(Mesh meshObj)
		{
			if(mesh==meshObj || oldMesh==meshObj)
			{
				return true;
			}
			return false;
		}

		public Vector3 rootBoneRotation
		{
			get
			{
				if(skin!=null)
				{ 
					return skin.rootBoneRotation;
				}
				return Vector3.zero;
			}
		}

		public bool containsBone(Transform target)
		{
			if(skin!=null)
			{
				return skin.containsTransformInBones(target);
			}
			return false;
		}

		private void setMeshAndType()
		{
			type = MeshType.None;
			if(obj.GetComponent<SkinnedMeshRenderer>()!=null && 
			   obj.GetComponent<SkinnedMeshRenderer>().sharedMesh!=null)
			{
				type = MeshType.Skinned;
				mesh = obj.GetComponent<SkinnedMeshRenderer>().sharedMesh;
			}
			if(obj.GetComponent<MeshFilter>()!=null && 
			   obj.GetComponent<MeshFilter>().sharedMesh!=null)
			{
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

		
		public static Renderer getRenderer(GameObject gameObj)
		{
			if(gameObj.GetComponent<Renderer>()!=null)
			{
				return gameObj.GetComponent<Renderer>();
			}
			return null;
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
