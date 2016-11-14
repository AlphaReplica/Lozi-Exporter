/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Textures collection, stores all textures of object and its children
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziTextureCollection : IDisposable
	{
		private bool includeTextures = true;
		private List<LoziTexture> textureCollection;

		public LoziTextureCollection(GameObject obj)
		{
			textureCollection = new List<LoziTexture>();

			foreach(Transform transform in obj.GetComponentsInChildren<Transform>()) 
			{
				Material[] materials = LoziMaterial.getMaterials(transform.gameObject);

				if(materials!=null && materials.Length>0)
				{
					for(int num1 = 0; num1 < materials.Length; num1++)
					{
						List<Texture> texturesArr = getMaterialTexures(materials[num1]);
						
						for(int num2 = 0; num2 < texturesArr.Count; num2++)
						{
							if(texturesArr[num2]!=null && canAddInArray(texturesArr[num2]))
							{
								textureCollection.Add(new LoziTexture(texturesArr[num2],false));
							}
						}
					}
				}

				Texture lightmap = getLightMap(transform.gameObject);

				if(lightmap!=null)
				{
					textureCollection.Add(new LoziTexture(lightmap,true));
				}
			}
		}

		public void generate(string path)
		{
			foreach(LoziTexture tex in textureCollection) 
			{
				tex.generate(path);
			}
		}

		public LoziTexture getLightMapByGameObject(GameObject target)
		{
			Texture texture = getLightMap(target);

			if(texture!=null)
			{
				for(int num = 0; num < textureCollection.Count; num++)
				{
					if(textureCollection[num].areSame(texture))
					{
						return textureCollection[num];
					}
				}
			}
			return null;
		}

		public Texture getLightMap(GameObject target)
		{
			Renderer renderer = LoziMesh.getRenderer(target);
			if(renderer!=null && renderer.lightmapIndex>-1)
			{
				if(LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapFar!=null)
				{
					return LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapFar;
				}
			}
			return null;
		}

		private List<Texture> getMaterialTexures(Material material)
		{
			List<Texture> textures = new List<Texture>();
			for (int num = 0; num < ShaderUtil.GetPropertyCount(material.shader) ; num++)
			{
				string property 				   = ShaderUtil.GetPropertyName(material.shader,num);

				if(ShaderUtil.GetPropertyType(material.shader,num) == ShaderUtil.ShaderPropertyType.TexEnv)
				{
					if(material.GetTexture(property)!=null)
					{
						textures.Add(material.GetTexture(property));
					}
				}
			}
			return textures;
		}

		private bool canAddInArray(Texture texture)
		{
			foreach(LoziTexture tex in textureCollection)
			{
				if(tex.areSame(texture))
				{
					return false;
				}
			}
			return true;
		}

		public List<LoziTexture> textures
		{
			get{return textureCollection;}
		}

		public List<Dictionary<string,object>> textureProperties
		{
			get
			{
				List<Dictionary<string,object>> arr = new List<Dictionary<string, object>>();

				for(int num = 0; num < textureCollection.Count; num++)
				{
					arr.Add(textureCollection[num].textureProperties);
				}
				return arr;
			}
		}

		public bool includeTexturesInFile
		{
			get
			{
				return includeTextures;
			}
			set
			{
				includeTextures = value;
				foreach(LoziTexture tex in textureCollection)
				{
					tex.includeInData = includeTextures;
				}
			}
		}

		public void Dispose()
		{ 
			for(int num = 0; num < textureCollection.Count; num++)
			{
				textureCollection[num].Dispose();
			}
			textureCollection.Clear();
			textureCollection = null;
		}
	}
}
