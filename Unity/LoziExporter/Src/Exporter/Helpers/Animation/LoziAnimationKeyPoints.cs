/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Value object for storing keypoint data array
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Lozi;

namespace Lozi.helpers
{
	public class LoziAnimationKeyPoints : IDisposable
	{
		public string  path;
		public int     index;
		public int     parent;
		public List<float> times;
		public LoziAnimationKeyPoint pos;
		public LoziAnimationKeyPoint rot;
		public LoziAnimationKeyPoint scl;

		public void Dispose()
		{
			times.Clear();

			pos.Dispose();
			rot.Dispose();
			scl.Dispose();

			path  = null;
			times = null;
			pos   = null;
			rot   = null;
			scl   = null;
		}
	}
}
