using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.HPBars
{
    public abstract class TransitionUpdateController
    {
        protected float proportionalDuration;

        protected TransitionUpdateManagerConfig config;
        protected TransitionData data;


        public TransitionUpdateController(TransitionData data, TransitionUpdateManagerConfig config)
        {
            this.config = config;
            this.data = data;
        }

        float result;
        public virtual float UpdatePercentage()
        {            
            if (config.duration == 0)
            {
                return 1;
            }
            if (config.proportional)
            {
                result = Mathf.InverseLerp(
                    proportionalDuration,
                    0,
                    data.timer
                );
            }
            else
            {
                result = Mathf.InverseLerp(
                    config.duration,
                    0,
                    data.timer
                );
            }
            return result;
        }

        public virtual void StartTransition()
        {
            if (this.config.proportional)
            {
                proportionalDuration = this.config.duration * Mathf.Abs(this.data.targetValue.FloatValue - this.data.lastValue.FloatValue);
                data.timer = proportionalDuration;
            }
            else
            {
                data.timer = config.duration;
            }
            data.TransitionStartEvent?.Invoke(data.targetValue);
        }

        public virtual void CheckUpdateAndStep()
        {
            if (data.currentValue.FloatValue == data.targetValue.FloatValue)
            {
                data.percentage = 1;
                return;
            }

            if (data.timer != 0)
            {
                data.timer = Mathf.Max(0, data.timer - Time.deltaTime);
            }
            else
            {
                //Should have ended
                //StartTransition();
            }

            data.percentage = UpdatePercentage();
            data.currentValue.FloatValue = Step();
        }

        protected abstract float Step();
    }
}