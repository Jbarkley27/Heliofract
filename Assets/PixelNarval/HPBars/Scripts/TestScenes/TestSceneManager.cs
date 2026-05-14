using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    public class TestSceneManager : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Toggle automaticToggle;
        [SerializeField] private Transform barParentTransform;
        [SerializeField] private List<TransitionData> bars;

        private Coroutine coroutine;
        [SerializeField] private int waitingFor;
        [SerializeField] private float restTime = 1;

        void Start()
        {
            if (barParentTransform != null)
            {
                bars = new List<TransitionData>();
                //bars = new List<TransitionData>(barParentTransform.GetComponentsInChildren<TransitionData>());
                foreach (Transform item in barParentTransform)
                {
                    TransitionData component = item.GetComponent<TransitionData>();
                    if (component != null && component.isActiveAndEnabled)
                    {
                        bars.Add(component);
                    }
                }

                foreach (TransitionData item in bars)
                {
                    slider.onValueChanged.AddListener((float v) =>
                    {
                        if (!automaticToggle.isOn)
                        {
                            item.StartTransitionTo(v);
                        }
                    });
                    item.TransitionEndEvent.Subscribe(OnTransitionFinished);
                }

                automaticToggle.onValueChanged.AddListener(OnAutomaticTogglePressed);
                if (automaticToggle.isOn)
                {
                    slider.interactable = false;
                    coroutine = StartCoroutine(StartTransition());
                }
                
            }
        }

        private void OnDestroy()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            
        }

        public void OnTransitionFinished (TransitionEventValue _)
        {
            if (automaticToggle.isOn)
            {
                --waitingFor;

            }
        }

        public void OnAutomaticTogglePressed (bool on)
        {
            slider.interactable = !on;
            if (on)
            {
                coroutine = StartCoroutine(StartTransition());
            }
            else
            {
                waitingFor = 0;
                StopCoroutine(coroutine);
            }
        }


        private IEnumerator StartTransition()
        {
            while (true)
            {

                float randomValue = Random.Range(0f, 1f);
                foreach (TransitionData item in bars)
                {
                    item.StartTransitionTo(randomValue);
                    ++waitingFor;
                }
                slider.value = randomValue;


                while (waitingFor > 0)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(restTime);
            }
            
        }
    }
}