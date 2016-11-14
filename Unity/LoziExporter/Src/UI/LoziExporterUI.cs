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
		private SoundsView              sounds;

		private LoziExporterUIResources resources;

		[MenuItem("Tools/Lozi Exporter")]
		static void Init ()
		{
			LoziExporterUI window		    = LoziExporterUI.CreateInstance<LoziExporterUI>();
			window.position 				= new Rect(Screen.width/2-200,Screen.height/2 - 100,500, (window.advanced) ? 650 : 140);
			window.autoRepaintOnSceneChange = true;
			window.minSize                  = new Vector3(500,130);
			window.resize(); 
			   
			window.ShowUtility();
		}

		// Constructor creates objects for other views and sets icons
		public LoziExporterUI()
		{
			titleContent = new GUIContent("Lozi Exporter "+LoziExporter.Version);
			skin	   	 = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

			resources  = new LoziExporterUIResources();
			hierarchy  = new HierarchyView(resources.icons);
			meshes     = new MeshesView(resources.icons);
			materials  = new MaterialsView(resources.icons);
			textures   = new TexturesView(resources.icons);
			animations = new AnimationsView(resources.icons);
			sounds     = new SoundsView(resources.icons);
		}

		void resize()
		{
			Vector2 pos   = new Vector2(position.x,position.y);
			this.minSize  = new Vector3(500,(advanced) ? 200 : 140); 
			this.position = new Rect(pos.x,pos.y,500, (advanced) ? 650 : 140);
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
				scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(position.width - 15), GUILayout.Height (position.height-160));
				
				if(target!=null)
				{
					if(sounds.drawSounds())
					{
						GUILayout.Space(10);
					}
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
				if(GUILayout.Button((LoziExporter.instance.pathToSave==null || LoziExporter.instance.pathToSave.Length==0) ? "Select path" : LoziExporter.instance.pathToSave,skin.textField, GUILayout.MaxWidth(350)))
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