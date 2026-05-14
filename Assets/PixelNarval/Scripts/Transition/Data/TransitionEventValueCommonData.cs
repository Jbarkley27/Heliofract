using UnityEngine;

namespace PixelNarval.HPBars
{
    [System.Serializable]
    public class TransitionEventValueCommonData
    {
        [Min(0)] public int maxValue;
        public MathUtils.RoundingType roundingType;
        public bool snapToInt;
    }
}
