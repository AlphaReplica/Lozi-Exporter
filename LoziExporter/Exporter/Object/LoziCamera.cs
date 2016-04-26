using UnityEngine;
using System.Collections;

namespace Lozi
{
	public class LoziCamera
	{
		public bool isFoldedInUI;
		
		private int		       	    objectId;
		private string            objectName;
		private GameObject				 obj;
		private Camera			   cameraObj;

		public LoziCamera(GameObject target)
		{
			this.obj  = target;
			cameraObj = this.obj.GetComponent<Camera>();
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