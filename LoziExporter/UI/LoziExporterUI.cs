using UnityEngine;
using UnityEditor;
using System.Collections;
using Lozi;

public class LoziExporterUI : EditorWindow
{
	private RectOffset offset;
	private Vector2 scroll;
	private int oldExportType =-1; 
	private int exportType 	  = 1;
	private string[] exportTabs = new string[]{"Scene","GameObject"};
	private GameObject oldtarget;
	private GameObject target;
	private GUISkin	   skin;

	private bool hierarchyUnfolded;
	private Texture2D[] icons;

	private GUIStyle buttonStyle;

	private LoziMesh tempMesh;

	[MenuItem("Tools/Lozi Exporter")]
	static void Init ()
	{
		LoziExporterUI window = (LoziExporterUI)EditorWindow.GetWindow (typeof (LoziExporterUI));

		window.position		  = new Rect(Screen.width/2, Screen.height/2,500, 600);

		window.Show();
	}
	
	public LoziExporterUI()
	{
		title 	 = "Lozi Exporter";
		skin	 = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
		icons    = new Texture2D[7];
		icons[0] = Resources.Load("lozi-icon-scene", 	     typeof(Texture2D)) as Texture2D;
		icons[1] = Resources.Load("lozi-icon-object", 	     typeof(Texture2D)) as Texture2D;
		icons[2] = Resources.Load("lozi-icon-camera", 	     typeof(Texture2D)) as Texture2D;
		icons[3] = Resources.Load("lozi-icon-light", 	     typeof(Texture2D)) as Texture2D;
		icons[4] = Resources.Load("lozi-icon-mesh", 	     typeof(Texture2D)) as Texture2D;
		icons[5] = Resources.Load("lozi-icon-skinnedMesh",   typeof(Texture2D)) as Texture2D;
		icons[6] = Resources.Load("lozi-icon-objectAnimated",typeof(Texture2D)) as Texture2D;
	}
	
	void OnGUI ()
	{
		if(offset==null)
		{
			offset = GUI.skin.box.margin;
		}

		GUILayout.Label ("Lozi "+exportTabs[exportType]+" Exporter", EditorStyles.boldLabel);

		getTarget();
		
		GUILayout.Space(10);

		scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width), GUILayout.Height (Screen.height - ((exportType==1)?140:120)));
		if(target!=null)
		{
			offset = new RectOffset(10,10,0,0);
			GUI.skin.box.margin = offset;
			drawMeshes();

			GUILayout.Space(10);

			offset = new RectOffset(10,10,0,0);
			GUI.skin.box.margin = offset; 
			drawHierarchy();
		}
		GUI.skin.box.margin = offset;
		EditorGUILayout.EndScrollView();
		if(GUILayout.Button("EXPORT"))
		{
			LoziExporter.instance.export();
		}
	}

	private void getTarget()
	{
		exportType = GUILayout.Toolbar(exportType, exportTabs);

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
		}
		GUILayout.Space(10);
		
		if(exportType == 1)
		{
			target = EditorGUILayout.ObjectField("Target GameObject", target,typeof(GameObject)) as GameObject;
		}

		if(target!=null && target!=oldtarget)
		{
			hierarchyUnfolded = false;
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

	private void drawMeshes()
	{
		Rect vertical = EditorGUILayout.BeginVertical("Box");
		//GUI.Box(vertical,GUIContent.none);

		GUILayout.Label ("Meshes", EditorStyles.boldLabel);

		if(LoziExporter.instance.meshCollection!=null)
		{
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
					tempMesh.geometryRotOffset  = EditorGUILayout.Vector3Field("Rotate Geometry",tempMesh.geometryRotOffset);
					GUILayout.Space(10);
					tempMesh.exportVertexColors = EditorGUILayout.Toggle("Export Vertex colors",tempMesh.exportVertexColors);
					GUILayout.Space(10);
					EditorGUILayout.BeginVertical();
					GUILayout.Label ("Export Uvs");
					tempMesh.exportUvs[0] = EditorGUILayout.Toggle("UV1", tempMesh.exportUvs[0]);
					tempMesh.exportUvs[1] = EditorGUILayout.Toggle("UV2", tempMesh.exportUvs[1]);
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
				}
			}
		}
		EditorGUILayout.EndVertical();
	}

	private void drawHierarchy()
	{
		Rect vertical = EditorGUILayout.BeginVertical("Box");
		GUI.Box(vertical,GUIContent.none);

		GUI.Box(EditorGUILayout.BeginHorizontal("Label"),GUIContent.none);
		GUILayout.Label ("Hierarchy", EditorStyles.boldLabel);
		if(GUILayout.Button((hierarchyUnfolded)? "Close All" : "Expand All"))
		{
			hierarchyUnfolded = !hierarchyUnfolded;

			foldHierarchy(LoziExporter.instance.target,hierarchyUnfolded);
		}
		EditorGUILayout.EndHorizontal();
		drawHierarchyItem(LoziExporter.instance.target);
		
		EditorGUILayout.EndVertical();
	}

	private void drawHierarchyItem(LoziObject target, int count = 0)
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
			case "Scene" 	  	  :GUILayout.Label(icons[0]); break;
			case "Object"	  	  :GUILayout.Label(icons[1]); break;
			case "Camera"	  	  :GUILayout.Label(icons[2]); break;
			case "Light" 	  	  :GUILayout.Label(icons[3]); break;
			case "Mesh"  	  	  :GUILayout.Label(icons[4]); break;
			case "SkinnedMesh"	  :GUILayout.Label(icons[5]); break;
			case "AnimationObject":GUILayout.Label(icons[6]); break;
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

	void OnDestroy()
	{
		LoziExporter.instance.reset();
	}
}