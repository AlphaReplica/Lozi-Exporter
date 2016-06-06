/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * View for Hierarchy, recursively shows object with children and sub children, with icons
 */


using UnityEngine;
using UnityEditor;
using System.Collections;
using Lozi;

namespace Lozi.UI
{
	public class HierarchyView
	{
		private Texture2D[] icons;
		private bool 		unfolded;

		public HierarchyView(Texture2D[] viewIcons)
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

		// Draws View Header with title and fold/unfolding button
		// Also flip setting could be set from here
		private void drawHeader()
		{	
			GUI.Box(EditorGUILayout.BeginVertical("Label"),GUIContent.none,GUIStyle.none);
			
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			GUILayout.Label ("Hierarchy", EditorStyles.boldLabel);

			if(LoziExporter.instance.target.childObjects.Count>0)
			{
				if(GUILayout.Button((unfolded)? "Close All" : "Expand All"))
				{
					unfolded = !unfolded;
					foldHierarchy(LoziExporter.instance.target,unfolded);
				}
			}

			EditorGUILayout.EndHorizontal();
			
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			LoziExporter.instance.exportHierarchy = GUILayout.Toggle(LoziExporter.instance.exportHierarchy,"Export");
			LoziExporter.instance.target.flip[0]  = GUILayout.Toggle(LoziExporter.instance.target.flip[0],"Flip X");
			LoziExporter.instance.target.flip[1]  = GUILayout.Toggle(LoziExporter.instance.target.flip[1],"Flip Y");
			LoziExporter.instance.target.flip[2]  = GUILayout.Toggle(LoziExporter.instance.target.flip[2],"Flip Z");
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.EndVertical();
		}

		// Recursively folds/unfolds all views
		private void foldHierarchy(LoziObject obj, bool fold)
		{
			if(obj!=null)
			{
				obj.isFoldedInUI = fold;
				
				for(int num = 0; num < obj.childObjects.Count; num++)
				{
					foldHierarchy(obj.childObjects[num],fold);
				}
			}
		}

		// Draws hierarchy view
		public void drawHierarchy()
		{
			GUI.skin.box.margin = new RectOffset(10,10,0,0); 
			if(LoziExporter.instance.target!=null)
			{
				Rect vertical = EditorGUILayout.BeginVertical("Box");
				
				GUI.Box(vertical,GUIContent.none);

				drawHeader();

				drawHierarchyItem(LoziExporter.instance.target,0);
				
				EditorGUILayout.EndVertical();
			}
		}
		
		// Draws hierarchy Item recursiely
		private void drawHierarchyItem(LoziObject target, int count)
		{
			if(target==null)
			{
				return;
			}
			
			count++;
			if (GUI.Button(EditorGUILayout.BeginVertical(), GUIContent.none))
			{
				target.isFoldedInUI = !target.isFoldedInUI;
			}
			
			GUI.skin.box.margin  = new RectOffset(10*count,0,0,0);
			GUI.Box(EditorGUILayout.BeginHorizontal("Box"),GUIContent.none);
			switch(target.objectType)
			{
				case ObjectType.Scene 	  	   	   :GUILayout.Label(icons[0]); break;
				case ObjectType.Object	  	   	   :GUILayout.Label(icons[1]); break;
				case ObjectType.PerspectiveCamera  :GUILayout.Label(icons[2]); break;
				case ObjectType.OrthographicCamera :GUILayout.Label(icons[2]); break;
				case ObjectType.PointLight 	  	   :GUILayout.Label(icons[3]); break;
				case ObjectType.DirectionalLight   :GUILayout.Label(icons[3]); break;
				case ObjectType.SpotLight 	  	   :GUILayout.Label(icons[3]); break;
				case ObjectType.AreaLight 	  	   :GUILayout.Label(icons[3]); break;
				case ObjectType.Mesh  	  	   	   :GUILayout.Label(icons[4]); break;
				case ObjectType.SkinnedMesh	   	   :GUILayout.Label(icons[5]); break;
				case ObjectType.AnimationObject	   :GUILayout.Label(icons[6]); break;
				case ObjectType.Bone      	   	   :GUILayout.Label(icons[7]); break;
			}
			GUILayout.Label(target.name); 
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.EndVertical();
			if(target.isFoldedInUI)
			{ 
				if(target.childObjects.Count>0)
				{
					EditorGUILayout.BeginVertical();
					for(int num = 0; num < target.childObjects.Count; num++)
					{
						drawHierarchyItem(target.childObjects[num],count);
					}
					EditorGUILayout.EndVertical();
				}
			}
		}
	}
}
