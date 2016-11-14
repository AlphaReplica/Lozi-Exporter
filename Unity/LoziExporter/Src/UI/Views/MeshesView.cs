/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * View for Meshes, shows mesh name and provides advanced options in dropdown
 */


using UnityEngine;
using UnityEditor;
using System.Collections;
using Lozi;

namespace Lozi.UI
{
	public class MeshesView
	{
		private LoziMesh 	tempMesh;
		private Texture2D[] icons;
		private bool 		unfolded;

		public MeshesView(Texture2D[] viewIcons)
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

		private int getRootBoneIndex(int id, int[] bones)
		{
			for(int num = 0; num <bones.Length; num++)
			{
				if(bones[num] == id)
				{
					return num;
				}
			}
			return -1;
		}

		// Draws Headers with fold/unfold button and title
		private void drawHeader()
		{
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			GUILayout.Label ("Meshes", EditorStyles.boldLabel);

			if(LoziExporter.instance.meshCollection.meshes.Count>1)
			{
				if(GUILayout.Button((unfolded)? "Close All" : "Expand All"))
				{
					unfolded = !unfolded;
					foldObjects(unfolded);
				}
			}
			EditorGUILayout.EndHorizontal();
			
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			LoziExporter.instance.exportMeshes = GUILayout.Toggle(LoziExporter.instance.exportMeshes,"Export");
			EditorGUILayout.EndHorizontal();
		}

		// folds/unfolds items
		private void foldObjects(bool fold)
		{
			for(int num = 0; num < LoziExporter.instance.meshCollection.meshes.Count; num++)
			{
				LoziExporter.instance.meshCollection.meshes[num].isFoldedInUI = fold;
			}
		}
		
		public bool drawMeshes()
		{
			GUI.skin.box.margin =  new RectOffset(3,2,0,0);
			
			if(LoziExporter.instance.meshCollection!=null &&
			   LoziExporter.instance.meshCollection.meshes.Count>0)
			{
				EditorGUILayout.BeginVertical("Box");

				drawHeader();

				for(int num = 0; num < LoziExporter.instance.meshCollection.meshes.Count; num++)
				{
					tempMesh = LoziExporter.instance.meshCollection.meshes[num];
					GUI.skin.box.margin  = new RectOffset(0,0,0,0);
					if (GUI.Button(EditorGUILayout.BeginHorizontal("Box"), GUIContent.none))
					{
						tempMesh.isFoldedInUI = !tempMesh.isFoldedInUI;
					}
					if(tempMesh.meshType == LoziMesh.MeshType.Static)
					{
						GUILayout.Label(icons[4]);
					}
					if(tempMesh.meshType == LoziMesh.MeshType.Skinned)
					{
						GUILayout.Label(icons[5]);
					}
					GUILayout.Label(tempMesh.name); 
					EditorGUILayout.EndHorizontal();
					if(tempMesh.isFoldedInUI)
					{ 
						GUI.Box(EditorGUILayout.BeginVertical("Box"), GUIContent.none);
						GUILayout.Label ("Advanced", EditorStyles.boldLabel);
						tempMesh.exportVertexColors = EditorGUILayout.Toggle("Export Vertex colors",tempMesh.exportVertexColors);
						tempMesh.recieveShadows     = EditorGUILayout.Toggle("Recieve Shadows", tempMesh.recieveShadows);
						tempMesh.castShadows        = EditorGUILayout.Toggle("Cast Shadows", tempMesh.castShadows);
						GUILayout.Space(10);
						EditorGUILayout.BeginVertical();
						GUILayout.Label ("Export Uvs");
						tempMesh.exportUvs[0] = EditorGUILayout.Toggle("UV1", tempMesh.exportUvs[0]);
						tempMesh.exportUvs[1] = EditorGUILayout.Toggle("UV2", tempMesh.exportUvs[1]);

						if(tempMesh.meshType == LoziMesh.MeshType.Skinned)
						{
							GUILayout.Space(10);
							int[] bones         = LoziExporter.instance.meshCollection.rootBoneIDArray<int>();
							int   boneIndex 	= getRootBoneIndex(tempMesh.rootBoneID,bones);

							boneIndex 			= EditorGUILayout.Popup("Root Bone ID",boneIndex,LoziExporter.instance.meshCollection.rootBoneIDArray<string>());

							tempMesh.rootBoneID = bones[boneIndex];

							if(GUILayout.Button("Reset Root Bone ID"))
							{
								tempMesh.resetBoneID();
							}
						}
						GUILayout.Space(10);
						EditorGUILayout.EndVertical();
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
