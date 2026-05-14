using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.HPBars
{
    public class SpeedOverTimeTransitionUpdateController : TransitionUpdateController
    {
        public SpeedOverTimeTransitionUpdateController(TransitionData data, TransitionUpdateManagerConfig config) : base(data, config)
        {
            base.config.speedTimeCurveArea = MathUtils.IntegrateCurve(base.config.speedCurve, 0, 1, 100);
        }

        protected override float Step()
        {
            return Mathf.Lerp(
                data.lastValue.FloatValue,
                data.targetValue.FloatValue,
                MathUtils.IntegrateCurve(config.speedCurve, 0, data.percentage, 50) / config.speedTimeCurveArea
                );
        }
    }
}