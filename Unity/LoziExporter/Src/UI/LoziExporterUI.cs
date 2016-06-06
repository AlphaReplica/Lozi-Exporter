/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Interface for lozi exporter with all features available
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using Lozi;

namespace Lozi.UI
{
	public class LoziExporterUI : EditorWindow
	{
		private RectOffset     			offset;
		private Vector2 	   			scroll;
		private int 		   			oldExportType =-1; 
		private int 		   			exportType = 1;
		private string[]       			exportTabs = new string[]{"Scene","GameObject"};
		private GameObject     			oldtarget;
		private GameObject     			target;
		private GUISkin	       			skin;
		
		private bool 		   			advanced = false;
		private bool 		   			isAdvancedOpened = false;
		private HierarchyView  			hierarchy;
		private MeshesView     			meshes;
		private MaterialsView  			materials;
		private TexturesView   			textures;
		private AnimationsView			animations;

		private LoziExporterUIResources resources;

		[MenuItem("Tools/Lozi Exporter")]
		static void Init ()
		{
			LoziExporterUI window		    = LoziExporterUI.CreateInstance<LoziExporterUI>();
			window.position 				= new Rect(Screen.width/2,Screen.height/2,500, (window.advanced) ? 650 : 140);
			window.autoRepaintOnSceneChange = true;
			window.resize(); 
			   
			window.ShowUtility();
		}

		// Constructor creates objects for other views and sets icons
		public LoziExporterUI()
		{
			title 	   = "Lozi Exporter "+LoziExporter.Version;
			skin	   = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

			resources  = new LoziExporterUIResources();
			hierarchy  = new HierarchyView(resources.icons);
			meshes     = new MeshesView(resources.icons);
			materials  = new MaterialsView(resources.icons);
			textures   = new TexturesView(resources.icons);
			animations = new AnimationsView(resources.icons);
		}

		void resize()
		{
			this.position = new Rect(this.position.x,this.position.y,500, (advanced) ? 650 : 140);
		}


		void OnGUI ()
		{
			GUILayout.Label ("Lozi "+exportTabs[exportType]+" Exporter", EditorStyles.boldLabel);
			getTarget();

			if(target!=null)
			{
				advanced = EditorGUILayout.Foldout(advanced, "Advanced");
				if(isAdvancedOpened!=advanced)
				{
					isAdvancedOpened = advanced;
					resize();
				}
			}
			else
			{
				advanced = false;
				GUILayout.Space(18);
			}
			drawAdvanced();
			drawExportPanel();
		}
		
		// Draws Top panel where target setted, draws tabs [Scene|GameObject] and help buttons, it's a prerequisite to draw other views
		private void getTarget()
		{
			EditorGUILayout.BeginHorizontal("Label");
			exportType = GUILayout.Toolbar(exportType, exportTabs);
			if(GUILayout.Button("Help"))
			{
				Application.OpenURL("http://alphareplica.github.io/Lozi-Exporter/help.html");
			}
			EditorGUILayout.EndHorizontal();
			
			if(exportType!=oldExportType)
			{
				target = null;
				LoziExporter.instance.reset();
				if(exportType == 0)
				{
					LoziExporter.instance.setSceneObject();
					target = LoziExporter.instance.sceneObject;
				}
				oldExportType = exportType;
				advanced = false;
				resize();
			}
			GUILayout.Space(10);
			
			GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none);
			
			if(exportType == 1)
			{
				target = EditorGUILayout.ObjectField("Target GameObject", target,typeof(GameObject),true) as GameObject;
			}
			if(target!=null)
			{
				if(GUILayout.Button("Refresh"))
				{
					oldtarget = null;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			if(target!=null && target!=oldtarget)
			{
				hierarchy.fold = false;
				LoziExporter.instance.setGameObject(target);
				oldtarget = target;
			} 
			if(target!=null)
			{
				if(LoziExporter.instance.target==null)
				{
					LoziExporter.instance.setGameObject(target);
					oldtarget = target;
				}
			}
		}
		
		// Draws advanced dropdown view, [Hierarchy,Meshes,Materials,Textures]
		private void drawAdvanced()
		{
			if(advanced)
			{
				if(offset==null)
				{
					offset = GUI.skin.box.margin;
				}
				
				GUI.Box(EditorGUILayout.BeginVertical("Label"),GUIContent.none);
				GUILayout.Space(5);
				scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 15), GUILayout.Height (495));
				
				if(target!=null)
				{
					if(animations.drawAnimations())
					{
						GUILayout.Space(10);
					}
					if(textures.drawTextures())
					{
						GUILayout.Space(10);
					}
					if(materials.drawMaterials())
					{
						GUILayout.Space(10);
					}
					if(meshes.drawMeshes())
					{
						GUILayout.Space(10);
					}
					hierarchy.drawHierarchy();
				}
				GUI.skin.box.margin = offset;
				EditorGUILayout.EndScrollView();
				GUILayout.Space(5);
				EditorGUILayout.EndVertical();
			}
			GUILayout.Space(5);
		}
		
		// Draws Export Box at the botton of window if target exits
		private void drawExportPanel()
		{
			if(target!=null)
			{
				GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none);
				if(GUILayout.Button((LoziExporter.instance.pathToSave==null || LoziExporter.instance.pathToSave.Length==0) ? "Select path" : LoziExporter.instance.pathToSave,skin.textField))
				{
					LoziExporter.instance.pathToSave = EditorUtility.SaveFilePanel("Save as Js","",target.name + ".lozi.js","js");
				}
				
				if(GUILayout.Button("EXPORT"))
				{
					if(LoziExporter.instance.pathToSave.Length>0)
					{
						LoziExporter.instance.export();
					}
					else
					{
						Debug.LogWarning("LoziExporter:No export path chosen!");
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.Label("Select target GameObject");
			}
		}
		
		// Clears current target and resets all vars
		public void reset()
		{
			LoziExporter.instance.reset();
			exportType = 1;
			oldExportType = -1;
			target = null;
		}

		void OnProjectChange()
		{
			reset();
		}

		void OnDestroy()
		{
			reset();
		}
	}
}