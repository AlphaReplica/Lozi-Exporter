using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Lozi;

namespace Lozi.baseClasses
{
	public class HierarchyManager<T> : IDisposable where T: HierarchyObject
	{
		public List<T> unsortedObjects;
		public List<T> sortedObjects;

		public Transform baseObj;

		public void setTarget(Transform obj)
		{
			baseObj = obj;
		}

		protected void sortObjectsByHierarchyIndex(T target,int parent = -1)
		{
			if(sortedObjects==null)
			{
				sortedObjects = new List<T>();
			}
			if(target!=null)
			{
				sortedObjects.Add(target);
				
				int index = getObjectIndex(target);

				target.parent = parent;
				target.index  = index;

				for(int num = 0; num < target.obj.childCount; num++)
				{
					T childTarget = getObjectByTransform(target.obj.GetChild(num));
					if(childTarget==null)
					{
						childTarget     = System.Activator.CreateInstance(typeof(T)) as T;
						childTarget.setObject(target.obj.GetChild(num));
						unsortedObjects.Add(childTarget);
					}
					sortObjectsByHierarchyIndex(childTarget,index);
				}
			}
		}

		public T getObjectByTransform(Transform target)
		{
			for(int num = 0; num < unsortedObjects.Count; num++)
			{
				if(unsortedObjects[num].obj==target)
				{
					return unsortedObjects[num];
				}
			}
			return null;
		}
		
		public int getObjectIndex(T target)
		{
			for(int num = 0; num < sortedObjects.Count; num++)
			{
				if(target.obj==sortedObjects[num].obj)
				{
					return num;
				}
			}
			return -1;
		}

		protected bool hasInUnsortedArray(Transform target)
		{
			foreach(T obj in unsortedObjects)
			{
				if(obj.obj == target)
				{
					return true;
				}
			}
			return false;
		}

		protected T rootObject
		{
			get
			{
				foreach(T obj in unsortedObjects)
				{
					if(obj.isRoot)
					{
						return obj;
					}
				}
				return null;
			}
		}

		public int getSortebObjectIndexByTarget(T target)
		{
			for(int num = 0; num < sortedObjects.Count; num++)
			{
				if(target.obj==sortedObjects[num].obj)
				{
					return num;
				}
			}
			return -1;
		}

		protected T getUnsortedObjectByIndex(int index)
		{
			if(unsortedObjects!=null)
			{
				if(unsortedObjects.Count>index)
				{
					return unsortedObjects[index];
				}
			}
			return null;
		}

		protected int getObjectIndexByPath(string path)
		{
			if(sortedObjects!=null)
			{
				for(int num = 0; num < sortedObjects.Count; num++)
				{
					if(path == sortedObjects[num].path)
					{
						return num;
					}
				}
			}
			return -1;
		}

		protected int getObjectParentIndexByPath(string path)
		{
			if(sortedObjects!=null)
			{
				for(int num = 0; num < sortedObjects.Count; num++)
				{
					if(path == sortedObjects[num].path)
					{
						return sortedObjects[num].parent;
					}
				}
			}
			return -1;
		}

		virtual public void Dispose()
		{
			if(unsortedObjects!=null)
			{
				for(int num = 0; num < unsortedObjects.Count; num++)
				{
					unsortedObjects[num].Dispose();
				}
			}
			if(sortedObjects!=null)
			{
				for(int num = 0; num < sortedObjects.Count; num++)
				{
					sortedObjects[num].Dispose();
				}
			}

			unsortedObjects.Clear();
			sortedObjects.Clear();

			unsortedObjects = null;
			sortedObjects   = null;
			baseObj 		= null;

		}
	}
}