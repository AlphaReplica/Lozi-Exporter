/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * View for animations, shows animation name and draws clip names that's included
 */


using UnityEngine;
using UnityEditor;
using System.Collections;
using Lozi;

namespace Lozi.UI
{
	public class AnimationsView
	{
		private LoziAnimation tempAnim;
		private Texture2D[]   icons;
		private bool 		  unfolded;

		public AnimationsView(Texture2D[] viewIcons)
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

		// Folds/Unfolds all objects in view
		private void foldObjects(bool fold)
		{
			for(int num = 0; num < LoziExporter.instance.animationCollection.animations.Count; num++)
			{
				LoziExporter.instance.animationCollection.animations[num].isFoldedInUI = fold;
			}
		}
		
		// Draws Header vith view name and fold/unfold button
		private void drawHeader()
		{
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			GUILayout.Label ("Animations", EditorStyles.boldLabel);

			if(LoziExporter.instance.animationCollection.animations.Count>1)
			{
				if(GUILayout.Button((unfolded)? "Close All" : "Expand All"))
				{
					unfolded = !unfolded;
					foldObjects(unfolded);
				}
			}
			EditorGUILayout.EndHorizontal();
			
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			LoziExporter.instance.exportAnimations = GUILayout.Toggle(LoziExporter.instance.exportAnimations,"Export");
			EditorGUILayout.EndHorizontal();
		}
		
		// Draws animation objects if exits
		public bool drawAnimations()
		{
			GUI.skin.box.margin =  new RectOffset(10,10,0,0);
			
			if(LoziExporter.instance.animationCollection!=null &&
			   LoziExporter.instance.animationCollection.animations.Count>0)
			{
				EditorGUILayout.BeginVertical("Box");

				drawHeader();

				for(int num = 0; num < LoziExporter.instance.animationCollection.animations.Count; num++)
				{
					tempAnim = LoziExporter.instance.animationCollection.animations[num];
					GUI.skin.box.margin  = new RectOffset(0,0,0,0);
					if (GUI.Button(EditorGUILayout.BeginHorizontal("Box"), GUIContent.none))
					{
						tempAnim.isFoldedInUI = !tempAnim.isFoldedInUI;
					}
					if(tempAnim.isSkinAnimation)
					{
						GUILayout.Label(icons[11]);
					}
					else
					{
						GUILayout.Label(icons[6]);
					}
					GUILayout.Label(tempAnim.name); 
					EditorGUILayout.EndHorizontal();

					if(tempAnim.isFoldedInUI)
					{ 
						GUI.Box(EditorGUILayout.BeginVertical("Box"), GUIContent.none);
						GUILayout.Label ("Clips", EditorStyles.boldLabel);
						GUI.Box(EditorGUILayout.BeginVertical("Box"), GUIContent.none);
						for(int num2 = 0; num2 < tempAnim.animationClips.Count; num2++)
						{
							GUILayout.Label(tempAnim.animationClips[num2].clip.name);

						}
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
