using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWRISimulator
{
    public class RotateScript : MonoBehaviour
    {
        public GameObject textObj_1;
        public GameObject textObj_2;
        public GameObject textObj_3;
        public GameObject textObj_4;
        public GameObject textObj_5;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine("TextRotate");
        }

        // Update is called once per frame
        void Update()
        {
        }

        IEnumerator TextRotate()
        {
            //for (int turn = 0; turn < 180; turn++)
            while(true)
            {
                if (textObj_1 != null)
                {
                    textObj_1.transform.Rotate(0, -1, 0);
                }

                if (textObj_2 != null)
                {
                    textObj_2.transform.Rotate(0, -1, 0);
                }

                if (textObj_3 != null)
                {
                    textObj_3.transform.Rotate(0, -1, 0);
                }

                if (textObj_4 != null)
                {
                    textObj_4.transform.Rotate(0, -1, 0);
                }

                if (textObj_5 != null)
                {
                    textObj_5.transform.Rotate(0, -1, 0);
                }

                yield return new WaitForSeconds(0.025f);
            }
        }
    }
}
