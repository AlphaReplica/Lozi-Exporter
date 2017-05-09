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
	public class SoundsView
	{
		private LoziSound	  tempSound;
		private Texture2D[]   icons;
		private bool 		  unfolded;

		public SoundsView(Texture2D[] viewIcons)
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
			for(int num = 0; num < LoziExporter.instance.soundCollection.sounds.Count; num++)
			{
				LoziExporter.instance.soundCollection.sounds[num].isFoldedInUI = fold;
			}
		}
		
		// Draws Header vith view name and fold/unfold button
		private void drawHeader()
		{
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			GUILayout.Label ("Sounds", EditorStyles.boldLabel);

			if(LoziExporter.instance.soundCollection.sounds.Count>1)
			{
				if(GUILayout.Button((unfolded)? "Close All" : "Expand All"))
				{
					unfolded = !unfolded;
					foldObjects(unfolded);
				}
			}
			EditorGUILayout.EndHorizontal();
			
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none,GUIStyle.none);
			LoziExporter.instance.exportSounds = GUILayout.Toggle(LoziExporter.instance.exportSounds,"Export");
			LoziExporter.instance.soundCollection.includeSoundsInFile = GUILayout.Toggle(LoziExporter.instance.soundCollection.includeSoundsInFile,"Include in Lozi");
			EditorGUILayout.EndHorizontal();
		}
		
		// Draws animation objects if exits
		public bool drawSounds()
		{
			GUI.skin.box.margin =  new RectOffset(3,2,0,0);
			
			if(LoziExporter.instance.soundCollection!=null &&
			   LoziExporter.instance.soundCollection.sounds.Count>0)
			{
				EditorGUILayout.BeginVertical("Box");

				drawHeader();

				for(int num = 0; num < LoziExporter.instance.soundCollection.sounds.Count; num++)
				{
					tempSound = LoziExporter.instance.soundCollection.sounds[num];
					GUI.skin.box.margin  = new RectOffset(0,0,0,0);
					if (GUI.Button(EditorGUILayout.BeginHorizontal("Box"), GUIContent.none))
					{
						tempSound.isFoldedInUI = !tempSound.isFoldedInUI;
					}
					GUILayout.Label(icons[12]);
					GUILayout.Label(tempSound.soundClipName); 
					EditorGUILayout.EndHorizontal();

					if(tempSound.isFoldedInUI)
					{ 
						GUI.Box(EditorGUILayout.BeginVertical("Box"), GUIContent.none);
						GUILayout.Label ("Channels: "+tempSound.soundClip.channels.ToString());
						GUILayout.Label ("Frequency: "+tempSound.soundClip.frequency.ToString());
						GUILayout.Label ("Samples: "+tempSound.soundClip.samples.ToString());
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
