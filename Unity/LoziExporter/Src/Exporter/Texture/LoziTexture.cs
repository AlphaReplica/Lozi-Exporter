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
		public  bool          reverse;
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
			
			if(directory.Length>=2)
			{
				directory    = arr[arr.Length-2]+"/"+arr[arr.Length-1];
			}

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
		
		private Texture2D createTexture(int width, int height, Color[] pixels, bool isNormal = false)
		{
			Texture2D tex = new Texture2D(width,height);

			tex.SetPixels(pixels);


			if(isNormal)
			{
				for (int y=0; y < tex.height; y++)
				{
					for (int x=0; x < tex.width; x++)
					{
						
						float xLeft  = tex.GetPixel(x-1,y).grayscale;
						float xRight = tex.GetPixel(x+1,y).grayscale;
						float yUp    = tex.GetPixel(x,y-1).grayscale;
						float yDown  = tex.GetPixel(x,y+1).grayscale;
						float xDelta = ((xLeft-xRight)+1)*0.5f;
						float yDelta = ((yUp-yDown)+1)*0.5f;
						
						tex.SetPixel(x,y,new Color(xDelta,yDelta,1.0f,1.0f));
					}
				}
			}

			return tex;
		}
		
		private Texture2D createTexture(int width, int height, Color32[] pixels)
		{
			Texture2D tex = new Texture2D(width,height);
			tex.SetPixels32(pixels);
			
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

		private bool makeTexturesReadable(Texture texture)
		{
			string 			assetPath = AssetDatabase.GetAssetPath(texture);
			TextureImporter importer  = AssetImporter.GetAtPath(assetPath) as TextureImporter;

			if (importer!=null)
			{
				if(!importer.isReadable)
				{
					importer.textureType = TextureImporterType.Advanced;
					importer.isReadable  = true;
					
					AssetDatabase.ImportAsset(assetPath);
					AssetDatabase.Refresh();
				}

				return importer.normalmap;
			}
			return false;
		}

		private Texture2D getProceduralTexture(ProceduralTexture texture)
		{
			string 			  assetPath = AssetDatabase.GetAssetPath(texture);
			SubstanceImporter importer  = AssetImporter.GetAtPath(assetPath) as SubstanceImporter;

			if (importer!=null)
			{
				ProceduralMaterial[] materials = importer.GetMaterials();

				for(int num1 = 0; num1 < materials.Length; num1++)
				{
					ProceduralMaterial material = materials[num1];

					int width  = 0;
					int height = 0;
					int format = 0;
					int load   = 0;
					
					importer.GetPlatformTextureSettings(material.name,"", out width,out height,out format, out load);

					importer.SetPlatformTextureSettings(material,"",width,height,1,3);
					importer.SetGenerateAllOutputs(material,true);
					importer.SaveAndReimport();
					material.RebuildTextures();

					material.isReadable = true;
				}

				Texture2D tex = createTexture(texture.width,
				                              texture.height,
				                              texture.GetPixels32(0,0,texture.width,texture.height));
				
				tex.name = texture.name;

				return tex;
			}
			return null;
		}

		private void setTextureData()
		{
			bool isNormal = makeTexturesReadable(obj);

			if(obj is Cubemap)
			{
				Cubemap cubemap = obj as Cubemap;

				try
				{
					Color[] right = cubemap.GetPixels(CubemapFace.PositiveX);
					Color[] left  = cubemap.GetPixels(CubemapFace.NegativeX);
					Color[] down  = cubemap.GetPixels(CubemapFace.NegativeY);
					Color[] up    = cubemap.GetPixels(CubemapFace.PositiveY);
					Color[] front = cubemap.GetPixels(CubemapFace.PositiveZ);
					Color[] back  = cubemap.GetPixels(CubemapFace.NegativeZ);

					if(!reverse)
					{
						Array.Reverse(right);
						Array.Reverse(left );
						Array.Reverse(front);
						Array.Reverse(back );

						Color[] temp = up;
						up   = down;
						down = temp;
					}

					textureData.Add(imageData(createTexture(obj.width,obj.height,right),obj.name+"_right"  ));
					textureData.Add(imageData(createTexture(obj.width,obj.height,left ),obj.name+"_left"   ));
					textureData.Add(imageData(createTexture(obj.width,obj.height,down ),obj.name+"_down"   ));
					textureData.Add(imageData(createTexture(obj.width,obj.height,up   ),obj.name+"_up"     ));
					textureData.Add(imageData(createTexture(obj.width,obj.height,front),obj.name+"_forward"));
					textureData.Add(imageData(createTexture(obj.width,obj.height,back ),obj.name+"_back"   ));
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
					textureData.Add(imageData(createTexture(obj.width,obj.height,(obj as Texture2D).GetPixels(),isNormal),obj.name));
				}
				catch(Exception e)
				{

					Debug.LogError("IS "+AssetDatabase.GetAssetPath(obj)+" Sprite? "+e.Message);
				}
			}
			if(obj is ProceduralTexture)
			{
				Texture2D tex = getProceduralTexture(obj as ProceduralTexture);
				if(tex!=null)
				{
					textureData.Add(imageData(tex,tex.name));
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
