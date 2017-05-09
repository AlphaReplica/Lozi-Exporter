/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Value object for storing material property data
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Lozi;

namespace Lozi.helpers
{
	public class LoziMaterialProperty : IDisposable
	{
		public  ShaderUtil.ShaderPropertyType type;
		public  string 	 propertyName;
		public  object   valObject;

		public LoziMaterialProperty(ShaderUtil.ShaderPropertyType propertyType, string name, object value)
		{
			type 		 = propertyType;
			propertyName = name;
			valObject 	 = value;
		}

		public void Dispose()
		{
			propertyName = null;
			valObject    = null;
		}
	}
}