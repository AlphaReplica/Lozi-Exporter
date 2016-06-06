/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Hierarchy object represents gameobject of type according to component
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public enum ObjectType {None               = 0,
							Scene    		   = 1,
							Object3D 		   = 2,
							AnimationObject    = 3,
							Object			   = 4,
							Bone               = 5,
							SpotLight   	   = 6,
							DirectionalLight   = 7,
							PointLight 		   = 8,
							AreaLight 		   = 9,
							PerspectiveCamera  = 10,
							OrthographicCamera = 11,
							SkinnedMesh 	   = 12,
							Mesh 			   = 13};

	public class LoziObject
	{
		public bool 			isFoldedInUI;
		public List<bool>       	    flip;

		private int		       	    objectId;
		private string            objectName;
		private string             objectTag;
		private int		          materialID;
		private int		              meshID;
		private ObjectType              type;
		private GameObject  		     obj;
		private bool				  isBone;
		private List<LoziObject>    children;
		private LoziObject			  parent;
		private LoziMesh				mesh;
		private LoziMaterial    	material;
		private LoziAnimation 	   animation;
		private LoziCamera			  camera;
		private LoziLight			   light;

		// Sets properties and recursively adds children objects to children array
		public LoziObject(GameObject target,ObjectType objType)
		{
			flip       = new List<bool>(){true,false,false};
			obj        = target;
			objectId   = target.transform.GetInstanceID();

			objectName = obj.name;
			objectTag  = (obj.tag == "Untagged") ? "" : obj.tag;
			isBone	   = LoziExporter.instance.meshCollection.containsBone(obj.transform);
			setComponents();

			children = new List<LoziObject>();

			setType(objType);

			for(int num = 0; num < obj.transform.childCount; num++)
			{
				children.Add(new LoziObject(obj.transform.GetChild(num).gameObject,ObjectType.None));
				children[children.Count-1].setParent(this);
			}
		}

		//sets component of type if exists
		private void setComponents()
		{
			if(LoziMesh.hasMesh(obj))
			{
				mesh = LoziExporter.instance.meshCollection.getMeshByGameObject(obj);
			}
			if(LoziMaterial.hasMaterial(obj))
			{
				material = LoziExporter.instance.materialCollection.getMaterialByGameObject(obj);
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

		//Sets type of object by existing component
		private void setType(ObjectType objType)
		{
			if(objType==ObjectType.None)
			{
				type = ObjectType.Object;
				if(mesh!=null)
				{
					meshID = mesh.id;
					switch(mesh.meshType)
					{
						case LoziMesh.MeshType.Static :{type = ObjectType.Mesh; 	   break;}
						case LoziMesh.MeshType.Skinned:{type = ObjectType.SkinnedMesh; break;}
					}
				}
				if(light!=null)
				{
					switch(light.type)
					{
						case LightType.Spot		  :{type = ObjectType.SpotLight; 		break;}
						case LightType.Directional:{type = ObjectType.DirectionalLight; break;}
						case LightType.Point	  :{type = ObjectType.PointLight; 		break;}
						case LightType.Area		  :{type = ObjectType.AreaLight;	    break;}
					}
				}
				
				if(camera!=null)
				{
					if(camera.orthographic){type = ObjectType.OrthographicCamera;}
					else				   {type = ObjectType.PerspectiveCamera; }
				}
				if(material!=null)
				{
					materialID = material.id;
				}
				if(animation!=null)
				{
					type = ObjectType.AnimationObject;
				}
				if(isBone)
				{
					type = ObjectType.Bone;
				}
			}
			else
			{
				if(mesh!=null)
				{
					meshID = mesh.id;
				}
				type = objType;
			}
		}

		public void setParent(LoziObject parent)
		{
			this.parent = parent;
			if(type == ObjectType.SkinnedMesh && this.parent.type == ObjectType.AnimationObject)
			{
				if(this.parent.animation!=null)
				{
					this.animation = this.parent.animation;
					this.animation.isSkinAnimation = true;
					this.parent.animation = null;
				}
			}
		}

		public List<float> flipArray
		{
			get
			{
				return new List<float>(){(flip[0]) ? -1 : 1,(flip[1]) ? -1 : 1,(flip[2]) ? -1 : 1};
			}
		}

		public bool hasChildren(GameObject obj)
		{
			if(obj.transform.childCount>0)
			{
				return true;
			}
			return false;
		}

		// current object properties as dictionary
		public Dictionary<string,object> objectProperties
		{
			get
			{
				if(!isBone)
				{
					Dictionary<string,object> dict = new Dictionary<string, object>();
					
					dict["id"		] = objectId;
					dict["name"		] = objectName;
					if(objectTag.Length>0)
					{
						dict["tag"	] = objectTag;
					}
					dict["type"		] = (int)type;
					dict["transform"] = getObjectTransform(obj);
					dict["flip"		] = flipArray;
					if(material!=null && materialID!=0)
					{
						dict["materialID"] = materialID;
					}
					if(mesh!=null && meshID!=0)
					{
						dict["meshID"] = meshID;
					}
					if(animation!=null && animation.id!=0)
					{
						dict["animationID"] = animation.id;
					}
					if(light!=null)
					{
						dict["lightData"] = light.objectProperties;
					}
					if(camera!=null)
					{
						dict["cameraData"] = camera.objectProperties;
					}
					if(children.Count>0)
					{
						dict["children"] = childrenProperties;
					}
					return dict;
				}
				return null;
			}
		}

		// Gets child by id
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

		// transform component as dictinary
		public Dictionary<string,object> getObjectTransform(GameObject obj)
		{
			Dictionary<string,object> dict = new Dictionary<string,object>();

			Vector3 posVec3   = obj.transform.localPosition;
			Vector3 rotVec3   = obj.transform.localEulerAngles;
			Vector3 scaleVec3 = obj.transform.localScale;

			if(mesh!=null)
			{
				if(type == ObjectType.SkinnedMesh && this.parent.type == ObjectType.AnimationObject)
				{
					rotVec3 = Vector3.zero;
				}
			}

			dict["position"] = new List<float>(){posVec3.x,    posVec3.y,  posVec3.z};
			dict["rotation"] = new List<float>(){rotVec3.x,    rotVec3.y,  rotVec3.z};
			dict["scale"   ] = new List<float>(){scaleVec3.x,scaleVec3.y,scaleVec3.z};

			return dict;
		}

		// childrens array as dictionary array
		public List<Dictionary<string,object>> childrenProperties
		{
			get
			{
				List<Dictionary<string,object>> childrenArr = new List<Dictionary<string, object>>();

				for(int num = 0; num < children.Count; num++)
				{
					Dictionary<string,object> dict = children[num].objectProperties;
					if(dict!=null)
					{
						childrenArr.Add(dict);
					}
				}
				return childrenArr;
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

		public ObjectType objectType
		{
			get{return type;}
		}
	}
}