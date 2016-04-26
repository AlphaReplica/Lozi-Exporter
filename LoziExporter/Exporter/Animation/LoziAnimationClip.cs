using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Lozi.baseClasses;
using Lozi.helpers;

namespace Lozi
{
	public class LoziAnimationClip : HierarchyManager<HierarchyObject>
	{
		private string         clipName;
		private string         wrapMode;
		private AnimationClip  clip;
		private float fps;
		private float length;
		private List<LoziAnimationKeyPoints> keys;

		public LoziAnimationClip (AnimationClip clip, Transform target):base()
		{
			setTarget(target);

			keys = new List<LoziAnimationKeyPoints>();

			clipName = clip.name;
			wrapMode = clip.wrapMode.ToString();
			fps      = clip.frameRate;
			length   = clip.length;

			AnimationClipCurveData[] curvesArray = AnimationUtility.GetAllCurves (clip, true);

			for(int num1 = 0; num1 < curvesArray.Length; num1++)
			{
				addToKeys(curvesArray[num1]);

				Dictionary<string, object> curve = new Dictionary<string, object>();
				curve["name"] = curvesArray[num1].propertyName;
				curve["type"] = curvesArray[num1].type.Name;
			}
			mergeTimes();
			getAnimationTargets();
			sortObjectsByHierarchyIndex(rootObject);
		}

		public void getAnimationTargets()
		{
			unsortedObjects = new List<HierarchyObject>();
			for(int num1 = 0; num1 < keys.Count; num1++)
			{
				string  path 		 = keys[num1].path;
				string[] pathTargets = path.Split('/');

				path = "";
				for(int num2 = 0; num2 < pathTargets.Length; num2++)
				{
					path += "/"+pathTargets[num2];

					if(path[0]=='/')
					{
						path = path.Substring(1);
					}

					Transform targetObj = baseObj.FindChild(path);

					if(targetObj!=null)
					{
						if(!hasInUnsortedArray(targetObj))
						{
							HierarchyObject animated = new HierarchyObject();
							animated.isRoot  		 = (path.Contains("/")) ? false : true;
							animated.path            = path;
							animated.setObject(targetObj);
							unsortedObjects.Add(animated);
						}
					}
				}
			}
		}

		private void parsePoint(string vec, LoziAnimationKeyPoint points, Keyframe[] keys)
		{
			if(points.time==null)
			{
				points.size  = keys.Length;
				points.time  = new float  [keys.Length];
				points.point = new Vector4[keys.Length];
			}
			
			for(int num = 0; num < keys.Length; num++)
			{
				if(points.point[num]==null){points.point[num] = new Vector4();}

				Vector4 temp = points.point[num];
				switch(vec)
				{
					case "x":{points.point[num] = new Vector4(keys[num].value,temp.y,temp.z,temp.w); break;}
					case "y":{points.point[num] = new Vector4(temp.x,temp.y,keys[num].value,temp.w); break;}
					case "z":{points.point[num] = new Vector4(temp.x,keys[num].value,temp.z,temp.w); break;}
					case "w":{points.point[num] = new Vector4(temp.x,temp.y,temp.z,keys[num].value); break;}
				}
				points.time[num] = keys[num].time;
			}
		}

		private LoziAnimationKeyPoints getKeyPoint(string path)
		{
			LoziAnimationKeyPoints point = null;
			for(int num = 0; num < keys.Count; num++)
			{
				if(keys[num].path == path)
				{
					point = keys[num];
					break;
				}
			}
			if(point==null)
			{
				point        = new LoziAnimationKeyPoints();
				point.pos    = new LoziAnimationKeyPoint();
				point.rot    = new LoziAnimationKeyPoint();
				point.scl    = new LoziAnimationKeyPoint();
				point.path   = path;
				keys.Add(point);
			}
			return point;
		}

		private void addToKeys(AnimationClipCurveData curveData)
		{
			string prop  = curveData.propertyName.Split('.')[0];
			string vec   = curveData.propertyName.Split('.')[1];
			LoziAnimationKeyPoints point = getKeyPoint(curveData.path);

			switch(prop)
			{
				case "m_LocalPosition":{parsePoint(vec,point.pos,curveData.curve.keys); break;}
				case "m_LocalRotation":{parsePoint(vec,point.rot,curveData.curve.keys); break;}
				case "m_LocalScale"	  :{parsePoint(vec,point.scl,curveData.curve.keys); break;}
			}
		}

		private void mergeTimes()
		{
			for(int num1 = 0; num1 < keys.Count; num1++)
			{
				LoziAnimationKeyPoints point = keys[num1];
				point.times = new List<float>();
				if(point.pos.time!=null){for(int num2 = 0; num2 < point.pos.time.Length; num2++){if(!point.times.Contains(point.pos.time[num2])){point.times.Add(point.pos.time[num2]);}}}
				if(point.rot.time!=null){for(int num2 = 0; num2 < point.rot.time.Length; num2++){if(!point.times.Contains(point.rot.time[num2])){point.times.Add(point.rot.time[num2]);}}}
				if(point.scl.time!=null){for(int num2 = 0; num2 < point.scl.time.Length; num2++){if(!point.times.Contains(point.scl.time[num2])){point.times.Add(point.scl.time[num2]);}}}
				point.times.Sort();
			}
		}

		private float[] getPointArrayByTime(LoziAnimationKeyPoint keyPoint, float time,int length)
		{
			float[] arr = new float[length];

			if(keyPoint.time!=null)
			{
				for(int num = 0; num < keyPoint.time.Length; num++)
				{
					if(time==keyPoint.time[num])
					{
						Vector4 point = keyPoint.point[num];
						
						arr[0] = point.x;
						arr[2] = point.y;
						arr[1] = point.z;
						
						if(length>3)
						{
							arr[3] = point.w;
						}
						return arr;
					}
				}
			}
			return null;
		}

		public List<Dictionary<string,object>> keysDictionary
		{
			get
			{
				List<Dictionary<string,object>> keysArr = new List<Dictionary<string, object>>();
				for(int num1 = 0; num1 < keys.Count; num1++)
				{
					List<Dictionary<string,object>> timesArr = new List<Dictionary<string, object>>();
					Dictionary<string,object> dict 			 = new Dictionary<string, object>();

					Vector3 oldPos = Vector3.zero;
					Vector3 oldScl = Vector3.one;
					for(int num2 = 0; num2 < keys[num1].times.Count; num2++)
					{
						Dictionary<string,object> keyDict = new Dictionary<string, object>();
						float[] pos						  = getPointArrayByTime(keys[num1].pos,keys[num1].times[num2],3);
						float[] rot						  = getPointArrayByTime(keys[num1].rot,keys[num1].times[num2],4);
						float[] scl						  = getPointArrayByTime(keys[num1].scl,keys[num1].times[num2],3);

						keyDict["time"] = keys[num1].times[num2];
						if(pos!=null){keyDict["pos"] = pos;}
						if(rot!=null){keyDict["rot"] = rot;}
						if(scl!=null){keyDict["scl"] = scl;}

						timesArr.Add(keyDict);
					}
					dict["path"  ] = keys[num1].path;
					
					dict["index" ] = getObjectIndexByPath(keys[num1].path);
					dict["parent"] = getObjectParentIndexByPath(keys[num1].path);
					dict["keys"  ] = timesArr;
					keysArr.Add(dict);
				}
				return keysArr;
			}
		}

		public Dictionary<string,object> clipDictionary
		{
			get
			{
				Dictionary<string,object> dict = new Dictionary<string, object>();
				dict["name"  	] = clipName;
				dict["fps"   	] = fps;
				dict["length"	] = length;
				dict["hierarchy"] = keysDictionary;

				return dict;
			}
		}

		override public void Dispose()
		{
			base.Dispose();
			
			for(int num = 0; num < keys.Count; num++)
			{
				keys[num].Dispose();
			}
			keys.Clear();

			keys     = null;
			clipName = null;
			wrapMode = null;
			clip     = null;
		}
	}
}

