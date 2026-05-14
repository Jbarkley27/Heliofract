using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.HPBars
{
    public class SequenceTestManager : MonoBehaviour
    {
        [SerializeField] private TransitionData fadeInData;
        [SerializeField] private List<TransitionData> transitionDatas;

        void Start()
        {
            if (transitionDatas.Count < 2)
            {
                return;
            }

            FadeIn();
        }

        private void FadeIn()
        {
            fadeInData.StartTransitionTo(0f, 1, (x) => StartFirstBar());
        }

        private void StartFirstBar()
        {
            transitionDatas[0].StartTransitionTo(0, 1.0f, (x) => StartSecondBar());
        }
        private void StartSecondBar()
        {
            transitionDatas[1].StartTransitionTo(0, 1.0f, (x) => StartThirdBar());
        }
        private void StartThirdBar()
        {
            transitionDatas[2].StartTransitionTo(0, 1.0f, (x) => FadeOut());
        }
        private void FadeOut()
        {
            fadeInData.StartTransitionTo(0f);
        }
    }
}
