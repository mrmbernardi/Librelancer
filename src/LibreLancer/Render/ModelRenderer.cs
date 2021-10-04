﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Numerics;
using System.Collections.Generic;
using LibreLancer.Utf.Cmp;
using LibreLancer.Utf.Mat;
using DfmFile = LibreLancer.Utf.Dfm.DfmFile;
using LibreLancer.GameData;
namespace LibreLancer
{
	public class ModelRenderer : ObjectRenderer
	{
		public Matrix4x4 World { get; private set; }
		public RigidModel Model { get; private set; }
        
		public NebulaRenderer Nebula;
        
        Vector3 pos;
		bool inited = false;
		SystemRenderer sysr;

		public ModelRenderer(RigidModel model)
		{
			Model = model;
		}

        double spinX;
        double spinY;
        double spinZ;
		public override void Update(double elapsed, Vector3 position, Matrix4x4 transform)
		{
			if (sysr == null)
				return;
			World = transform;
            if(Spin != Vector3.Zero) {
                spinX += elapsed * Spin.X;
                if (spinX > (2 * Math.PI)) spinX -= 2 * Math.PI;
                spinY += elapsed * Spin.Y;
                if (spinY > (2 * Math.PI)) spinY -= 2 * Math.PI;
                spinZ += elapsed * Spin.Z;
                if (spinZ > (2 * Math.PI)) spinZ -= 2 * Math.PI;
            }
            if (Nebula == null || pos != position && sysr != null)
			{
				pos = position;
				Nebula = sysr.ObjectInNebula(position);
			}
        }

        int GetLevel(RigidModelPart file, Vector3 center, Vector3 camera)
		{
			float[] ranges = LODRanges ?? file.Mesh.Switch2;
			var dsq = Vector3.DistanceSquared(center, camera);
            if (ranges == null) {
                CurrentLevel = 0;
                return 0;
            }
            int lvl = -1;
			for (int i = 0; i < ranges.Length; i++)
			{
				var d = ranges[i];
				if (i > 0 && ranges[i] < ranges[i - 1]) break;
				if (dsq < (d * sysr.LODMultiplier) * (d * sysr.LODMultiplier)) break;
                if (i >= file.Mesh.Levels.Length) {
                    CurrentLevel = -1;
                    return -1;
                }
                CurrentLevel = lvl = i;
			}
			return lvl;
		}

		public override bool OutOfView(ICamera camera)
		{
			if (Model != null)
            {
                foreach (var part in Model.AllParts)
                {
                    if (!part.Active) continue;
                    if (part.Mesh == null) continue;
                    var center = Vector3.Transform(part.Mesh.Center, part.LocalTransform * World);
                    var lvl = GetLevel(part, center, camera.Position);
                    if (lvl != -1)
                    {
                        var bsphere = new BoundingSphere(center, part.Mesh.Radius);
                        if (camera.Frustum.Intersects(bsphere)) return false; //visible
                    }
                }
            }
            return true; //not visible
		}

		public override void DepthPrepass(ICamera camera, RenderContext rstate)
		{
            
        }

        Matrix4x4 _worldSph;
        public override bool PrepareRender(ICamera camera, NebulaRenderer nr, SystemRenderer sys)
		{
            _worldSph = World;
            if(Spin != Vector3.Zero)
            {
                _worldSph = (Matrix4x4.CreateRotationX((float)spinX) *
                     Matrix4x4.CreateRotationY((float)spinY) *
                     Matrix4x4.CreateRotationZ((float)spinZ)) * World;
            }
            this.sysr = sys;
			if (Nebula != null && nr != Nebula)
			{
                return false;
			}
			var dsq = Vector3.DistanceSquared(pos, camera.Position);
			if (LODRanges != null) //Fastest cull
			{
				var maxd = LODRanges[LODRanges.Length - 1] * sysr.LODMultiplier;
				maxd *= maxd;
                if (dsq > maxd)
                {
                    CurrentLevel = -1;
                    return false;
                }
			}
            if (Model != null)
            {
                foreach (var part in Model.AllParts)
                { 
                    if(!part.Active || part.Mesh == null) continue;
                    var center = Vector3.Transform(part.Mesh.Center, part.LocalTransform * World);
                    var lvl = GetLevel(part, center, camera.Position);
                    if (lvl != -1)
                    {
                        var bsphere = new BoundingSphere(center, part.Mesh.Radius);
                        if (camera.Frustum.Intersects(bsphere))
                        {
                            sysr.AddObject(this);
                            return true; //visible
                        }
                    }
                }
            }
            return false;
		}
		public override void Draw(ICamera camera, CommandBuffer commands, SystemLighting lights, NebulaRenderer nr)
		{
            if (Model != null) {
                Model.Update(camera, sysr.Game.TotalTime, sysr.ResourceManager);
                foreach (var part in Model.AllParts)
                {
                    if (part.Mesh == null) continue;
                    if (!part.Active) continue;
                    var w = part.LocalTransform * World;
                    var center = Vector3.Transform(part.Mesh.Center, w);
                    var lvl = GetLevel(part, center, camera.Position);
                    if (lvl == -1) continue;
                    var lighting = RenderHelpers.ApplyLights(lights, LightGroup, center, part.GetRadius(), nr,
                        LitAmbient, LitDynamic, NoFog);
                    var r = part.GetRadius() + lighting.FogRange.Y;
                    if (lighting.FogMode != FogModes.Linear ||
                        Vector3.DistanceSquared(camera.Position, center) <= (r * r)) {
                        part.Mesh.DrawBuffer(lvl, sysr.ResourceManager, commands, w, ref lighting, Model.MaterialAnims);
                    }
                }
            }
		}
	}
}

