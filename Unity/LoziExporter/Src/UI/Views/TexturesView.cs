/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * View for Textures, includes baked lightmaps
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using Lozi.helpers;
using Lozi;

namespace Lozi.UI
{
	public class TexturesView
	{
		private Texture2D[] icons;
		private LoziTexture tempTex;

		private bool 		 unfolded;

		public TexturesView(Texture2D[] viewIcons)
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

		private void drawHeader()
		{
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			GUILayout.Label ("Textures", EditorStyles.boldLabel);

			if(LoziExporter.instance.textureCollection.textures.Count>1)
			{
				if(GUILayout.Button((unfolded)? "Close All" : "Expand All"))
				{
					unfolded = !unfolded;
					foldObjects(unfolded);
				}
			}

			EditorGUILayout.EndHorizontal();
			
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			LoziExporter.instance.exportTextures = GUILayout.Toggle(LoziExporter.instance.exportTextures,"Export");
			LoziExporter.instance.textureCollection.includeTexturesInFile = GUILayout.Toggle(LoziExporter.instance.textureCollection.includeTexturesInFile,"Include in Lozi");
			EditorGUILayout.EndHorizontal();
		}

		private void foldObjects(bool fold)
		{
			for(int num = 0; num < LoziExporter.instance.textureCollection.textures.Count; num++)
			{
				LoziExporter.instance.textureCollection.textures[num].isFoldedInUI = fold;
			}
		}

		public bool drawTextures()
		{
			GUI.skin.box.margin =  new RectOffset(10,10,0,0);
			if(LoziExporter.instance.textureCollection!=null &&
			   LoziExporter.instance.textureCollection.textures.Count>0)
			{
				EditorGUILayout.BeginVertical("Box");

				drawHeader();
				
				for(int num = 0; num < LoziExporter.instance.textureCollection.textures.Count; num++)
				{
					tempTex = LoziExporter.instance.textureCollection.textures[num];
					GUI.skin.box.margin  = new RectOffset(0,0,0,0);
					if (GUI.Button(EditorGUILayout.BeginHorizontal("Box"), GUIContent.none))
					{
						tempTex.isFoldedInUI = !tempTex.isFoldedInUI;
					}

					if(tempTex.texture is Cubemap)
					{
						GUILayout.Label(icons[10]);
					}
					else
					{
						GUILayout.Label(icons[9]);
					}
					GUILayout.Label(tempTex.name); 
					EditorGUILayout.EndHorizontal();

					if(tempTex.isFoldedInUI)
					{ 
						GUI.Box(EditorGUILayout.BeginVertical("Box"), GUIContent.none);
						GUILayout.Label ("Advanced", EditorStyles.boldLabel);
						EditorGUILayout.BeginHorizontal();
						if(tempTex.texture is Texture2D)
						{
							GUILayout.Label (tempTex.texture,GUILayout.Width(100),GUILayout.Height(100));
						}
						EditorGUILayout.BeginVertical();
						if(tempTex.texture is Cubemap)
						{
							GUILayout.Label ("TYPE: Cubemap");
						}
						else
						{
							GUILayout.Label ("TYPE: Texture");
						}
						GUILayout.Label ("RESOLUTION: "+tempTex.texture.width.ToString()+"X"+tempTex.texture.height.ToString());
						GUILayout.Label ("ID: "+tempTex.id.ToString());
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndHorizontal();
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
