using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.HPBars
{
    public class SimpleFillOverTimeTransitionUpdateController : TransitionUpdateController
    {
        protected AnimationCurve curve;

        public SimpleFillOverTimeTransitionUpdateController(TransitionData data, TransitionUpdateManagerConfig config) : base(data, config)
        {
            
        }

        protected override float Step()
        {
            return Mathf.Lerp(
                data.lastValue.FloatValue,
                data.targetValue.FloatValue,
                data.percentage
                );
        }
    }
}