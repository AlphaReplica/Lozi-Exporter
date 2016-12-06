/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Materials collection, stores all materials of object and its children
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziMaterialCollection : IDisposable
	{
		private List<LoziMaterial> materialObjects;

		public LoziMaterialCollection(GameObject obj)
		{
			materialObjects = new List<LoziMaterial>();

			foreach(Transform transform in obj.GetComponentsInChildren<Transform>()) 
			{
				Material[] materials = LoziMaterial.getMaterials(transform.gameObject);
				if(materials!=null && materials.Length>0)
				{
					for(int num = 0; num < materials.Length; num++)
					{
						if(materials[num]!=null && canAddInArray(materials[num]))
						{
							int lightMapID  = -1;
							LoziTexture tex = LoziExporter.instance.textureCollection.getLightMapByGameObject(transform.gameObject);
							
							if(tex!=null)
							{
								lightMapID = tex.id;
							}
							materialObjects.Add(new LoziMaterial(transform.gameObject,materials[num],lightMapID));
						}
					}
				}
			}
		}

		public void generate()
		{
			foreach(LoziMaterial material in materialObjects) 
			{
				material.generate(material.material);
			}
		}

		private bool canAddInArray(Material material)
		{
			foreach(LoziMaterial loziMaterial in materialObjects)
			{
				if(loziMaterial.isSameMaterial(material))
				{
					return false;
				}
			}
			return true;
		}

		public List<LoziMaterial> materials
		{
			get{return materialObjects;}
		}

		public List<Dictionary<string,object>> materialProperties
		{
			get
			{
				List<Dictionary<string,object>> arr = new List<Dictionary<string, object>>();

				for(int num = 0; num < materialObjects.Count; num++)
				{
					arr.Add(materialObjects[num].materialProperties);
				}

				return arr;
			}
		}
		
		public LoziMaterial getMaterialByMaterial(Material material)
		{
			foreach(LoziMaterial matObj in materialObjects)
			{
				if(matObj.isSameMaterial(material))
				{
					return matObj;
				}
			}
			return null;
		}
		
		public int getMaterialIdByMaterial(Material material)
		{
			LoziMaterial materialObj = getMaterialByMaterial(material);

			if(materialObj!=null)
			{
				return materialObj.id;
			}
			return 0;
		}
		
		public LoziMaterial getMaterialByGameObject(GameObject obj)
		{
			return getMaterialByMaterial(LoziMaterial.getMaterial(obj));
		}
		
		public List<LoziMaterial> getMaterialsByGameObject(GameObject obj)
		{
			Renderer rend = LoziMesh.getRenderer(obj);
			
			List<LoziMaterial> returnMaterials = new List<LoziMaterial>();
			if(rend)
			{
				for(int num = 0; num < rend.sharedMaterials.Length; num++)
				{
					LoziMaterial mat = getMaterialByMaterial(rend.sharedMaterials[num]);

					if(mat!=null)
					{
						returnMaterials.Add(mat);
					}
				}
			}
			return returnMaterials;
		}
		
		public List<int> getMaterialIds(List<LoziMaterial> materials)
		{
			List<int> ids = new List<int>();
			for(int num = 0; num < materials.Count; num++)
			{
				ids.Add(materials[num].id);
			}
			return ids;
		}

		public void Dispose()
		{ 
			for(int num = 0; num < materialObjects.Count; num++)
			{
				materialObjects[num].Dispose();
			}
			materialObjects.Clear();
			materialObjects = null;
		}
	}
}
