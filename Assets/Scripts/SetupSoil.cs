using AGXUnity;
using AGXUnity.Collide;
using AGXUnity.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PWRISimulator
{
    public class SetupSoil : MonoBehaviour
    {
        private DeformableTerrain terrain;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // 重機の姿勢がロード完了
            if (GlobalVariables.SetupJointCompletedFlag && GlobalVariables.SetupJointDumpCompletedFlag) {

                // JSON形式で保存したデータを読込
                var json_ms = GlobalVariables.saveMachines;
                var json_ds = GlobalVariables.saveDumpSoil;
                var json = GlobalVariables.saveParticles;


                // 積載
                for (int i = 0; i < json_ds.data.Length; i++)
                {
                    GameObject dumpObj = GameObject.Find("ic120_" + json_ds.data[i].id);
                    if (dumpObj != null)
                    {
                        var ds = dumpObj.GetComponentInChildren<DumpSoil>();
                        ds.soilMass = json_ds.data[i].mass;
                        Debug.Log("id: " + json_ds.data[i].id + ", mass: " + ds.soilMass);
                    }
                }

                if (terrain == null)
                {
                    terrain = FindObjectOfType<DeformableTerrain>();
                }
                var soilSim = terrain.Native.getSoilSimulationInterface();

                for (int i = 0; i < json.data.Length; i++)
                {
                    // 掘削で生成されたモデルを再現
                    var set_pos = new agx.Vec3(json.data[i].position.x, json.data[i].position.y, json.data[i].position.z);
                    var set_vel = new agx.Vec3(json.data[i].velocity.x, json.data[i].velocity.y, json.data[i].velocity.z);
                    var set_rad = json.data[i].radius;

                    soilSim.createSoilParticle(set_rad, set_pos, set_vel);
                }

                // フラグを下す
                GlobalVariables.SetupJointFlag = false;
                GlobalVariables.SetupJointDumpFlag = false;

                GlobalVariables.SetupJointCompletedFlag = false;
                GlobalVariables.SetupJointDumpCompletedFlag = false;

                // すべて終えたら時刻とスコアをセット
                CountdownTimer.timeRemaining = json_ms.time;
                GlobalVariables.score = json_ms.score;



                var MessageDaialogUI = Resources.Load<GameObject>("Prefabs/MessageDialog");
                var MessageDaialogUIobj = Instantiate(MessageDaialogUI);
                var _uiMessageDaialogDocument = MessageDaialogUIobj.GetComponent<UIDocument>();

                var root = _uiMessageDaialogDocument.rootVisualElement;
                root.Q<UnityEngine.UIElements.Label>("Title").text = "Information";
                root.Q<UnityEngine.UIElements.Label>("Message").text = "Load completed.";
            }
        }
    }
}
