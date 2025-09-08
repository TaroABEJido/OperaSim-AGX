using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
# endif

namespace PWRISimulator
{
    public class saveScript : MonoBehaviour
    {
        private const string fileName = "SaveTerrain";

        private TerrainData terrainData;


        [System.Serializable]
        public class SaveData
        {
            public SerializedTerrain[] list;
        }

        [System.Serializable]
        public class SerializedTerrain
        {
            public float[] heights;
            public float[] alphas;

            // add
            public string name;
        }


        public void SerializeTerrain(string path)
        {
            double time = Time.realtimeSinceStartup;

            SaveData save = new SaveData();
            save.list = new SerializedTerrain[Terrain.activeTerrains.Length];

            int i = 0;

            // テレインデータを取得
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                Debug.Log(terrain.name);
                terrainData = terrain.terrainData;

                SerializedTerrain st = new SerializedTerrain();
                st.heights = ConvertToFlat(terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution));
                st.alphas = ConvertToFlat(terrainData.GetAlphamaps(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution));

                // add 
                st.name = terrain.name;

                save.list[i] = st;

                i++;
            }


            string json = JsonUtility.ToJson(save);

            StreamWriter writer = new StreamWriter(path + ".ter", false);
            writer.Write(json);
            writer.Close();

            Debug.Log("COMPLETED IN " + ((Time.realtimeSinceStartup - time)) + " " + json.Length);
        }

        private float[] ConvertToFlat(float[,] arr)
        {
            int len = arr.GetLength(0) * arr.GetLength(1);
            float[] ret = new float[len];
            int index = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int i2 = 0; i2 < arr.GetLength(1); i2++)
                {
                    ret[index] = arr[i, i2];
                    index++;
                }
            }
            return ret;
        }

        private float[,] ConvertFromFlat(float[] arr, int len1, int len2)
        {
            float[,] ret = new float[len1, len2];
            int index = 0;
            for (int i = 0; i < len1; i++)
            {
                for (int i2 = 0; i2 < len2; i2++)
                {
                    ret[i, i2] = arr[index];
                    index++;
                }
            }
            return ret;
        }

        private float[] ConvertToFlat(float[,,] arr)
        {
            int len = arr.GetLength(0) * arr.GetLength(1) * arr.GetLength(2); ;
            float[] ret = new float[len];
            int index = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int i2 = 0; i2 < arr.GetLength(1); i2++)
                {
                    for (int i3 = 0; i3 < arr.GetLength(2); i3++)
                    {
                        ret[index] = arr[i, i2, i3];
                        index++;
                    }
                }
            }
            return ret;
        }

        private float[,,] ConvertFromFlat(float[] arr, int len1, int len2, int len3)
        {

            float[,,] ret = new float[len1, len2, len3];
            int index = 0;
            for (int i = 0; i < len1; i++)
            {
                for (int i2 = 0; i2 < len2; i2++)
                {
                    for (int i3 = 0; i3 < len3; i3++)
                    {
                        ret[i, i2, i3] = arr[index];
                        index++;
                    }
                }
            }
            return ret;
        }


        // ボタンが押された場合に呼び出し
        public void OnClick()
        {
            SerializeTerrain(Path.Combine(GlobalVariables.BACKUP_FOLDER, fileName));
        }
    }
}