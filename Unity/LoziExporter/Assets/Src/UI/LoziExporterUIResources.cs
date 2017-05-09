/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * UI Icons
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using System.IO;
using Lozi;

namespace Lozi.UI
{
	public class LoziExporterUIResources
	{
		public Texture2D[] icons;
		public LoziExporterUIResources()
		{
			icons     = new Texture2D[13];
			icons[ 0] = loadTexture("lozi-icon-scene");
			icons[ 1] = loadTexture("lozi-icon-object");
			icons[ 2] = loadTexture("lozi-icon-camera");
			
			icons[ 3] = loadTexture("lozi-icon-light");
			icons[ 4] = loadTexture("lozi-icon-mesh");
			icons[ 5] = loadTexture("lozi-icon-skinnedMesh");
			icons[ 6] = loadTexture("lozi-icon-objectAnimated");
			icons[ 7] = loadTexture("lozi-icon-bone");
			icons[ 8] = loadTexture("lozi-icon-material");
			icons[ 9] = loadTexture("lozi-icon-texture");
			icons[10] = loadTexture("lozi-icon-cubemap");
			icons[11] = loadTexture("lozi-icon-skinAnimated");
			icons[12] = loadTexture("lozi-icon-sound");
		}

		private Texture2D loadTexture(string name)
		{
			Texture2D texture = (Texture2D)Resources.Load(name, typeof(Texture2D)) as Texture2D;

			if(texture == null)
			{
				Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LoziExporter.Src.UI.Resources." + name + ".png");
				texture 	  = new Texture2D(2,2);
				texture.LoadImage(ReadToEnd(stream));
			}
			return texture;
		}

		// Method by Alex Chouls
		private byte[] ReadToEnd(Stream stream)
		{
			long originalPosition = stream.Position;
			stream.Position = 0;
			
			try
			{
				byte[] readBuffer = new byte[4096];
				
				int totalBytesRead = 0;
				int bytesRead;
				
				while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
				{
					totalBytesRead += bytesRead;
					
					if (totalBytesRead == readBuffer.Length)
					{
						int nextByte = stream.ReadByte();
						if (nextByte != -1)
						{
							byte[] temp = new byte[readBuffer.Length * 2];
							Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
							Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
							readBuffer = temp;
							totalBytesRead++;
						}
					}
				}
				
				byte[] buffer = readBuffer;
				if (readBuffer.Length != totalBytesRead)
				{
					buffer = new byte[totalBytesRead];
					Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
				}
				return buffer;
			}
			finally
			{
				stream.Position = originalPosition;
			}
		}
	}
}