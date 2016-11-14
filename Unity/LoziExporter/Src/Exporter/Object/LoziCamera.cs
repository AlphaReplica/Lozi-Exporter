/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Camera object
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziCamera
	{
		public bool isFoldedInUI;

		private GameObject				 obj;
		private Camera			   cameraObj;

		public LoziCamera(GameObject target)
		{
			this.obj  = target;
			cameraObj = this.obj.GetComponent<Camera>();
		}

		public bool orthographic
		{
			get
			{
				return cameraObj.orthographic;
			}
		}

		// current object properties as dictionary
		public Dictionary<string,object> objectProperties
		{
			get
			{
				Dictionary<string,object> dict = new Dictionary<string, object>();

				dict["cameraID" ] = cameraObj.GetInstanceID();
				dict["fov"	    ] = cameraObj.fieldOfView;
				dict["aspect"   ] = cameraObj.aspect;
				dict["near"     ] = cameraObj.nearClipPlane;
				dict["far"      ] = cameraObj.farClipPlane;
				dict["orthoSize"] = cameraObj.orthographicSize;
				dict["depth"    ] = cameraObj.depth;
				dict["isOrtho"  ] = cameraObj.orthographic;

				
				return dict;
			}
		}

		public static bool hasCamera(GameObject obj)
		{
			if(obj.GetComponent<Camera>()!=null)
			{
				return true;
			}
			return false;
		}
	}
}