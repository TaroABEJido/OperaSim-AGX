using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;

namespace PWRISimulator
{
    public class CountdownTimer : MonoBehaviour
    {

        private VisualElement root;
        private ProgressBar countPB;
        private string CountdownStr;
        //private float TimerCount = 180.0f;
        //private float timeRemaining;
        //private bool isRunning = true;

        public static float timeRemaining;
        public static bool isRunning = true;

        [SerializeField] private Color lowColor = Color.red;
        [SerializeField] private Color midColor = Color.yellow;
        [SerializeField] private Color highColor = Color.green;

        private void OnEnable()
        {
            root = this.GetComponent<UIDocument>().rootVisualElement;

            countPB = root.Q<ProgressBar>("CountdownProgress");
            //countPB.style.display = DisplayStyle.None;

            //timeRemaining = TimerCount;
            timeRemaining = GlobalVariables.GameTime;
            countPB.value = 1.0f;
            countPB.title = timeRemaining.ToString();

            isRunning = false;

            ResetTimer();

            //var PBC = root.Q(className: "unity-progress-bar__container");
            var backColor = new Color(0, 0, 0, 0);
            //PBC.style.backgroundColor = backColor;
            //PBC.style.borderTopColor = new Color(0, 0, 0, 0);
            //PBC.style.borderBottomColor = new Color(0, 0, 0, 0);
            //PBC.style.borderRightColor = new Color(0, 0, 0, 0);
            //PBC.style.borderLeftColor = new Color(0, 0, 0, 0);

            //var PBTC = root.Q(className: "unity-progress-bar__title-container");
            //PBTC.style.backgroundColor = backColor;
            //PBTC.style.borderTopColor = new Color(0, 0, 0, 0);
            //PBTC.style.borderBottomColor = new Color(0, 0, 0, 0);
            //PBTC.style.borderRightColor = new Color(0, 0, 0, 0);
            //PBTC.style.borderLeftColor = new Color(0, 0, 0, 0);

            var PBB = root.Q(className: "unity-progress-bar__background");
            PBB.style.backgroundColor = backColor;
            PBB.style.borderTopColor = new Color(0, 0, 0, 0);
            PBB.style.borderBottomColor = new Color(0, 0, 0, 0);
            PBB.style.borderRightColor = new Color(0, 0, 0, 0);
            PBB.style.borderLeftColor = new Color(0, 0, 0, 0);

            var PBP = root.Q(className: "unity-progress-bar__progress");
            PBP.style.height = 46;
            //PB1.style.width = 390;
            //countPB.style.display = DisplayStyle.None;


        }


        // Start is called before the first frame update
        void Start()
        {
            countPB = root.Q<ProgressBar>("CountdownProgress");

            //countPB.style.height = 100;

        }

        // Update is called once per frame
        void Update()
        {

            if (isRunning)
            {
                timeRemaining -= Time.deltaTime;
                var countPBTCL = root.Q<UnityEngine.UIElements.Label>("TimeCountLabel");
                //countPBTCL.text = Mathf.Max(timeRemaining, 0).ToString("F1");
                countPBTCL.text = CalcTimer(timeRemaining);

                countPB.value = (timeRemaining / GlobalVariables.GameTime) * 100.0f;

                UpdateColor(countPB.value);

                if (timeRemaining <= 0)
                {
                    isRunning = false;
                    countPBTCL.text = "Time's up!";
                    //var title = root.Q(className: "unity-progress-bar__title");
                    countPBTCL.style.color = lowColor;
                    countPB.value = 0.0f;

                    GlobalVariables.changeActionMode(-1);

                }

            }

        }


        public void StartTimer()
        {
            root = this.GetComponent<UIDocument>().rootVisualElement;
            countPB = root.Q<ProgressBar>("CountdownProgress");
            //countPB.style.display = DisplayStyle.Flex;
            countPB.value = 100.0f;

            var countPBTCL = root.Q<UnityEngine.UIElements.Label>("TimeCountLabel");
            //countPBTCL.text = Mathf.Max(timeRemaining, 0).ToString("F1");
            countPBTCL.text = CalcTimer(timeRemaining);

            isRunning = true;

        }

        public void StopTimer()
        {
            isRunning = false;
        }

        public void ResetTimer()
        {
            root = this.GetComponent<UIDocument>().rootVisualElement;
            countPB = root.Q<ProgressBar>("CountdownProgress");
            timeRemaining = GlobalVariables.GameTime;
            countPB.value = 100.0f;
            var countPBTCL = root.Q<UnityEngine.UIElements.Label>("TimeCountLabel");
            //countPBTCL.text = Mathf.Max(timeRemaining, 0).ToString("F1");
            countPBTCL.text = CalcTimer(timeRemaining);

            isRunning = false;

        }


        private void UpdateColor(float value)
        {

            var PB= root.Q(className: "unity-progress-bar__progress");


            if (value < GlobalVariables.TimeBarRedThreshold)
            {
                PB.style.backgroundColor = lowColor;
            }
            else if (value < GlobalVariables.TimeBarYellowThreshold)
            {
                PB.style.backgroundColor = midColor;
            }
            else
            {
                PB.style.backgroundColor = highColor;
            }

        }

        private string CalcTimer(float timedata)
        {

            string TimeStr;

            // 分、秒、ミリ秒を計算
            int minutes = Mathf.FloorToInt(timedata / 60f);
            int seconds = Mathf.FloorToInt(timedata % 60f);
            //int milliseconds = Mathf.FloorToInt((timedata * 1000f) % 1000f);

            // フォーマットして表示
            //TimeStr = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
            TimeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

            return TimeStr;

        }

    }
}
