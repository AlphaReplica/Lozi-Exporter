using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UniThreeD;

namespace Lozi
{
	public class LoziExporter : MonoBehaviour
	{
		private static LoziExporter loziInstance;

		public GameObject sceneObject;
		public LoziObject target;
		public LoziMeshCollection meshCollection;
		public LoziAnimationCollection animationCollection;

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

		public void setGameObject(GameObject obj)
		{
			meshCollection 		= new LoziMeshCollection(obj);
			animationCollection = new LoziAnimationCollection(obj);
			target 		   		= new LoziObject(obj,((sceneObject==obj) ? "Scene" : ""));
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
				sceneObject = null;
				DestroyImmediate(sceneObject);
			}
		}

		public void parseObject(GameObject obj)
		{
			meshCollection = new LoziMeshCollection(obj);
		}

		public void export()
		{
			meshCollection.generateGeometry();
			Dictionary<string,object> mainDict   = new Dictionary<string, object>();
			Dictionary<string,object> assetsDict = new Dictionary<string, object>();
			
			assetsDict["meshes"    ] = LoziExporter.instance.meshCollection.meshesProperties;
			assetsDict["animations"] = LoziExporter.instance.animationCollection.animationsDictionary;
			
			mainDict  ["assets" ] = assetsDict;
			mainDict  ["objects"] = LoziExporter.instance.target.objectProperties;

			Debug.Log("aa");
			string str = MiniJSON.Json.Serialize(mainDict);
			Debug.Log("bb");
			//Debug.Log(str);
			
			StreamWriter sr = new StreamWriter("D:/Dropbox/Beka's DB/Other Projects/www/WebSocket/err2.js");
			sr.Write(str);
			sr.Close();
		}

		public static Dictionary<string,object> exportGameObjectToThreeJS(GameObject obj)
		{
			LoziExporter.instance.setGameObject(obj);
			LoziExporter.instance.parseObject(LoziExporter.instance.target.targetGameObject);
			//LoziObject            threeObj = new LoziObject(obj);
			//ThreeAssetsCollection assets   = new ThreeAssetsCollection(obj);

			Dictionary<string,object> mainDict   = new Dictionary<string, object>();
			Dictionary<string,object> assetsDict = new Dictionary<string, object>();
 
			assetsDict["meshes"    ] = LoziExporter.instance.meshCollection.meshesProperties;
			assetsDict["animations"] = LoziExporter.instance.animationCollection.animationsDictionary;
			 
			mainDict  ["assets" ] = assetsDict;
			mainDict  ["objects"] = LoziExporter.instance.target.objectProperties;

			return mainDict;
		}
	}
}

