/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * main class, singleton created which sets gameobject or wraps all gameobjects in scene gameobjects and exports as scene
 * when export called json generated from target gameobject and saved to path provided by user
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lozi
{
	public class LoziExporter : MonoBehaviour
	{
		private static LoziExporter loziInstance;
		
		public  bool				   exportSounds		= true;
		public  bool				   exportAnimations = true;
		public  bool				   exportMeshes     = true;
		public  bool				   exportMaterials  = true;
		public  bool				   exportTextures   = true;
		public  bool				   exportHierarchy  = true;
		private bool                   includeColliders = false;
		private bool                   includeScripts   = false;

		public string	  			   pathToSave;
		public GameObject 			   sceneObject;
		public LoziObject 			   target;
		public LoziMeshCollection 	   meshCollection;
		public LoziTextureCollection   textureCollection;
		public LoziMaterialCollection  materialCollection;
		public LoziAnimationCollection animationCollection;
		public LoziSoundCollection	   soundCollection;

		public static string Version
		{
			get
			{
				return "0.95";
			}
		}

		public static LoziExporter instance
		{
			get
			{
				if(loziInstance==null)
				{
					loziInstance = FindObjectOfType<LoziExporter>();

					if(loziInstance==null)
					{
						GameObject instanceObj = new GameObject("Lozi");
						instanceObj.hideFlags  = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
						loziInstance = instanceObj.AddComponent<LoziExporter>();
					}
				}
				return loziInstance;
			}
		}
		
		public bool exportColliders
		{
			get
			{
				return includeColliders;
			}
			set
			{
				includeColliders = value;
				target.exportColliders = value;
				meshCollection.exportColliders = value;
			}
		}
		
		public bool exportScripts
		{
			get
			{
				return includeScripts;
			}
			set
			{
				includeScripts = value;
				target.exportScriptProperties = value;
				meshCollection.exportScriptProperties = value;
			}
		}

		public void setGameObject(GameObject obj)
		{
			meshCollection 		= new LoziMeshCollection(obj);
			textureCollection   = new LoziTextureCollection(obj);
			materialCollection  = new LoziMaterialCollection(obj);
			animationCollection = new LoziAnimationCollection(obj);
			soundCollection     = new LoziSoundCollection(obj);
			target 		   		= new LoziObject(obj,((sceneObject==obj) ? ObjectType.Scene : ObjectType.None));
			target.generateScripts();
			meshCollection.generateScriptProperties();
		}

		public void setSceneObject()
		{
			sceneObject = new GameObject("Scene");

			Transform[] objects = GameObject.FindObjectsOfType<Transform>();

			for(int num = 0; num < objects.Length; num++)
			{
				if(objects[num].gameObject!=this.gameObject)
				{
					if(objects[num].parent==null)
					{
						objects[num].parent = sceneObject.transform;
					}
				}
			}
		}

		public void reset()
		{
			if(sceneObject!=null)
			{
				Transform[] objects = GameObject.FindObjectsOfType<Transform>();
				
				for(int num = 0; num < objects.Length; num++)
				{
					if(objects[num].parent==sceneObject.transform)
					{
						objects[num].parent = null;
					}
				}
				if(meshCollection!=null)
				{
					meshCollection.Dispose();
				}
				if(textureCollection!=null)
				{
					textureCollection.Dispose();
				}
				if(materialCollection!=null)
				{
					materialCollection.Dispose();
				}
				if(animationCollection!=null)
				{
					animationCollection.Dispose();
				}
				if(soundCollection!=null)
				{
					soundCollection.Dispose();
				}
				DestroyImmediate(sceneObject);
				sceneObject = null;
			}
		}

		public void export()
		{
			generateJSON();
		}

		private void generateJSON()
		{
			if(exportMeshes)
			{
				meshCollection.generate();
			}
			if(exportMaterials)
			{
				materialCollection.generate();
			}
			if(exportTextures)
			{
				string path = "";
				if(!textureCollection.includeTexturesInFile)
				{
					path = createDirectory("textures");
				}
				textureCollection.generate(path);
			}
			if(exportSounds)
			{
				string path = "";
				if(!soundCollection.includeSoundsInFile)
				{
					path = createDirectory("sounds");
				}
				soundCollection.generate(path);
			}
			if(exportAnimations)
			{
				animationCollection.generate();
			}
			
			Dictionary<string,object> mainDict   = new Dictionary<string, object>();
			Dictionary<string,object> assetsDict = new Dictionary<string, object>();
			
			if(exportMeshes)
			{
				assetsDict["meshes"] = meshCollection.meshesProperties;
				assetsDict["bones" ] = meshCollection.boneProperties;
			}
			if(exportTextures)
			{
				assetsDict["textures"] = textureCollection.textureProperties;
			}
			if(exportMaterials)
			{
				assetsDict["materials" ] = materialCollection.materialProperties;
			}
			if(exportAnimations)
			{
				assetsDict["animations"] = animationCollection.animationsDictionary;
			}
			if(exportSounds)
			{
				assetsDict["sounds"] = soundCollection.soundProperties;
			}
			
			mainDict  ["assets"] = assetsDict;
			if(exportHierarchy)
			{
				mainDict  ["objects"] = target.objectProperties;
			}
			
			string str = MiniJSON.Json.Serialize(mainDict);
			
			StreamWriter sr = new StreamWriter(LoziExporter.instance.pathToSave);
			sr.Write(str);
			sr.Close();
		}

		private string createDirectory(string sub)
		{
			string str = "";
			string[] arr  = pathToSave.Split('/');
			
			for(int num = 0; num < arr.Length-1; num++)
			{
				str+=arr[num]+"/";
			}
			str += target.name+"_data";

			if (!Directory.Exists(str)) 
			{
				Directory.CreateDirectory(str);
			}
			if(!Directory.Exists(str+"/"+sub))
			{
				Directory.CreateDirectory(str+"/"+sub);
			}
			return str+"/"+sub;
		}
	}
}

