/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Material Object, represents Unity material with properties
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Lozi.helpers;

namespace Lozi
{
	public class LoziMaterial : IDisposable
	{
		public  bool 		  			 isFoldedInUI;
		public  int           			 materialType;
		public  int     		   		transparentID;
		private int	        	 			 objectId;
		private int	        			   lightmapId;
		private Material		 			 material;
		private GameObject  		   			  obj;
		private string			   			  objName;
		public  string[] 			textureProperties;
		private List<LoziMaterialProperty> properties;

		public LoziMaterial(GameObject target,int lightMapID)
		{
			transparentID	   = 0;
			textureProperties  = new string[]{"None"};
			lightmapId 		   = lightMapID;
			obj 	   		   = target;
			generate();
		}

		// Generates data from unity material
		public void generate()
		{
			material = LoziMaterial.getMaterial(obj);
			objName  = material.name;
			objectId = material.GetInstanceID();
			getProperties();
		}

		// gets all properties of material shader and adds lightmap property if lightmapid >-1
		private void getProperties()
		{
			if(properties!=null)
			{
				if(properties.Count>0)
				{
					for(int num = 0; num < properties.Count; num++)
					{
						properties[num].Dispose();
					}
					properties.Clear();
				}
			}
			List<string> textureProps = new List<string>();
			properties 		  		  = new List<LoziMaterialProperty>();

			for (int num = 0; num < ShaderUtil.GetPropertyCount(material.shader) ; num++)
			{
				ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(material.shader,num);
				string property 				   = ShaderUtil.GetPropertyName(material.shader,num);
				object value                       = null;
				switch(type)
				{
					case ShaderUtil.ShaderPropertyType.Range :
					case ShaderUtil.ShaderPropertyType.Float :{value = 				material.GetFloat  (property);  break;}
					case ShaderUtil.ShaderPropertyType.Color :{value = colorToList (material.GetColor  (property)); break;}
					case ShaderUtil.ShaderPropertyType.Vector:{value = vectorToList(material.GetVector (property)); break;}
					case ShaderUtil.ShaderPropertyType.TexEnv:{value = 				material.GetTexture(property);  break;}
				}

				if(value!=null)
				{
					if(type == ShaderUtil.ShaderPropertyType.TexEnv)
					{
						textureProps.Add(property);
						value = (value as Texture).GetInstanceID();
					}
					properties.Add(new LoziMaterialProperty(type,property,value));
				}
			}
			if(textureProps.Count>0)
			{
				textureProperties    = new string[textureProps.Count+1];
				textureProperties[0] = "None";
				 
				for(int num = 0; num < textureProperties.Length-1; num++)
				{
					textureProperties[num+1] = textureProps[num];
				}
			}
			if(lightmapId>-1)
			{
				properties.Add(new LoziMaterialProperty(ShaderUtil.ShaderPropertyType.TexEnv,"lightMap",lightmapId));
			}
			if(transparentID>0)
			{
				string propName 		  = textureProperties[transparentID];
				LoziMaterialProperty prop = getPropertyByString(propName);

				if(prop!=null)
				{
					properties.Add(new LoziMaterialProperty(ShaderUtil.ShaderPropertyType.TexEnv,"transparentMap",prop.valObject));
				}
			}
		}

		private LoziMaterialProperty getPropertyByString(string str)
		{
			for(int num = 0; num < properties.Count; num++)
			{
				if(properties[num].propertyName == str)
				{
					return properties[num];
				}
			}
			return null;
		}

		private List<float> colorToList(Color col)
		{
			return new List<float>(){col.r,col.g,col.b,col.a};
		}
		
		private List<float> vectorToList(Vector4 vec)
		{
			return new List<float>(){vec.x,vec.y,vec.z,vec.w};
		}

		//returns generated data as dictionary
		public Dictionary<string,object> materialProperties
		{
			get
			{
				if(material!=null)
				{
					Dictionary<string,object> propertiesList = new Dictionary<string, object>();

					for(int num = 0; num < properties.Count; num++)
					{
						propertiesList[properties[num].propertyName] = properties[num].valObject;
					}

					Dictionary<string,object> materialDict  = new Dictionary<string, object>();
					materialDict["id"        ] = objectId;
					materialDict["name"      ] = material.name;
					materialDict["type"      ] = materialType;
					materialDict["properties"] = propertiesList;

					return materialDict;
				}
				else
				{
					return null;
				}
			}
		}
		
		public int id
		{
			get
			{
				return objectId;
			}
		}
		
		public string name
		{
			get
			{
				return objName;
			}
		}
		
		public List<LoziMaterialProperty> materialProps
		{
			get
			{
				return properties;
			}
		}

		public bool isSameMaterial(Material materialObj)
		{
			if(material==materialObj)
			{
				return true;
			}
			return false;
		}

		public void Dispose()
		{
			obj      = null;
			material = null;
			objName  = null;
		}

		// gets material from gameobject
		public static Material getMaterial(GameObject gameObj)
		{
			if(gameObj.GetComponent<SkinnedMeshRenderer>()!=null && 
			   gameObj.GetComponent<SkinnedMeshRenderer>().sharedMaterial!=null)
			{
				return gameObj.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
			}
			if(gameObj.GetComponent<Renderer>()!=null && 
			   gameObj.GetComponent<Renderer>().sharedMaterial!=null)
			{
				return gameObj.GetComponent<Renderer>().sharedMaterial;
			}
			return null;
		}

		public static bool hasMaterial(GameObject gameObj)
		{
			return (getMaterial(gameObj)==null) ? false : true;
		}
	}
}
