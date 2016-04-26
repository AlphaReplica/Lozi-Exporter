using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniThreeD;
using Lozi.baseClasses;

namespace Lozi
{
	[System.Serializable]
	public class LoziBone : HierarchyObject
	{
		public string      name;
		public string      objectId;
		public string      parentID;
		public List<float> pos;
		public List<float> rotq;
		public List<float> scl;

		public LoziBone():base()
		{

		}

		public override void setObject(Transform target)
		{
			base.setObject(target);
			name     = obj.name;
			
			Vector3 poz = obj.localPosition;

			pos      = new List<float>(){obj.localPosition.x, obj.localPosition.y, obj.localPosition.z};
			rotq     = new List<float>(){obj.localRotation.x, obj.localRotation.y, obj.localRotation.z,obj.localRotation.w};
			scl      = new List<float>(){obj.localScale.x,	  obj.localScale.y,    obj.localScale.z};
		}
		
		public bool isSame(Transform target)
		{
			if(target==obj)
			{
				return true;
			}
			return false;
		}
		
		public Dictionary<string,object> boneDictionary
		{
			get
			{
				Dictionary<string,object> dict = new Dictionary<string, object>();
				dict["parent"] = parent;
				dict["name"  ] = name;
				dict["id"    ] = objectId;
				dict["pos"   ] = pos;
				dict["rotq"  ] = rotq;
				dict["scl"   ] = scl;
				
				return dict;
			}
		}

		public static int getParent(GameObject target,GameObject hierarchyParent)
		{
			string path  = GetPath(target,hierarchyParent);
			int    index = getParentIndex(path);
			
			Debug.Log(hierarchyParent.name+" -- "+path+ " -- " +index.ToString());
			return index;
		}
		
		public static int getParentIndex(string path)
		{
			string[] strings = path.Split('/');
			return strings.Length-2;
		}

		public static int getParentIndex2(string path)
		{
			string[] strings = path.Split('/');
			return strings.Length-3;
		}

		public static string GetPath(GameObject target,GameObject exclude = null)
		{
			List<string> path = new List<string>();
			
			Transform current = target.transform;
			path.Add(current.name);
			
			while (current.parent != null && current.parent != exclude.transform)
			{
				path.Insert(0, current.parent.name);
				current = current.parent;
			}
			return string.Join("/", path.ToArray());
		}
	}
}
