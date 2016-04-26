using UnityEngine;
using System.Collections;

namespace Lozi
{
	public class LoziLight
	{
		public bool isFoldedInUI;
		
		private int		       	    objectId;
		private string            objectName;
		private GameObject				 obj;
		private Camera			    lightObj;

		public LoziLight(GameObject target)
		{
			this.obj = target;
			lightObj = this.obj.GetComponent<Camera>();
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