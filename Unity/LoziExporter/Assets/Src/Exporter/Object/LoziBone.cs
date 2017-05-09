/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Bone Object for skinnedMeshRenderer
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lozi.baseClasses;

namespace Lozi
{
	[System.Serializable]
	public class LoziBone : HierarchyObject
	{
		public string      	 		 name;
		public int	       	 		 objectId;
		public string      	 		 parentID;
		public List<float> 	 		 pos;
		public List<float> 	 		 rotq;
		public List<float> 	 		 scl;
		public LoziCollider  		 collider;
		public LoziScriptProperties  props;
		public LoziSoundSource       sound;
		public bool          		 includeColliders;
		public bool          		 includeScripts;

		public LoziBone():base()
		{
		}

		public override void setObject(Transform target)
		{
			base.setObject(target);

			if(target)
			{
				if(LoziCollider.hasCollider(obj.gameObject))
				{
					collider = new LoziCollider(obj.gameObject);
				}
				if(LoziScriptProperties.hasScript(obj.gameObject))
				{
					props = new LoziScriptProperties(obj.gameObject);
				}
				if(LoziSoundSource.hasSound(obj.gameObject))
				{
					if(LoziSoundSource.hasClip(obj.gameObject))
					{
						sound = new LoziSoundSource(obj.gameObject);
					}
				}
				name     = obj.name;
				objectId = obj.GetInstanceID();

				Vector3 posVec  = obj.localPosition;
				Quaternion quat = obj.localRotation;
				Vector3 scalVec = obj.localScale;

				posVec = new Vector3(-posVec.x,posVec.y,posVec.z);
				quat   = Quaternion.Euler(obj.localEulerAngles.x,obj.localEulerAngles.y,obj.localEulerAngles.z);
				quat   = new Quaternion(-quat.x,quat.y,quat.z,-quat.w);
				//quat = Quaternion.Inverse(quat);
				//posVec = Vector3.Reflect(posVec,Vector3.forward);
				pos      = new List<float>(){posVec.x, posVec.y, posVec.z};
				rotq     = new List<float>(){quat.x, quat.y,  quat.z, quat.w};
				scl      = new List<float>(){obj.localScale.x,	  obj.localScale.y,    obj.localScale.z};
			}
		}

		public void generateScriptProperties()
		{
			if(props!=null)
			{
				props.generate();
			}
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
				dict["parent"  ] = parent;
				dict["name"    ] = name;
				dict["id"      ] = objectId;
				dict["pos"     ] = pos;
				dict["rotq"    ] = rotq;
				dict["scl"     ] = scl;

				if(sound!=null)
				{
					dict["sound"] = sound.sourceProperties;
				}

				if(includeScripts)
				{
					if(props!=null)
					{
						dict["scriptProperties"] = props.objectProperties;
					}
				}
				if(includeColliders)
				{
					if(collider!=null && collider.hasColliderInfo)
					{
						dict["collider"] = collider.objectProperties;
					}
				}
				
				return dict;
			}
		}

		public static int getParent(GameObject target,GameObject hierarchyParent)
		{
			string path  = GetPath(target,hierarchyParent);
			int    index = getParentIndex(path);

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

		public static string GetPath(GameObject target,GameObject exclude)
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
