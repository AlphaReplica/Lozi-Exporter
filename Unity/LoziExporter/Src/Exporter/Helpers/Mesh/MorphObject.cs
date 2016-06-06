/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Value object for storing mesh morph vertices
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Lozi;

namespace Lozi.helpers
{
	public class MorphObject : IDisposable
	{
		public string name;
		public List<float> vertices;

		public void Dispose()
		{
			vertices.Clear();

			name     = null;
			vertices = null;
		}
	}
}
