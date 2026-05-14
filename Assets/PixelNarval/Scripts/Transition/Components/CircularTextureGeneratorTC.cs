using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelNarval.TextureGenerators;

namespace PixelNarval.HPBars
{
    [AddComponentMenu("PixelNarval/Transition/CircularTextureGeneratorTC")]
    public class CircularTextureGeneratorTC : TextureGeneratorTC
    {
        [SerializeField] private Vector2 center = new Vector2(0.5f, 0.5f);
        [Range(0, 2)] [SerializeField] private float externalRadius = 1;
        [Range(0, 2)] [SerializeField] private float internalRadius = 0.8f;
        [SerializeField] protected float originSegmentsSeparation = 5;
        [Range(0, 360)]
        [SerializeField] private float angleOffset;        

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("CircularTextureGeneratorTC", "Button Icon");
        }
#endif

        protected override Texture2D Generate()
        {
            return SegmentedTextureGenerator.GenerateCircularTexture(colorGradient, colorGradientType, resolution, center, externalRadius, internalRadius, angleOffset, filterMode, segmentNumber, originSegmentsSeparation, segmentsSeparation);
        }

    } 
}
