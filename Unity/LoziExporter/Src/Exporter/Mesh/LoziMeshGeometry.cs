/**
 * Created by Beka Mujirishvili (ifrit88@gmail.com)
 * 
 * Generates geometry data from provided mesh and stores
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lozi
{
	public class LoziMeshGeometry : IDisposable
	{
		private Mesh             	 mesh;
		private int	        	 objectId;
		private string         objectName;
		private List<float> vertextColors;
		private List<float>  meshVertices;
		private List<float>   meshNormals;
		private List<List<float>> meshUvs;
		private List<List<int>>	meshFaces;

		private bool   exportVertexColors;
		private bool[] 			exportUvs;

		public LoziMeshGeometry(Mesh meshObj,bool exportVertexColors,bool[] exportUvs)
		{
			this.mesh     	 		= meshObj;
			this.exportUvs			= exportUvs;
			this.exportVertexColors = exportVertexColors;
			this.objectId           = this.mesh.GetInstanceID();

			objectName   = mesh.name;
			objectId     = mesh.GetInstanceID();
		}

		public void generate(bool exportVertexColors,bool[] exportUvs)
		{
			this.exportUvs			= exportUvs;
			this.exportVertexColors = exportVertexColors;

			clear();

			meshVertices = new List<float>();
			meshNormals  = new List<float>();
			meshUvs		 = new List<List<float>>();
			meshFaces    = new List<List<int>>();

			parseVertices();
			parseNormals();
			parseUVs();
			parseFaces();
			parseVertexColors();
		}

		private void parseVertices()
		{
			foreach(Vector3 vertice in mesh.vertices)
			{
				meshVertices.Add(-vertice.x);
				meshVertices.Add(vertice.y);
				meshVertices.Add(vertice.z);
			}
		}

		private void parseNormals()
		{
			Vector3[] normals = mesh.normals;

			foreach(Vector3 normal in normals)
			{
				meshNormals.Add(normal.x);
				meshNormals.Add(normal.y);
				meshNormals.Add(normal.z);
			}
		}

		private void parseUVs()
		{
			for(int num = 0; num < exportUvs.Length; num++)
			{
				Vector2[] uvArr = null;
				
				switch(num)
				{
					case 0 :{uvArr = mesh.uv;  break;}
					case 1 :{uvArr = mesh.uv2; break;}
				}

				if(exportUvs[num]==true)
				{
					meshUvs.Add(new List<float>());
					foreach(Vector2 uv in uvArr)
					{
						meshUvs[num].Add(uv.x);
						meshUvs[num].Add(uv.y);
					}
				}
			}
		}

		private void parseVertexColors()
		{
			if(exportVertexColors)
			{
				vertextColors = new List<float>();
				for(int num = 0; num < mesh.colors.Length; num++)
				{
					vertextColors.Add(mesh.colors[num].r);
					vertextColors.Add(mesh.colors[num].g);
					vertextColors.Add(mesh.colors[num].b);
					vertextColors.Add(mesh.colors[num].a);
					
				}
			}
		}

		private void parseFaces()
		{
			for(int num1 = 0; num1 < mesh.subMeshCount; num1++)
			{
				List<int> trianglesArr = new List<int>();
				int[] triangles = mesh.GetTriangles(num1);

				for(int num2 = 0; num2 < triangles.Length; num2+=3)
				{
					int temp = triangles[num2 + 0];
					triangles[num2 + 0] = triangles[num2 + 1];
					triangles[num2 + 1] = temp;
				}

				for(int num2 = 0; num2 < triangles.Length; num2++)
				{
					trianglesArr.Add(triangles[num2]+1);
				}
				meshFaces.Add(trianglesArr);
			}
		}

		public List<float> vertices
		{
			get{return meshVertices;}
		}
		
		public List<float> normals
		{
			get{return meshNormals;}
		}
		
		public List<List<float>> uvs
		{
			get{return meshUvs;}
		}

		public List<List<int>> faces
		{
			get{return meshFaces;}
		}
		
		public int id
		{
			get{return objectId;}
			set{objectId = value;}
		}
		
		public string name
		{
			get{return objectName;}
		}

		public Mesh meshObject
		{
			get{return mesh;}
		}

		private void clear()
		{
			if(meshUvs!=null)
			{
				for(int num = 0; num < meshUvs.Count; num++)
				{
					meshUvs[num].Clear();
				}
				meshUvs.Clear();
			}
			if(meshVertices!=null)
			{
				meshVertices.Clear();
			}
			if(meshNormals!=null)
			{
				meshNormals.Clear();
			}
			if(meshFaces!=null)
			{
				meshFaces.Clear();
			}
			if(vertextColors!=null)
			{
				vertextColors.Clear();
			}
		}

		public void Dispose()
		{
			clear();

			vertextColors  = null;
			meshVertices   = null;
			meshNormals    = null;
			meshFaces 	   = null;
			meshUvs 	   = null;
			mesh 		   = null;
			objectName 	   = null;
			exportUvs 	   = null;
		}
	}
}