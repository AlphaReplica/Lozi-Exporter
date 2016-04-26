using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziObject
	{
		public bool isFoldedInUI;

		private int		       	    objectId;
		private string            objectName;
		private string             objectTag;
		private int		              meshID;
		private string                  type;
		private GameObject  		     obj;
		private List<LoziObject>    children;
		private List<LoziMesh>        meshes;
		private LoziObject			  parent;
		private LoziMesh				mesh;
		private LoziAnimation 	   animation;
		private LoziCamera			  camera;
		private LoziLight			   light;
		private List<float>     	position;
		private List<float>  		rotation;
		private List<float>			   scale;

		public LoziObject(GameObject target,string objType = "")
		{
			obj        = target;
			objectId   = target.transform.GetInstanceID();

			objectName = obj.name;
			objectTag  = (obj.tag == "Untagged") ? "" : obj.tag;
			//mesh       = LoziExporter.instance.;
			//meshID     = (ThreeMesh.getMesh(obj)!=null) ? assets.getMeshIdByGameObject(obj) : "";

			setComponents();

			Vector3 posVec3   = obj.transform.localPosition;
			Vector3 rotVec3   = obj.transform.localEulerAngles;
			Vector3 scaleVec3 = obj.transform.localScale;
			
			position = new List<float>(){posVec3.x,    posVec3.y,  posVec3.z};
			rotation = new List<float>(){rotVec3.x,    rotVec3.y,  rotVec3.z};
			scale	 = new List<float>(){scaleVec3.x,scaleVec3.y,scaleVec3.z};

			children = new List<LoziObject>();

			setType(objType);

			for(int num = 0; num < obj.transform.childCount; num++)
			{
				children.Add(new LoziObject(obj.transform.GetChild(num).gameObject));
				//children[children.Count].setParent(this);
			}
		}

		private void setComponents()
		{
			if(LoziMesh.hasMesh(obj))
			{
				mesh = LoziExporter.instance.meshCollection.getMeshByGameObject(obj);
			}
			if(LoziCamera.hasCamera(obj))
			{
				camera = new LoziCamera(obj);
			}
			if(LoziLight.hasLight(obj))
			{
				light = new LoziLight(obj);
			}
			if(LoziAnimation.hasAnimation(obj))
			{
				animation = LoziExporter.instance.animationCollection.getAnimationByGameObject(obj);
			}
		}

		private void setType(string objType = "")
		{
			if(objType=="")
			{
				type = "Object";
				if(animation!=null)
				{
					type = "AnimationObject";
				}
				if(camera!=null)
				{
					type = "Camera";
				}
				if(light!=null)
				{
					type = "Light";
				}
				if(mesh!=null)
				{
					if(mesh.meshType == LoziMesh.MeshType.Static)
					{
						type = "Mesh";
					}
					if(mesh.meshType == LoziMesh.MeshType.Skinned)
					{
						type = "SkinnedMesh";
					}
				}
			}
			else
			{
				type = objType;
			}
		}

		public void setParent(LoziObject parent)
		{
			this.parent = parent;
		}

		public bool hasChildren(GameObject obj)
		{
			if(obj.transform.childCount>0)
			{
				return true;
			}
			return false;
		}

		public Dictionary<string,object> objectProperties
		{
			get
			{
				Dictionary<string,object> dict = new Dictionary<string, object>();
			
				dict["id"		] = objectId;
				dict["name"		] = objectName;
				if(objectTag.Length>0)
				{
					dict["tag"	] = objectTag;
				}
				dict["type"		] = type;
				dict["transform"] = getObjectTransform(obj);
			
				if(meshID!=0)
				{
					dict["mesh"] = meshID;
					//dict["material" ] = getObjectMaterial(obj);
				}
				if(children.Count>0)
				{
					dict["children" ] = childrenProperties;
				}
				return dict;
			}
		}

		public LoziObject getById(int id)
		{
			LoziObject returnObj = null;
			if(id == objectId)
			{
				returnObj = this;
			}
			else
			{
				for(int num = 0; num < children.Count; num++)
				{
					returnObj = children[num].getById(id);
					if(returnObj!=null)
					{
						break;
					}
				}
			}
			return returnObj;
		}

		public Dictionary<string,object> getObjectTransform(GameObject obj)
		{
			Dictionary<string,object> dict = new Dictionary<string,object>();

			dict["position"] = position;
			dict["rotation"] = rotation;
			dict["scale"   ] = scale;

			return dict;
		}

		public List<Dictionary<string,object>> childrenProperties
		{
			get
			{
				List<Dictionary<string,object>> childrenArr = new List<Dictionary<string, object>>();

				for(int num = 0; num < children.Count; num++)
				{
					childrenArr.Add(children[num].objectProperties);
				}
				return childrenArr;
			}
		}

		private List<LoziMesh> getMeshes(List<LoziMesh> mesheObjectsArray = null)
		{
			if(mesheObjectsArray==null)
			{
				mesheObjectsArray = new List<LoziMesh>();
			}

			if(mesh!=null)
			{
				mesheObjectsArray.Add(mesh);
				
				for(int num = 0; num < children.Count; num++)
				{
					children[num].getMeshes(mesheObjectsArray);
				}
			}
			return mesheObjectsArray;
		}

		public List<LoziMesh> meshObjects
		{
			get
			{
				if(meshes==null)
				{
					meshes = getMeshes();
				}
				return meshes;
			}
		}

		public void setMeshID(int id)
		{
			meshID = id;
		}
		
		public int id
		{
			get{return objectId;}
		}
		
		public string name
		{
			get{return objectName;}
		}
		
		public string tag
		{
			get{return objectTag;}
		}

		public GameObject targetGameObject
		{
			get{return obj;}
		}

		public List<LoziObject> childObjects
		{
			get{return children;}
		}

		public string objectType
		{
			get{return type;}
		}
	}
}