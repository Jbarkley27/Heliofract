using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.HPBars
{
    public class PerFrameTransitionUpdateController : TransitionUpdateController
    {
        private float direction;
        private int maxFrames;
        private float difference;
        private float toAdd;


        public PerFrameTransitionUpdateController(TransitionData data, TransitionUpdateManagerConfig config) : base(data, config)
        {

        }

        public override float UpdatePercentage()
        {
            return Mathf.InverseLerp(
                maxFrames,
                0,
                data.timer
            );
        }

        public override void StartTransition ()
        {
            direction = data.Direction;

            switch (config.speedType)
            {
                case TransitionUpdateManagerConfig.speedFillTypeEnum.absolute:
                    data.timer = Mathf.Abs(data.targetValue.FloatValue - data.currentValue.FloatValue) / Mathf.Abs(config.absoluteSpeed);
                    break;
                case TransitionUpdateManagerConfig.speedFillTypeEnum.percentage:
                    data.timer = 1 / config.percentageSpeed;
                    break;
                default:
                    break;
            }
            maxFrames = (int) data.timer;
            data.TransitionStartEvent?.Invoke(data.targetValue);
        }

        public override void CheckUpdateAndStep()
        {
            if (data.currentValue.FloatValue == data.targetValue.FloatValue)
            {
                data.percentage = 1;
                data.timer = 0;
                return;
            }

            //timer counts the frames left

            if (data.timer != 0)
            {
                --data.timer;
            }
            else
            {
                
            }

            data.percentage = UpdatePercentage();
            data.currentValue.FloatValue = Step();
        }

        protected override float Step()
        {
            difference = data.targetValue.FloatValue - data.currentValue.FloatValue;
            switch (config.speedType)
            {
                case TransitionUpdateManagerConfig.speedFillTypeEnum.percentage:
                    toAdd = (direction * config.percentageSpeed);
                    break;
                    
                case TransitionUpdateManagerConfig.speedFillTypeEnum.absolute:
                    toAdd =  + (direction * config.absoluteSpeed);
                    break;
                default:
                    toAdd = 0;
                    break;
            }
            if (config.useDeltaTime)
            {
                toAdd *= Time.deltaTime;
            }

            if (Mathf.Abs(toAdd) > Mathf.Abs(difference))
            {
                return data.targetValue.FloatValue;
            }
            return Mathf.Max(0, data.currentValue.FloatValue + toAdd);
        }


    }
}