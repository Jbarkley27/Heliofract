using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.HPBars
{    
    public static class MathUtils
    {
        public enum RoundingType
        {
            Floor = 0,
            Round = 2,
            Ceil = 4
        }

        //Taken from https://answers.unity.com/questions/1259647/calculate-surface-under-a-curve-from-an-animationc.html
        // Integrate area under AnimationCurve between start and end time
        public static float IntegrateCurve(AnimationCurve curve, float startTime, float endTime, int steps)
        {
            return Integrate(curve.Evaluate, startTime, endTime, steps);
        }

        // Integrate function f(x) using the trapezoidal rule between x=x_low..x_high
        public static float Integrate(System.Func<float, float> f, float x_low, float x_high, int N_steps)
        {
            float h = (x_high - x_low) / N_steps;
            float res = (f(x_low) + f(x_high)) *0.5f;
            for (int i = 1; i < N_steps; i++)
            {
                res += f(x_low + i * h);
            }
            return h * res;
        }

        public static int AdjustFloatToInt (float floatValue, int maxValue, RoundingType roundingType)
        {
            float proportional = floatValue * maxValue;
            int adjusted;

            switch (roundingType)
            {
                case RoundingType.Floor:
                    adjusted = Mathf.FloorToInt(proportional);
                    break;
                case RoundingType.Round:
                    adjusted = Mathf.RoundToInt(proportional);
                    break;
                case RoundingType.Ceil:
                    adjusted = Mathf.CeilToInt(proportional);
                    break;
                default:
                    adjusted = (int)proportional;
                    break;
            }

            return adjusted;
        }

        public static float AdjustIntToFloat (int intValue, int maxValue)
        {
            if (maxValue <= 0)
            {
                Debug.LogError("Can't use this function when max value == 0");
                return 1;
            }
            intValue = Mathf.Clamp(intValue, 0, maxValue);
            return (float) intValue / maxValue;
        }

        public static float AdjustedRoundedFloat (float floatValue, int maxValue, RoundingType roundingType)
        {
            if (maxValue == 0)
            {
                return floatValue;
            }
            return AdjustIntToFloat(AdjustFloatToInt(floatValue, maxValue, roundingType), maxValue);
        }
    } 
}
