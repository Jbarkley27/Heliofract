using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.HPBars
{
    public class FillOverTimeTransitionUpdateController : TransitionUpdateController
    {
        protected AnimationCurve curve;

        public FillOverTimeTransitionUpdateController(TransitionData data, TransitionUpdateManagerConfig config) : base(data, config)
        {
            this.curve = config.fillCurve;
        }

        protected override float Step()
        {
            return Mathf.Lerp(
                data.lastValue.FloatValue,
                data.targetValue.FloatValue,
                curve.Evaluate(data.percentage)
                );
        }
    }
}