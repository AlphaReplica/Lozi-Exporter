/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Texture object, generates base64 or exports to given path
 * Currently supports only Texture2D and Cubemap with 6 pieces
 */
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziTexture : IDisposable
	{
		public  bool 		  isFoldedInUI;
		public  bool          includeInData = true;
		private bool          islightMap;
		private int	          objectId;
		private Texture  	  obj;
		private string		  objName;
		private string		  pathToExport;
		private string		  directory;
		public List<string>   textureData;

		public LoziTexture(Texture tex,bool isLightmap)
		{
			islightMap = isLightmap;
			obj 	   = tex;
			objName    = obj.name;
			objectId   = obj.GetInstanceID();
		}

		public void generate(string path)
		{
			pathToExport = path+"/";
			string[] arr = path.Split('/');
			directory    = arr[arr.Length-1];

			clear();
			textureData = new List<string>();
			makeReadable();
			setTextureData();
		}

		private void makeReadable()
		{
			string assetPath = AssetDatabase.GetAssetPath(obj);
			TextureImporter importer  = AssetImporter.GetAtPath(assetPath) as TextureImporter;

			if (importer != null)
			{
				importer.isReadable = true;

				AssetDatabase.ImportAsset(assetPath);
				AssetDatabase.Refresh();
			}
		}

		private Texture2D createTexture(int width, int height, Color[] pixels)
		{
			Texture2D tex = new Texture2D(width,height);
			tex.SetPixels(pixels);

			return tex;
		}

		private string imageData(Texture2D tex,string texName)
		{
			if(includeInData)
			{
				return "data:image/png;base64,"+Convert.ToBase64String(tex.EncodeToPNG());
			}
			else
			{
				SaveTextureToFile(tex.EncodeToPNG(),texName+".png");
				return directory+"/"+texName+".png";
			}
		}

		private void SaveTextureToFile(byte[] bytes,string fileName)
		{
			BinaryWriter binary = new BinaryWriter(File.Open(pathToExport+fileName,FileMode.Create));
			binary.Write(bytes);
		}

		private void setTextureData()
		{
			if(obj is Cubemap)
			{
				Cubemap cubemap = obj as Cubemap;

				try
				{
					textureData.Add(imageData(createTexture(obj.width,obj.height,cubemap.GetPixels(CubemapFace.PositiveX)),obj.name+"_right"));
					textureData.Add(imageData(createTexture(obj.width,obj.height,cubemap.GetPixels(CubemapFace.NegativeX)),obj.name+"_left"));
					textureData.Add(imageData(createTexture(obj.width,obj.height,cubemap.GetPixels(CubemapFace.NegativeY)),obj.name+"_down"));
					textureData.Add(imageData(createTexture(obj.width,obj.height,cubemap.GetPixels(CubemapFace.PositiveY)),obj.name+"_up"));
					textureData.Add(imageData(createTexture(obj.width,obj.height,cubemap.GetPixels(CubemapFace.PositiveZ)),obj.name+"_forward"));
					textureData.Add(imageData(createTexture(obj.width,obj.height,cubemap.GetPixels(CubemapFace.NegativeZ)),obj.name+"_back"));
				}
				catch(Exception e)
				{
					Debug.LogError("IS "+AssetDatabase.GetAssetPath(obj)+" Cubemap set to readable? "+e.Message);
				}

			}
			if(obj is Texture2D)
			{
				try
				{
					textureData.Add(imageData(createTexture(obj.width,obj.height,(obj as Texture2D).GetPixels()),obj.name));
				}
				catch(Exception e)
				{

					Debug.LogError("IS "+AssetDatabase.GetAssetPath(obj)+" Sprite? "+e.Message);
				}
			}
		}

		public bool areSame(Texture tex)
		{
			if(obj == tex)
			{
				return true;
			}
			return false;
		}

		public Dictionary<string,object> textureProperties
		{
			get
			{
				if(obj!=null)
				{
					Dictionary<string,object> dict = new Dictionary<string, object>();
					dict["id"    	  ] = objectId;
					dict["name"  	  ] = objName;
					dict["textureData"] = textureData;
					dict["width" 	  ] = obj.width;
					dict["height"	  ] = obj.height; 

					return dict;
				}
				return null;
			}
		}
		
		public bool lightmap
		{
			get
			{
				return islightMap;
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

		public Texture texture
		{
			get
			{
				return obj;
			}
		}

		private void clear()
		{
			if(textureData!=null)
			{
				textureData.Clear();
			}
			textureData = null;
		}

		public void Dispose()
		{
			clear();
			obj     = null;
			objName = null;
		}
	}
}
