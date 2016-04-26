using UnityEngine;
using System;
using System.Collections;
using Lozi;

namespace Lozi.helpers
{
	public class LoziAnimationKeyPoint : IDisposable
	{
		public int       size;
		public float[]	 time;
		public Vector4[] point;

		public void Dispose()
		{
			time  = null;
			point = null;
		}
	}
}
