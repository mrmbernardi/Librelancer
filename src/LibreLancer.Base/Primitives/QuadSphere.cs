﻿/* The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * 
 * The Initial Developer of the Original Code is Callum McGing (mailto:callum.mcging@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2013-2016
 * the Initial Developer. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using LibreLancer.Vertices;

namespace LibreLancer.Primitives
{
	public class QuadSphere
	{
		VertexBuffer vertexBuffer;
		ElementBuffer elementBuffer;

		int primitiveCountSide;

		Dictionary<CubeMapFace, int> offsets = new Dictionary<CubeMapFace, int>();

		public IVertexType VertexType
		{
			get
			{
				return vertexBuffer.VertexType;
			}
		}
		public VertexBuffer VertexBuffer
		{
			get
			{
				return vertexBuffer;
			}
		}
		VertexPositionNormalTexture[] vertices;
		ushort[] indices;
		public QuadSphere(int slices)
		{
			int planeVerts = (slices + 1) * (slices + 1);
			vertices = new VertexPositionNormalTexture[planeVerts * 6];
			int planeIndices = slices * slices * 6;
			indices = new ushort[planeIndices * 6];
			bool[] ccw = new bool[planeIndices * 6];
			primitiveCountSide = planeIndices / 3;
			//Offsets
			offsets.Add(CubeMapFace.PositiveY, 0);
			offsets.Add(CubeMapFace.NegativeY, planeIndices);
			offsets.Add(CubeMapFace.NegativeZ, 2 * planeIndices);
			offsets.Add(CubeMapFace.PositiveZ, 3 * planeIndices);
			offsets.Add(CubeMapFace.NegativeX, 4 * planeIndices);
			offsets.Add(CubeMapFace.PositiveX, 5 * planeIndices);
			//Generate planes
			int vertexCount = 0;
			//BOTTOM
			TopBottom(1, slices, vertices, vertexCount);
			vertexCount += planeVerts;
			//TOP
			TopBottom(-1, slices, vertices, vertexCount);
			vertexCount += planeVerts;
			//FRONT
			FrontBack(-1, slices, vertices, vertexCount);
			vertexCount += planeVerts;
			//BACK
			FrontBack(1, slices, vertices, vertexCount);
			vertexCount += planeVerts;
			//LEFT
			LeftRight(-1, slices, vertices, vertexCount);
			vertexCount += planeVerts;
			//RIGHT
			LeftRight(1, slices, vertices, vertexCount);
			vertexCount += planeVerts;
			//Generate indices
			int indexCount = 0;
			int baseVert = 0;
			//BOTTOM
			Indices(2, 1, 0, 1, 2, 3, slices, ref indexCount, indices, baseVert);
			baseVert += planeVerts;
			//TOP
			Indices(0, 1, 2, 3, 2, 1, slices, ref indexCount, indices, baseVert);
			baseVert += planeVerts;
			//FRONT
			Indices(2, 1, 0, 1, 2, 3, slices, ref indexCount, indices, baseVert);
			baseVert += planeVerts;
			//BACK
			Indices(0, 1, 2, 3, 2, 1, slices, ref indexCount, indices, baseVert);
			baseVert += planeVerts;
			//LEFT
			Indices(2, 1, 0, 1, 2, 3, slices, ref indexCount, indices, baseVert);
			baseVert += planeVerts;
			//RIGHT
			Indices(0, 1, 2, 3, 2, 1, slices, ref indexCount, indices, baseVert);
			//Transform Cube to Sphere
			for (int i = 0; i < vertices.Length; i++)
			{
				float x = vertices[i].Position.X;
				float y = vertices[i].Position.Y;
				float z = vertices[i].Position.Z;
				vertices[i].Position = new Vector3(
					(float)(x * Math.Sqrt(1.0 - (y * y / 2.0) - (z * z / 2.0) + (y * y * z * z / 3.0))),
					(float)(y * Math.Sqrt(1.0 - (z * z / 2.0) - (x * x / 2.0) + (z * z * x * x / 3.0))),
					(float)(z * Math.Sqrt(1.0 - (x * x / 2.0) - (y * y / 2.0) + (x * x * y * y / 3.0)))
				);
				vertices[i].Normal = vertices[i].Position;
			}
			//Upload
			vertexBuffer = new VertexBuffer(typeof(VertexPositionNormalTexture), vertices.Length);
			vertexBuffer.SetData(vertices);
			elementBuffer = new ElementBuffer(indices.Length);
			elementBuffer.SetData(indices);
			vertexBuffer.SetElementBuffer(elementBuffer);
		}
		void TopBottom(int Y, int slices, VertexPositionNormalTexture[] vertices, int vertexCount)
		{
			int width = slices + 1, height = slices + 1;
			float advance = (2f / slices);
			float tadvance = (1f / slices);
			for (int z = 0; z < height; z++)
			{
				int basev = vertexCount + (z * width);
				for (int x = 0; x < width; x++)
				{
					int index = basev + x;
					vertices[index] = new VertexPositionNormalTexture(
						new Vector3(
							-1 + advance * x,
							Y,
							-1 + advance * z
						),
						Vector3.Zero,
						new Vector2(
							tadvance * x, 
							(Y == -1) ? tadvance * z : 1 - (tadvance * z)
						)
					);
				}
			}
		}
		void FrontBack(int Z, int slices, VertexPositionNormalTexture[] vertices, int vertexCount)
		{
			int width = slices + 1, height = slices + 1;
			float advance = (2f / slices);
			float tadvance = (1f / slices);
			for (int z = 0; z < height; z++)
			{
				int basev = vertexCount + (z * width);
				for (int x = 0; x < width; x++)
				{
					int index = basev + x;
					vertices[index] = new VertexPositionNormalTexture(
						new Vector3(
							-1 + advance * x,
							-1 + advance * z,
							Z
						),
						Vector3.Zero,
						new Vector2(
							(Z == -1) ? 1 - (tadvance * x) : tadvance * x, 
							tadvance * z
						)
					);
				}
			}
		}
		void LeftRight(int X, int slices, VertexPositionNormalTexture[] vertices, int vertexCount)
		{
			int width = slices + 1, height = slices + 1;
			float advance = (2f / slices);
			float tadvance = (1f / slices);
			for (int z = 0; z < height; z++)
			{
				int basev = vertexCount + (z * width);
				for (int x = 0; x < width; x++)
				{
					int index = basev + x;
					vertices[index] = new VertexPositionNormalTexture(
						new Vector3(
							X,
							-1 + advance * x,
							-1 + advance * z
						),
						Vector3.Zero,
						new Vector2(
							(X == -1) ? tadvance * z : 1 - (tadvance * z), 
							tadvance * x
						)
					);
				}
			}
		}
		void Indices(ushort t0, ushort t1, ushort t2, ushort t3, ushort t4, ushort t5, int slices, ref int i, ushort[] indices, int baseVert)
		{
			int width = slices + 1;
			int height = slices;
			ushort[] temp = new ushort[6];
			for (int y = 0; y < height; y++)
			{
				int basev = baseVert + (y * width);
				for (int x = 0; x < slices; x++)
				{
					//Allow defined winding order
					temp[0] = (ushort)(basev + x);
					temp[1] = (ushort)(basev + x + 1);
					temp[2] = (ushort)(basev + width + x);
					temp[3] = (ushort)(basev + width + x + 1);

					indices[i++] = temp[t0];
					indices[i++] = temp[t1];
					indices[i++] = temp[t2];

					indices[i++] = temp[t3];
					indices[i++] = temp[t4];
					indices[i++] = temp[t5];
				}
			}
		}
		public void GetDrawParameters(CubeMapFace face, out int start, out int count)
		{
			start = offsets[face];
			count = primitiveCountSide;
		}
	}
}