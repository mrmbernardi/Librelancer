﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Numerics;
using System.Collections.Generic;
using LibreLancer.Fx;

namespace LibreLancer
{
    public class ProjectileManager
    {
        public Projectile[] Projectiles = new Projectile[16384];

        int projectilePtr = 0;

        GameWorld world;
        public ProjectileManager(GameWorld world)
        {
            this.world = world;
        }

        public void FixedUpdate(double time)
        {
            var tFloat = (float)time;
            for(int i = 0; i < Projectiles.Length; i++) {
                if (!Projectiles[i].Alive) continue;
                Projectiles[i].Position += (Projectiles[i].Normal * tFloat);
                world.DrawDebug(Projectiles[i].Position);
                Projectiles[i].Time += tFloat;
                if(Projectiles[i].Time >= Projectiles[i].Data.Lifetime) {
                    Projectiles[i].Alive = false;
                } else if (world.Physics.PointCollision(Projectiles[i].Position, out Physics.PhysicsObject po, out Vector3 contactPoint)) {
                    Projectiles[i].Alive = false;
                    world.Renderer.SpawnTempFx(Projectiles[i].Data.HitEffect, contactPoint);
                }
                
            }
        }

        Dictionary<string, ProjectileData> datas = new Dictionary<string, ProjectileData>();
        public ProjectileData GetData(GameData.Items.GunEquipment gunDef)
        {
            ProjectileData pdata;
            if (datas.TryGetValue(gunDef.Nickname, out pdata)) return pdata;
            pdata = new ProjectileData();
            pdata.Munition = gunDef.Munition;
            pdata.Lifetime = gunDef.Munition.Def.Lifetime;
            pdata.Velocity = gunDef.Def.MuzzleVelocity;
            var res = world.Renderer.Game.GetService<GameDataManager>();
            pdata.HitEffect = res.GetEffect(gunDef.Munition.Def.MunitionHitEffect)
                .GetEffect(world.Renderer.ResourceManager);
            datas.Add(gunDef.Nickname, pdata);
            return pdata;
        }

        public void SpawnProjectile(ProjectileData projectile, Vector3 position, Vector3 heading)
        {
            if (projectilePtr == 16383) projectilePtr = 0;
            Projectiles[projectilePtr] = new Projectile() {
                Data = projectile,
                Time = 0,
                Alive = true,
                Position = position,
                Start = position,
                Normal = heading * projectile.Velocity
            };
            projectilePtr++;
        }
    }
    public class ProjectileData
    {
        public GameData.Items.MunitionEquip Munition;
        public float Velocity;
        public float Lifetime;
        public ParticleEffect HitEffect;
    }

    public struct Projectile
    {
        public ProjectileData Data;
        public bool Alive;
        public float Time;
        public Vector3 Position;
        public Vector3 Start;
        public Vector3 Normal;
    }
}
