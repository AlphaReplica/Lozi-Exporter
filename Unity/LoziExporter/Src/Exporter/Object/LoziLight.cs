/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Collider Component
 */
/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Light object
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziLight
	{
		public bool isFoldedInUI;
		
		private GameObject		 obj;
		private Light       lightObj;
		
		public LoziLight(GameObject target)
		{
			this.obj = target;
			lightObj = this.obj.GetComponent<Light>();
		}
		
		public LightType type
		{
			get
			{
				return lightObj.type;
			}
		}
		
		
		private List<float> colorToList(Color col)
		{
			return new List<float>(){col.r,col.g,col.b,col.a};
		}
		
		// current object properties as dictionary
		public Dictionary<string,object> objectProperties
		{
			get
			{
				Dictionary<string,object> dict = new Dictionary<string, object>();

				dict["lightID"  ] = lightObj.GetInstanceID();
				dict["color"	] = colorToList(lightObj.color);
				dict["intensity"] = lightObj.intensity;
				dict["angle"    ] = lightObj.spotAngle;
				dict["range"    ] = lightObj.range;
				dict["shadow"	] = (lightObj.shadows==LightShadows.None) ? false : true;
				
				return dict;
			}
		}
		
		public static bool hasLight(GameObject obj)
		{
			if(obj.GetComponent<Light>()!=null)
			{
				return true;
			}
			return false;
		}
	}
}