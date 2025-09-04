using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace PWRISimulator
{
    public class ShovelScore : MonoBehaviour
    {
        // �ʒu��ێ�
        private int prevPosX;
        private int prevPosY;

        // �i���s�G���A�ɑ؍݂�������
        private float stayTime;


        private void scoringRestrictedAreas()
        {
            // �o�ߎ��Ԃ̉��Z
            stayTime += Time.deltaTime;

            Debug.Log("stayTime: " + stayTime);

            // �X�R�A�v�Z
            if ((int)stayTime >= 1)
            {
                // 1�b�ȏ�o�߂Ō��Z
                GlobalVariables.incrementScore((int)(GlobalVariables.OffTruckCoef * (int)stayTime));
                // �X�R�A�v�Z�������͌o�ߎ��Ԃ�������Ă���
                stayTime = stayTime - (int)stayTime;
            }
        }



        // Start is called before the first frame update
        void Start()
        {
            // ������
            stayTime = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (GlobalVariables.ActionMode == 3)
            {
                //Debug.Log("prevPosX: " + prevPosX + ", prevPosY: " + prevPosY + ", Object: " + this.gameObject);

                // ���ݒn���擾
                double Xpos = this.gameObject.transform.position.x;
                double Ypos = this.gameObject.transform.position.z;

                // �G���A�m�F
                int x_idx = (int)(Xpos / GlobalVariables.step_x);
                int z_idx = (int)(Ypos / GlobalVariables.step_z);

                int curtArea = (int)GlobalVariables.getAreaMat(x_idx, z_idx);

                //Debug.Log("curtArea: " + curtArea + ", Position: (" + Xpos + ", " + Ypos + ")");


                //--------------------
                // �G���A���Ƃ̏���
                //--------------------
                if (curtArea == 2)
                {
                    // �i���s�G���A
                    scoringRestrictedAreas();
                }

                // �i���s�G���A�łȂ��ꍇ�͌o�ߎ��Ԃ����Z�b�g
                if (curtArea != 2)
                {
                    stayTime = 0.0f;
                }
            }
        }
    }
}
