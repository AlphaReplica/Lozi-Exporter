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
	public class LoziCollider
	{
		private int              type;
		private GameObject	 	  obj;
		private Collider	 collider;

		public LoziCollider(GameObject target)
		{
			type 	 = -1;
			this.obj = target;

			if(this.obj.GetComponent<BoxCollider>()!=null)
			{
				collider = this.obj.GetComponent<BoxCollider>();
				type = 0;
			}
			if(this.obj.GetComponent<SphereCollider>()!=null)
			{
				collider = this.obj.GetComponent<SphereCollider>();
				type = 1;
			}
			if(this.obj.GetComponent<CapsuleCollider>()!=null)
			{
				collider = this.obj.GetComponent<CapsuleCollider>();
				type = 2;
			}
		}

		public bool hasColliderInfo
		{
			get
			{
				return (type>-1);
			}
		}

		public Dictionary<string,object> getVectorAsDictionary(Vector3 value)
		{
			Dictionary<string,object> dict = new Dictionary<string, object>();

			dict["x"] = value.x;
			dict["y"] = value.y;
			dict["z"] = value.z;

			return dict;
		}

		// current object properties as dictionary
		public Dictionary<string,object> objectProperties
		{
			get
			{
				Dictionary<string,object> dict = new Dictionary<string, object>();
				
				dict["id"		] = collider.GetInstanceID();
				dict["type"		] = type;
				dict["center"   ] = getVectorAsDictionary(collider.bounds.center);
				dict["size"     ] = getVectorAsDictionary(collider.bounds.size );
				dict["isTrigger"] = collider.isTrigger;

				return dict;
			}
		}
		
		public static bool hasCollider(GameObject obj)
		{
			if(obj.GetComponent<Collider>()!=null)
			{
				return true;
			}
			return false;
		}
	}
}