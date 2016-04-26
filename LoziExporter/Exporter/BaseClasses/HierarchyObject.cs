using System;
using UnityEngine;
using System.Collections;
using Lozi;

namespace Lozi.baseClasses
{
	public class HierarchyObject : IDisposable
	{
		public Transform obj;
		public int		 parent;
		public int       index;
		public bool      isRoot;
		public string    path;

		virtual public void setObject(Transform target)
		{
			obj = target;
		}
		
		virtual public void Dispose()
		{
			obj  = null;
			path = null;
		}
	}
}