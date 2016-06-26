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
using System.Runtime.InteropServices;
using LibreLancer.Vertices;
namespace LibreLancer
{
	public class Billboards
	{
		const int MAX_BILLBOARDS = 1024;

		[StructLayout(LayoutKind.Sequential)]
		struct BVert : IVertexType
		{
			public Vector3 Position;
			public Vector2 Size;
			public Color4 Color;
			public Vector2 Texture0;
			public Vector2 Texture1;
			public Vector2 Texture2;
			public Vector2 Texture3;
			public float Angle;

			public VertexDeclaration GetVertexDeclaration()
			{
				return new VertexDeclaration (
					18 * sizeof(float),
					new VertexElement (VertexSlots.Position, 3, VertexElementType.Float, false, 0),
					new VertexElement (VertexSlots.Size, 2, VertexElementType.Float, false, sizeof(float) * 3),
					new VertexElement (VertexSlots.Color, 4, VertexElementType.Float, false, sizeof(float) * 5),
					new VertexElement (VertexSlots.Texture1, 2, VertexElementType.Float, false, sizeof(float) * 9),
					new VertexElement (VertexSlots.Texture2, 2, VertexElementType.Float, false, sizeof(float) * 11),
					new VertexElement (VertexSlots.Texture3, 2, VertexElementType.Float, false, sizeof(float) * 13),
					new VertexElement (VertexSlots.Texture4, 2, VertexElementType.Float, false, sizeof(float) * 15),
					new VertexElement (VertexSlots.Angle, 1, VertexElementType.Float, false, sizeof(float) * 17)
				);
			}

		}

		Shader shader;
		BVert[] vertices;
		VertexBuffer vbo;
		public Billboards ()
		{
			shader = ShaderCache.Get (
				"Billboard.vs",
				"Billboard.frag",
				"Billboard.gs"
			);
			shader.SetInteger ("tex0", 0);
			vertices = new BVert[MAX_BILLBOARDS];
			vbo = new VertexBuffer (typeof(BVert), MAX_BILLBOARDS, true);
		}

		ICamera camera;
		Texture2D currentTexture;
		int billboardCount = 0;
		RenderState renderstate;
		public void Begin(ICamera cam, RenderState rs)
		{
			camera = cam;
			currentTexture = null;
			billboardCount = 0;
			renderstate = rs;
		}

		public void Draw(
			Texture2D texture,
			Vector3 Position,
			Vector2 size,
			Color4 color,
			Vector2 topleft,
			Vector2 topright,
			Vector2 bottomleft,
			Vector2 bottomright,
			float angle
		)
		{
			if (currentTexture != texture && currentTexture != null)
				Flush ();
			if (billboardCount + 1 > MAX_BILLBOARDS)
				Flush ();
			currentTexture = texture;
			//setup vertex
			vertices [billboardCount].Position = Position;
			vertices [billboardCount].Size = size;
			vertices [billboardCount].Color = color;
			vertices [billboardCount].Texture0 = topleft;
			vertices [billboardCount].Texture1 = topright;
			vertices [billboardCount].Texture2 = bottomleft;
			vertices [billboardCount].Texture3 = bottomright;
			vertices [billboardCount].Angle = angle;
			//increase count
			billboardCount++;
		}

		void Flush()
		{
			if (billboardCount == 0)
				return;
			
			var view = camera.View;
			var vp = camera.ViewProjection;
			shader.SetMatrix ("View", ref view);
			shader.SetMatrix ("ViewProjection", ref vp);
			currentTexture.BindTo (0);
			shader.UseProgram ();
			//draw
			renderstate.Cull = false;
			renderstate.BlendMode = BlendMode.Normal;
			vbo.SetData(vertices, billboardCount);
			vbo.Draw (PrimitiveTypes.Points, billboardCount);
			renderstate.Cull = true;
			//blah
			currentTexture = null;
			billboardCount = 0;
		}

		public void End()
		{
			Flush ();
		}
	}
}
