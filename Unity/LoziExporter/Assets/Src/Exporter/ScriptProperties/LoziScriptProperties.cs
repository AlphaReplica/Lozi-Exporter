/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Light object
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziScriptProperties
	{
		Dictionary<string,Dictionary<string,object>> scriptObjects;

		private GameObject obj;
		public LoziScriptProperties(GameObject target)
		{
			obj = target;

		}

		public void generate()
		{
			MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
			
			scriptObjects = new Dictionary<string,Dictionary<string,object>>();
			
			foreach(MonoBehaviour script in scripts)
			{
				scriptObjects[script.GetType().ToString()] = new Dictionary<string,object>();
				
				System.Reflection.FieldInfo[] properties = script.GetType().GetFields();
				
				for(int num = 0; num < properties.Length; num++)
				{
					if(properties[num].IsPublic)
					{
						Type   type = properties[num].FieldType;
						var    val  = properties[num].GetValue(script);
						string name = properties[num].Name;
						
						if(isValidType(type))
						{
							if(val!=null && val.ToString()!="null" && val.ToString()!="Null")
							{
								scriptObjects[script.GetType().ToString()][name] = getValue(val,type);
							}
						}
					}
				}
			}
		}

		public object getValue(object val, Type type, bool addTypeLabel = true)
		{
			if(type.IsArray)
			{
				List<object> objects = new List<object>();
				foreach(object obj in val as Array)
				{
					var newObj = getValue(obj,type.GetElementType(),false);

					if(newObj!=null)
					{
						objects.Add(newObj);
					}
				}
				val = objects;
			}
			else
			{
				int newVal = getValueInstanceID(val as UnityEngine.Object);
				
				val = (newVal==0) ? val : newVal;
			}

			val = getValueAsDictionary(val,type);

			if(addTypeLabel)
			{
				int typeInt = typeStringToInt(getTypeString(type));

				Dictionary<string,object> dict = new Dictionary<string, object>();

				dict["type" ] = typeInt;
				dict["value"] = val;

				val = dict;
			}

			return val;
		}

		public object getValueAsDictionary(object value, Type type)
		{
			Dictionary<string,object> dict = new Dictionary<string, object>();

			if(type == typeof(Rect ))
			{
				dict["x"	 ] = ((Rect)value).x;
				dict["y"	 ] = ((Rect)value).y;
				dict["width" ] = ((Rect)value).width;
				dict["height"] = ((Rect)value).height;
			}
			if(type == typeof(Vector2))
			{
				dict["x"	 ] = ((Vector2)value).x;
				dict["y"	 ] = ((Vector2)value).y;
			}
			if(type == typeof(Vector3))
			{
				dict["x"	 ] = ((Vector3)value).x;
				dict["y"	 ] = ((Vector3)value).y;
				dict["z"	 ] = ((Vector3)value).z;
			}
			if(type == typeof(Vector4))
			{
				dict["x"	 ] = ((Vector4)value).x;
				dict["y"	 ] = ((Vector4)value).y;
				dict["z"	 ] = ((Vector4)value).z;
				dict["w"	 ] = ((Vector4)value).w;
			}

			if(dict.Count>0)
			{
				value = dict;
			}

			return value;
		}

		public int getValueInstanceID(UnityEngine.Object val)
		{
			if(val!=null)
			{
				if(val.GetType() == typeof(Texture2D  		   ) ||
				   val.GetType() == typeof(Cubemap  		   ) ||
				   val.GetType() == typeof(Material 		   ) ||
				   val.GetType() == typeof(Mesh		 		   ) ||
				   val.GetType() == typeof(SkinnedMeshRenderer ) ||
				   val.GetType() == typeof(MeshFilter 	 	   ) ||
				   val.GetType() == typeof(AnimationClip 	   ) ||
				   val.GetType() == typeof(Animation	 	   ) ||
				   val.GetType() == typeof(BoxCollider	 	   ) ||
				   val.GetType() == typeof(SphereCollider 	   ) ||
				   val.GetType() == typeof(CapsuleCollider 	   ) ||
				   val.GetType() == typeof(Collider		 	   ) ||
				   val.GetType() == typeof(GameObject	 	   ) ||
				   val.GetType() == typeof(AudioClip		   ) ||
				   val.GetType() == typeof(AudioSource		   ) ||
				   val.GetType() == typeof(Camera			   ) ||
				   val.GetType() == typeof(Light			   ) ||
				   val.GetType() == typeof(Transform		   ))
				{
					if(val.GetType() == typeof(Animation))
					{
						LoziAnimation anim = LoziExporter.instance.animationCollection.getAnimationByGameObject((val as Animation).gameObject);

						if(anim!=null)
						{
							return anim.id;
						}
						val.GetInstanceID();
					}
					if(val.GetType() == typeof(MeshFilter))
					{
						if((val as MeshFilter).sharedMesh!=null)
						{
							return (val as MeshFilter).sharedMesh.GetInstanceID();
						}
					}
					if(val.GetType() == typeof(SkinnedMeshRenderer))
					{
						if((val as SkinnedMeshRenderer).sharedMesh!=null)
						{
							return (val as SkinnedMeshRenderer).sharedMesh.GetInstanceID();
						}
					}
					if(val.GetType() == typeof(GameObject ))
					{
						return (val as GameObject).transform.GetInstanceID();
					}
					if(val.GetType() == typeof(Transform ))
					{
						return (val as Transform).GetInstanceID();
					}
					return val.GetInstanceID();
				}
			}

			return 0;
		}

		public bool isValidType(Type type)
		{
			if(type == typeof(Vector2		 	   ) || type == typeof(Vector2[]  	 		 ) ||
			   type == typeof(Vector3		 	   ) || type == typeof(Vector3[]		 	 ) ||
			   type == typeof(Vector4		 	   ) || type == typeof(Vector4[]		 	 ) ||
			   type == typeof(Int16  		 	   ) || type == typeof(Int16[]		 		 ) ||
			   type == typeof(Int32  		 	   ) || type == typeof(Int32[]		 		 ) ||
			   type == typeof(Int64  		 	   ) || type == typeof(Int64[]		 	 	 ) ||
			   type == typeof(Single 		 	   ) || type == typeof(Single[]		 		 ) ||
			   type == typeof(Double 		 	   ) || type == typeof(Double[]		 		 ) ||
			   type == typeof(Boolean		 	   ) || type == typeof(Boolean[]		 	 ) ||
			   type == typeof(string 		 	   ) || type == typeof(string[]		 		 ) ||
			   type == typeof(String 		 	   ) || type == typeof(String[]		 		 ) ||
			   type == typeof(Rect   		 	   ) || type == typeof(Rect[]  		 		 ) ||
			   type == typeof(Texture2D		       ) || type == typeof(Texture2D[]  	 	 ) ||
			   type == typeof(Cubemap   		   ) || type == typeof(Cubemap[]  	 		 ) ||
			   type == typeof(Material			   ) || type == typeof(Material[]		 	 ) ||
			   type == typeof(Mesh			 	   ) || type == typeof(Mesh[]			 	 ) ||
			   type == typeof(SkinnedMeshRenderer  ) || type == typeof(SkinnedMeshRenderer[] ) ||
			   type == typeof(MeshFilter	 	   ) || type == typeof(MeshFilter[]	 		 ) ||
			   type == typeof(Collider			   ) || type == typeof(Collider[] 			 ) ||
			   type == typeof(BoxCollider		   ) || type == typeof(BoxCollider[] 		 ) ||
			   type == typeof(SphereCollider	   ) || type == typeof(SphereCollider[] 	 ) ||
			   type == typeof(CapsuleCollider	   ) || type == typeof(CapsuleCollider[] 	 ) ||
			   type == typeof(AudioClip		  	   ) || type == typeof(AudioClip[]	 		 ) ||
			   type == typeof(AudioSource   	   ) || type == typeof(AudioSource[]	 	 ) ||
			   type == typeof(Animation		  	   ) || type == typeof(Animation[]	 		 ) ||
			   type == typeof(AnimationClip  	   ) || type == typeof(AnimationClip[] 		 ) ||
			   type == typeof(Light			  	   ) || type == typeof(Light[]		 		 ) ||
			   type == typeof(Camera		  	   ) || type == typeof(Camera[]		 		 ) ||
			   type == typeof(GameObject	 	   ) || type == typeof(GameObject[]	 		 ) ||
			   type == typeof(Transform 	 	   ) || type == typeof(Transform[]	 		 ))
			{
				return true;
			}
			return false;
		}

		public int typeStringToInt(string type)
		{
			int returnType = -1;

			switch(type)
			{
				case "float" 		  :{returnType =  0; break;}
				case "Int"   		  :{returnType =  1; break;}
				case "Boolean" 		  :{returnType =  2; break;}
				case "String" 		  :{returnType =  3; break;}
				case "Rect" 		  :{returnType =  4; break;}
				case "Vector2" 		  :{returnType =  5; break;}
				case "Vector3"  	  :{returnType =  6; break;}
				case "Vector4"  	  :{returnType =  7; break;}
				case "Material"  	  :{returnType =  8; break;}
				case "Texture2D" 	  :{returnType =  9; break;}
				case "Cubemap"    	  :{returnType = 10; break;}
				case "Mesh"   	  	  :{returnType = 11; break;}
				case "AudioClip"  	  :{returnType = 12; break;}
				case "AudioSource"    :{returnType = 13; break;}
				case "Animation"  	  :{returnType = 14; break;}
				case "AnimationClip"  :{returnType = 15; break;}
				case "Light"  		  :{returnType = 16; break;}
				case "Camera"  		  :{returnType = 17; break;}
				case "Collider"  	  :{returnType = 18; break;}
				case "Object"     	  :{returnType = 19; break;}
				
				case "float[]" 	  	  :{returnType = 20; break;}
				case "Int[]"   	  	  :{returnType = 21; break;}
				case "Boolean[]"  	  :{returnType = 22; break;}
				case "String[]"   	  :{returnType = 23; break;}
				case "Rect[]" 	  	  :{returnType = 24; break;}
				case "Vector2[]"  	  :{returnType = 25; break;}
				case "Vector3[]"  	  :{returnType = 26; break;}
				case "Vector4[]"  	  :{returnType = 27; break;}
				case "Material[]" 	  :{returnType = 28; break;}
				case "Texture2D[]"	  :{returnType = 29; break;}
				case "Cubemap[]"  	  :{returnType = 30; break;}
				case "Mesh[]" 	  	  :{returnType = 31; break;}
				case "AudioClip[]"	  :{returnType = 32; break;}
				case "AudioSource[]"  :{returnType = 33; break;}
				case "Animation[]"	  :{returnType = 34; break;}
				case "AnimationClip[]":{returnType = 35; break;}
				case "Light[]"		  :{returnType = 36; break;}
				case "Camera[]"		  :{returnType = 37; break;}
				case "Collider[]"  	  :{returnType = 38; break;}
				case "Object[]"   	  :{returnType = 39; break;}
			}
			return returnType;
		}
		
		public string getTypeString(Type type)
		{
			string str = type.ToString().Replace("System","")
									    .Replace("UnityEngine","")
									    .Replace(".","")
									    .Replace("16","")
									    .Replace("32","")
									    .Replace("64","")
									    .Replace("Single","float")
										.Replace("Double","float")
										.Replace("MeshFilter","Mesh")
										.Replace("SkinnedMeshRenderer","Mesh")
										.Replace("BoxCollider","Collider")
										.Replace("SphereCollider","Collider")
										.Replace("CapsuleCollider","Collider")
									    .Replace("GameObject","Object")
									    .Replace("Transform","Object");
			return str;
		}

		// current object properties as dictionary
		public Dictionary<string,Dictionary<string,object>> objectProperties
		{
			get
			{
				return scriptObjects;
			}
		}
		
		public static bool hasScript(GameObject obj)
		{
			if(obj.GetComponent<MonoBehaviour>()!=null)
			{
				return true;
			}
			return false;
		}
	}
}