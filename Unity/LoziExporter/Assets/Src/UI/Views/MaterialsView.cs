/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * View for Materials, shows material name and draws material exiting properties
 * provides option to choose material type provided by webGL engine
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Lozi.helpers;
using Lozi;

namespace Lozi.UI
{
	public class MaterialsView
	{
		private string[]     		 materialTypesOpt = new string[] {"Standard", "Lit", "Phong", "Lambert","By Selection"};
		private string[]     		 materialTypes    = new string[] {"Standard", "Lit", "Phong", "Lambert"};
		private string[]     		 materialSide     = new string[] {"Front"   , "Back", "Double"};
		private Texture2D[]  		 icons;
		private LoziMaterial 		 tempMat;
		private LoziMaterialProperty matProperties;
		private int			 		 materialTypesIndex = 1;
		private int			 		 oldMaterialTypesIndex;
		private bool 		 		 unfolded;

		public MaterialsView(Texture2D[] viewIcons)
		{
			icons = viewIcons;
		}

		public bool fold
		{
			get
			{
				return unfolded;
			}
			set
			{
				unfolded = value;
			}
		}

		// Draws Header vith view name and fold/unfold button
		// Also provides dropdown to select material type globally
		private void drawHeader()
		{
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			GUILayout.Label ("Materials", EditorStyles.boldLabel);

			if(LoziExporter.instance.materialCollection.materials.Count>1)
			{
				if(GUILayout.Button((unfolded)? "Close All" : "Expand All"))
				{
					unfolded = !unfolded;
					foldObjects(unfolded);
				}
			}

			EditorGUILayout.EndHorizontal();
			
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);

			LoziExporter.instance.exportMaterials = GUILayout.Toggle(LoziExporter.instance.exportMaterials,"Export");

			if(LoziExporter.instance.materialCollection.materials.Count>0)
			{
				materialTypesIndex = EditorGUILayout.Popup("Materials Type", materialTypesIndex, materialTypesOpt);

				if(materialTypesIndex!=oldMaterialTypesIndex)
				{
					if(materialTypesIndex<4)
					{
						for(int num = 0; num < LoziExporter.instance.materialCollection.materials.Count; num++)
						{
							LoziExporter.instance.materialCollection.materials[num].materialType = materialTypesIndex;
						}
					}
					oldMaterialTypesIndex = materialTypesIndex;
				}
			}

			EditorGUILayout.EndHorizontal();
		}

		// Folds/Unfolds all objects in view
		private void foldObjects(bool fold)
		{
			for(int num = 0; num < LoziExporter.instance.materialCollection.materials.Count; num++)
			{
				LoziExporter.instance.materialCollection.materials[num].isFoldedInUI = fold;
			}
		}

		// Converts property to string, vec(0,0) to [0,0]
		private string propertyToString(object obj)
		{
			if(obj is float)
			{
				return obj.ToString();
			}
			if(obj is int)
			{
				return obj.ToString();
			}
			if(obj is List<float>)
			{
				string arr = "";
				for(int num = 0; num < (obj as List<float>).Count; num++)
				{
					arr+=(obj as List<float>)[num]+",";
				}
				return arr.Substring(0,arr.Length-1);
			}
			return "";
		}

		// Draws materials if exists
		public bool drawMaterials()
		{
			GUI.skin.box.margin =  new RectOffset(3,2,0,0);
			
			if(LoziExporter.instance.materialCollection!=null &&
			   LoziExporter.instance.materialCollection.materials.Count>0)
			{
				EditorGUILayout.BeginVertical("Box");

				drawHeader();

				for(int num = 0; num < LoziExporter.instance.materialCollection.materials.Count; num++)
				{
					tempMat = LoziExporter.instance.materialCollection.materials[num];
					GUI.skin.box.margin  = new RectOffset(0,0,0,0);
					if (GUI.Button(EditorGUILayout.BeginHorizontal("Box"), GUIContent.none))
					{
						tempMat.isFoldedInUI = !tempMat.isFoldedInUI;
					}
					GUILayout.Label(icons[8]);

					GUILayout.Label(tempMat.name); 
					EditorGUILayout.EndHorizontal();

					if(materialTypesIndex!=4)
					{
						tempMat.materialType = materialTypesIndex;
					}

					if(tempMat.isFoldedInUI)
					{
						GUI.Box(EditorGUILayout.BeginVertical("Box"), GUIContent.none);
						GUILayout.Space(5);
						tempMat.materialType  = EditorGUILayout.Popup("Material Type"      , tempMat.materialType, materialTypes);
						tempMat.materialSide  = EditorGUILayout.Popup("Material Side"	   , tempMat.materialSide, materialSide);
						tempMat.transparentID = EditorGUILayout.Popup("Transparent Texture", tempMat.transparentID, tempMat.textureProperties);

						if(materialTypesIndex!=4 && tempMat.materialType!=materialTypesIndex)
						{
							materialTypesIndex = 4;
						}
						GUILayout.Label ("Properties", EditorStyles.boldLabel);
						for(int num2 = 0; num2 < tempMat.materialProps.Count; num2++)
						{
							matProperties = tempMat.materialProps[num2];
							EditorGUILayout.TextField(matProperties.propertyName,propertyToString(matProperties.valObject));
						}
						GUILayout.Space(10);
						EditorGUILayout.EndVertical();
					}
				}
				
				EditorGUILayout.EndVertical();
				return true;
			}
			return false;
		}
	}
}
