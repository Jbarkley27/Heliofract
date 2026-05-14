using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.HPBars
{
    [System.Serializable]
    public class TransitionUpdateManagerConfig
    {

        public bool advancedConfig;
        public enum fillTypeEnum { perTime, perFrame }
        [Tooltip("Set the amount filled per time or set a fixed filling speed")]
        public fillTypeEnum fillType;
        public enum timeFillTypeEnum { speedOverTime, fillOverTime }
        [Tooltip("Set the speed over time or the amount over time instead")]
        public timeFillTypeEnum timeFllType;
        public enum speedFillTypeEnum { absolute, percentage }
        [Tooltip("The speed should be applied to a fixed amount or a fixed percentage of the max value")]
        public speedFillTypeEnum speedType;
        [Min(0)]
        public float duration = 2;
        [Tooltip("Should the speed be proportional to the amount gained or lost?")]
        public bool proportional;
        public AnimationCurve speedCurve = AnimationCurve.Linear(0, 1, 1, 1);
        public AnimationCurve fillCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float absoluteSpeed;
        public bool useDeltaTime;

        [Range(0.0001f, 1f)]
        public float percentageSpeed = 0.005f;
        public float speedTimeCurveArea;
    }
}