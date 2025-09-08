using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
# if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
# endif

public class resetScript : MonoBehaviour
{
    private const string BACKUP_FOLDER = "Assets/Backup/";
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


    public void DeserializeTerrain(string path)
    {
        double time = Time.realtimeSinceStartup;

        // バイナリファイル読み込み
        StreamReader reader = new StreamReader(path + ".ter");
        string jsonString = reader.ReadToEnd();

        // 地形データの配列で扱う
        SaveData save = new SaveData();
        save = JsonUtility.FromJson<SaveData>(jsonString);

        //Debug.Log(save.list.Length);


        foreach (SerializedTerrain st in save.list)
        {
            // 同名のオブジェクトを検索
            GameObject obj = GameObject.Find(st.name);

            if (obj != null)
            {
                // 同名のオブジェクトが存在したら取得
                terrainData = obj.GetComponent<Terrain>().terrainData;

                // 地形読み込み
                terrainData.SetAlphamaps(0, 0, ConvertFromFlat(st.alphas, terrainData.alphamapResolution, terrainData.alphamapResolution, terrainData.alphamapLayers));
                terrainData.SetHeights(0, 0, ConvertFromFlat(st.heights, terrainData.heightmapResolution, terrainData.heightmapResolution));
            }
        }

        Debug.Log("COMPLETED IN " + ((Time.realtimeSinceStartup - time)) + " " + jsonString.Length);
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
        Debug.Log("Click!");

        string fileName = "default";

        DeserializeTerrain(Path.Combine(BACKUP_FOLDER, fileName));
    }
}
