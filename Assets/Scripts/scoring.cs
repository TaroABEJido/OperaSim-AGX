using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace PWRISimulator
{
    public class Score : MonoBehaviour
    {


        public float timeOut = 500;
        private float timeElapsed;

        private VisualElement root;

        void OnEnable()
        {
            root = this.GetComponent<UIDocument>().rootVisualElement;

        }


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {

            timeElapsed += Time.deltaTime;

            if (timeElapsed >= timeOut)
            {


                if (GlobalVariables.ActionMode == 3)
                {
                    var Score = root.Q<UnityEngine.UIElements.Label>("Value");
                    Score.text = CalcScore().ToString();
                }

                timeElapsed = 0.0f;

                //UnityEngine.Debug.Log("Call");
            }

            //GlobalVariables.incrementScore(10);

        }


        private int CalcScore()
        {

            return GlobalVariables.score;
        }



    }
}
