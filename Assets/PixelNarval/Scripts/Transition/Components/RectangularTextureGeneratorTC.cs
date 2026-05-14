using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelNarval.TextureGenerators;

namespace PixelNarval.HPBars
{
    [AddComponentMenu("PixelNarval/Transition/RectangularTextureGeneratorTC")]
    public class RectangularTextureGeneratorTC : TextureGeneratorTC
    {
        [SerializeField] private bool horizontalSections;     
    
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("RectangularTextureGeneratorTC", "sv_icon_name0");
        }
#endif

        protected override Texture2D Generate()
        {
            return SegmentedTextureGenerator.GenerateRectangularTexture(colorGradient, colorGradientType, resolution, filterMode, segmentNumber, segmentsSeparation, horizontalSections);
        }
    } 
}
