﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LibreLancer.Shaders
{
    using System;
    
    public class Basic_PositionNormalColorTexture
    {
        static ShaderVariables[] variants;
        private static bool iscompiled = false;
        private static int GetIndex(ShaderFeatures features)
        {
            ShaderFeatures masked = (features & ((ShaderFeatures)(135)));
            if ((masked == ((ShaderFeatures)(1))))
            {
                return 1;
            }
            if ((masked == ((ShaderFeatures)(128))))
            {
                return 2;
            }
            if ((masked == ((ShaderFeatures)(129))))
            {
                return 3;
            }
            if ((masked == ((ShaderFeatures)(2))))
            {
                return 4;
            }
            if ((masked == ((ShaderFeatures)(3))))
            {
                return 5;
            }
            if ((masked == ((ShaderFeatures)(130))))
            {
                return 6;
            }
            if ((masked == ((ShaderFeatures)(131))))
            {
                return 7;
            }
            if ((masked == ((ShaderFeatures)(4))))
            {
                return 8;
            }
            if ((masked == ((ShaderFeatures)(5))))
            {
                return 9;
            }
            if ((masked == ((ShaderFeatures)(132))))
            {
                return 10;
            }
            if ((masked == ((ShaderFeatures)(133))))
            {
                return 11;
            }
            if ((masked == ((ShaderFeatures)(6))))
            {
                return 12;
            }
            if ((masked == ((ShaderFeatures)(7))))
            {
                return 13;
            }
            if ((masked == ((ShaderFeatures)(134))))
            {
                return 14;
            }
            if ((masked == ((ShaderFeatures)(135))))
            {
                return 15;
            }
            return 0;
        }
        public static ShaderVariables Get(LibreLancer.Graphics.RenderContext device, ShaderFeatures features)
        {
            AllShaders.Compile(device);
            return variants[GetIndex(features)];
        }
        public static ShaderVariables Get(LibreLancer.Graphics.RenderContext device)
        {
            AllShaders.Compile(device);
            return variants[0];
        }
        internal static void Compile(LibreLancer.Graphics.RenderContext device, string sourceBundle)
        {
            if (iscompiled)
            {
                return;
            }
            iscompiled = true;
            ShaderVariables.Log("Compiling Basic_PositionNormalColorTexture");
            variants = new ShaderVariables[16];
            if (device.HasFeature(LibreLancer.Graphics.GraphicsFeature.Features430))
            {
                variants[0] = ShaderVariables.Compile(device, sourceBundle.Substring(184594, 1657), sourceBundle.Substring(45704, 6786));
                variants[1] = ShaderVariables.Compile(device, sourceBundle.Substring(184594, 1657), sourceBundle.Substring(56725, 6840));
                variants[2] = ShaderVariables.Compile(device, sourceBundle.Substring(190494, 4584), sourceBundle.Substring(74349, 2730));
                variants[3] = ShaderVariables.Compile(device, sourceBundle.Substring(190494, 4584), sourceBundle.Substring(79522, 2784));
                variants[4] = ShaderVariables.Compile(device, sourceBundle.Substring(184594, 1657), sourceBundle.Substring(86531, 6830));
                variants[5] = ShaderVariables.Compile(device, sourceBundle.Substring(184594, 1657), sourceBundle.Substring(97640, 6884));
                variants[6] = ShaderVariables.Compile(device, sourceBundle.Substring(190494, 4584), sourceBundle.Substring(106957, 2774));
                variants[7] = ShaderVariables.Compile(device, sourceBundle.Substring(190494, 4584), sourceBundle.Substring(112218, 2828));
                variants[8] = ShaderVariables.Compile(device, sourceBundle.Substring(184594, 1657), sourceBundle.Substring(119421, 6980));
                variants[9] = ShaderVariables.Compile(device, sourceBundle.Substring(184594, 1657), sourceBundle.Substring(130830, 7034));
                variants[10] = ShaderVariables.Compile(device, sourceBundle.Substring(190494, 4584), sourceBundle.Substring(140447, 2924));
                variants[11] = ShaderVariables.Compile(device, sourceBundle.Substring(190494, 4584), sourceBundle.Substring(146008, 2978));
                variants[12] = ShaderVariables.Compile(device, sourceBundle.Substring(184594, 1657), sourceBundle.Substring(153405, 7024));
                variants[13] = ShaderVariables.Compile(device, sourceBundle.Substring(184594, 1657), sourceBundle.Substring(164902, 7078));
                variants[14] = ShaderVariables.Compile(device, sourceBundle.Substring(190494, 4584), sourceBundle.Substring(174607, 2968));
                variants[15] = ShaderVariables.Compile(device, sourceBundle.Substring(190494, 4584), sourceBundle.Substring(180256, 3022));
            }
            else
            {
                variants[0] = ShaderVariables.Compile(device, sourceBundle.Substring(183278, 1316), sourceBundle.Substring(40082, 4181));
                variants[1] = ShaderVariables.Compile(device, sourceBundle.Substring(183278, 1316), sourceBundle.Substring(52490, 4235));
                variants[2] = ShaderVariables.Compile(device, sourceBundle.Substring(186251, 4243), sourceBundle.Substring(67592, 2389));
                variants[3] = ShaderVariables.Compile(device, sourceBundle.Substring(186251, 4243), sourceBundle.Substring(77079, 2443));
                variants[4] = ShaderVariables.Compile(device, sourceBundle.Substring(183278, 1316), sourceBundle.Substring(82306, 4225));
                variants[5] = ShaderVariables.Compile(device, sourceBundle.Substring(183278, 1316), sourceBundle.Substring(93361, 4279));
                variants[6] = ShaderVariables.Compile(device, sourceBundle.Substring(186251, 4243), sourceBundle.Substring(104524, 2433));
                variants[7] = ShaderVariables.Compile(device, sourceBundle.Substring(186251, 4243), sourceBundle.Substring(109731, 2487));
                variants[8] = ShaderVariables.Compile(device, sourceBundle.Substring(183278, 1316), sourceBundle.Substring(115046, 4375));
                variants[9] = ShaderVariables.Compile(device, sourceBundle.Substring(183278, 1316), sourceBundle.Substring(126401, 4429));
                variants[10] = ShaderVariables.Compile(device, sourceBundle.Substring(186251, 4243), sourceBundle.Substring(137864, 2583));
                variants[11] = ShaderVariables.Compile(device, sourceBundle.Substring(186251, 4243), sourceBundle.Substring(143371, 2637));
                variants[12] = ShaderVariables.Compile(device, sourceBundle.Substring(183278, 1316), sourceBundle.Substring(148986, 4419));
                variants[13] = ShaderVariables.Compile(device, sourceBundle.Substring(183278, 1316), sourceBundle.Substring(160429, 4473));
                variants[14] = ShaderVariables.Compile(device, sourceBundle.Substring(186251, 4243), sourceBundle.Substring(171980, 2627));
                variants[15] = ShaderVariables.Compile(device, sourceBundle.Substring(186251, 4243), sourceBundle.Substring(177575, 2681));
            }
        }
    }
}
