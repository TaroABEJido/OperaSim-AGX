﻿using AGXUnity;
using AGXUnity.Collide;
using AGXUnity.Model;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Providers.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Vector3 = UnityEngine.Vector3;


namespace PWRISimulator
{
    public class TerrainScore : MonoBehaviour
    {
        // エリア判定用の画像パス
        private const string WORK_FOLDER = "Assets/TerrainMaterial/";
        private const string image_file = "terrain_area.png";

        // 地形オブジェクト
        public GameObject obj;

        // 掘削エリアの枠からエリア座標を取得する
        public GameObject excFrame;
        // 放土エリアの枠からエリア座標を取得する
        public GameObject dmpFrame;

        // AGX
        //public DeformableTerrain terrain;
        //public DeformableTerrainShovel shovel;
        private DeformableTerrain terrain;
        private DeformableTerrainShovel shovel;

        // 掘削エリアのサイズ
        private const float excArea_sx = 10.0f;
        private const float excArea_sy = 5.0f;
        // 放土エリアのサイズ
        private const float dmpArea_sx = 10.0f;
        private const float dmpArea_sy = 5.0f;

        // test 20250902
        // 放土エリアのマージンサイズ
        private const float dmpAreaMargin_sx = 1.0f;
        private const float dmpAreaMargin_sy = 1.0f;


        // スコアリング
        private double init_sum;
        private double curt_sum;
        private double prev_sumDiff;

        private double init_excSum;
        private double curt_excSum;
        private double prev_excDiff;

        private double init_dmpSum;
        private double curt_dmpSum;
        private double prev_dmpDiff;


        // teat 20250902
        private double init_dmpSum_margin;
        private double curt_dmpSum_margin;
        private double prev_dmpDiff_margin;



        // スコア積算

        // 掘削エリアと放⼟エリアを除いたエリア
        private double sumScore;
        // 掘削エリア
        private double excScore;
        // 放土エリア
        private double dmpScore;


        public static double[,] CreateMatrix(int rows, int cols, double value)
        {
            double[,] matrix = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = value;
                }
            }
            return matrix;
        }

        public static byte[] readPngFile(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryReader bin = new BinaryReader(fileStream);
                byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);
                bin.Close();
                return values;
            }
        }

        public static Texture2D readByBinary(byte[] bytes, int t_resolution)
        {
            Texture2D texture = new Texture2D(t_resolution, t_resolution);
            texture.LoadImage(bytes);
            return texture;
        }

        public static void setColorMat(Texture2D tex, int t_resolution)
        {
            // 初期化
            var mat = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(CreateMatrix(t_resolution, t_resolution, 0));

            // ピクセル情報を取得する
            var colors = tex.GetPixels();

            // アンチエイリアスや画像の劣化の影響もあるため少し幅を持たせておく
            var offset = 5;

            // エリアの範囲確認
            var count_a1 = 0;
            var count_a2 = 0;
            var count_a3 = 0;
            var count_a4 = 0;
            var count_a5 = 0;
            var count_err = 0;

            // 画像のサイズ分ループを回し、色情報からエリアを識別し行列に格納
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    Color32 color = (Color32)colors[x + (y * tex.width)];

                    if (color.r < 255 + offset && color.r > 255 - offset &&
                        color.g < 255 + offset && color.g > 255 - offset &&
                        color.b < 255 + offset && color.b > 255 - offset)
                    {
                        // 走行可能エリア
                        mat[x, y] = 1.0;

                        count_a1 += 1;
                    }
                    else if (color.r < 127 + offset && color.r > 127 - offset &&
                        color.g < 127 + offset && color.g > 127 - offset &&
                        color.b < 127 + offset && color.b > 127 - offset)
                    {
                        // 進入不可エリア
                        mat[x, y] = 2.0;

                        count_a2 += 1;
                    }
                    else if (color.r < 12 + offset && color.r > 12 - offset &&
                        color.g < 212 + offset && color.g > 212 - offset &&
                        color.b < 252 + offset && color.b > 252 - offset)
                    {
                        // 掘削エリア
                        mat[x, y] = 3.0;

                        count_a3 += 1;

                    }
                    else if (color.r < 96 + offset && color.r > 96 - offset &&
                        color.g < 165 + offset && color.g > 165 - offset &&
                        color.b < 0 + offset && color.b > 0 - offset)
                    {
                        // 放土エリア
                        mat[x, y] = 4.0;

                        count_a4 += 1;
                    }
                    else if (color.r < 80 + offset && color.r > 80 - offset &&
                        color.g < 51 + offset && color.g > 51 - offset &&
                        color.b < 13 + offset && color.b > 13 - offset)
                    {
                        // 泥濘エリア
                        mat[x, y] = 5.0;

                        count_a5 += 1;
                    }
                    else
                    {
                        // 判定できない場合
                        mat[x, y] = 99.0;

                        count_err += 1;
                    }

                    //Debug.Log("mat[x,y] = " + mat[x, y] + ", x=" + x + ", y=" + y);
                    //Debug.Log("R:" + color.r + ", G:" + color.g + ", B:" + color.b);
                }
            }

            // 確認
            Debug.Log("走行エリア: " + count_a1);
            Debug.Log("進入禁止エリア: " + count_a2);
            Debug.Log("掘削エリア: " + count_a3);
            Debug.Log("放土エリア: " + count_a4);
            Debug.Log("泥濘エリア: " + count_a5);
            Debug.Log("識別不可: " + count_err);


            GlobalVariables.areaMat = mat;
        }

        public static Color getColor(Texture2D tex, int x, int y)
        {
            tex.ReadPixels(new Rect(x, y, 1, 1), 0, 0);
            Color color = tex.GetPixel(0, 0);
            return color;
        }


        //------------------------------

        private double calcSumExcArea(MathNet.Numerics.LinearAlgebra.Matrix<float> mat)
        {
            Vector3 excArea_pc = excFrame.transform.position;

            Vector2 excArea_p1 = new Vector2(excArea_pc[0] - excArea_sx * 0.5f, excArea_pc[2] - excArea_sy * 0.5f);
            Vector2 excArea_p2 = new Vector2(excArea_pc[0] + excArea_sx * 0.5f, excArea_pc[2] + excArea_sy * 0.5f);

            //Debug.Log("excArea_p1: " + excArea_p1 + ", excArea_p2: " + excArea_p2);

            // index を計算
            Vector2 excArea_p1_idx = new Vector2(excArea_p1[0] / (float)GlobalVariables.step_x, excArea_p1[1] / (float)GlobalVariables.step_z);
            Vector2 excArea_p2_idx = new Vector2(excArea_p2[0] / (float)GlobalVariables.step_x, excArea_p2[1] / (float)GlobalVariables.step_z);

            //Debug.Log("excArea_p1_idx: " + excArea_p1_idx + ", excArea_p2_idx: " + excArea_p2_idx);

            // terrainData.GetHeights で得られるheightのindexがy, x
            // 引数は rowStart、rowCount、columnStart、columnCount
            var subMatrix0 = mat.SubMatrix((int)excArea_p1_idx[1], (int)excArea_p2_idx[1] - (int)excArea_p1_idx[1],
                                           (int)excArea_p1_idx[0], (int)excArea_p2_idx[0] - (int)excArea_p1_idx[0]);

            double sum0 = subMatrix0.Enumerate().Sum();
            //Debug.Log("excArea sum: " + sum0);

            return sum0;
        }


        private double calcSumDmpArea(MathNet.Numerics.LinearAlgebra.Matrix<float> mat)
        {
            Vector3 dmpArea_pc = dmpFrame.transform.position;

            Vector2 dmpArea_p1 = new Vector2(dmpArea_pc[0] - dmpArea_sx * 0.5f, dmpArea_pc[2] - dmpArea_sy * 0.5f);
            Vector2 dmpArea_p2 = new Vector2(dmpArea_pc[0] + dmpArea_sx * 0.5f, dmpArea_pc[2] + dmpArea_sy * 0.5f);

            // index を計算
            Vector2 dmpArea_p1_idx = new Vector2(dmpArea_p1[0] / (float)GlobalVariables.step_x, dmpArea_p1[1] / (float)GlobalVariables.step_z);
            Vector2 dmpArea_p2_idx = new Vector2(dmpArea_p2[0] / (float)GlobalVariables.step_x, dmpArea_p2[1] / (float)GlobalVariables.step_z);

            // terrainData.GetHeights で得られるheightのindexがy, x
            // 引数は rowStart、rowCount、columnStart、columnCount
            var subMatrix1 = mat.SubMatrix((int)dmpArea_p1_idx[1], (int)dmpArea_p2_idx[1] - (int)dmpArea_p1_idx[1],
                                           (int)dmpArea_p1_idx[0], (int)dmpArea_p2_idx[0] - (int)dmpArea_p1_idx[0]);

            double sum1 = subMatrix1.Enumerate().Sum();
            //Debug.Log("dmpArea sum: " + sum1);

            return sum1;
        }


        private double calcSumDmpAreaMargin(MathNet.Numerics.LinearAlgebra.Matrix<float> mat)
        {
            Vector3 dmpArea_pc = dmpFrame.transform.position;

            Vector2 dmpAreaMargin_p1 = new Vector2(dmpArea_pc[0] - dmpArea_sx * 0.5f - dmpAreaMargin_sx, dmpArea_pc[2] - dmpArea_sy * 0.5f - dmpAreaMargin_sy);
            Vector2 dmpAreaMargin_p2 = new Vector2(dmpArea_pc[0] + dmpArea_sx * 0.5f + dmpAreaMargin_sx, dmpArea_pc[2] + dmpArea_sy * 0.5f + dmpAreaMargin_sy);

            // index を計算
            Vector2 dmpAreaMargin_p1_idx = new Vector2(dmpAreaMargin_p1[0] / (float)GlobalVariables.step_x, dmpAreaMargin_p1[1] / (float)GlobalVariables.step_z);
            Vector2 dmpAreaMargin_p2_idx = new Vector2(dmpAreaMargin_p2[0] / (float)GlobalVariables.step_x, dmpAreaMargin_p2[1] / (float)GlobalVariables.step_z);

            // terrainData.GetHeights で得られるheightのindexがy, x
            // 引数は rowStart、rowCount、columnStart、columnCount
            var subMatrix1 = mat.SubMatrix((int)dmpAreaMargin_p1_idx[1], (int)dmpAreaMargin_p2_idx[1] - (int)dmpAreaMargin_p1_idx[1],
                                           (int)dmpAreaMargin_p1_idx[0], (int)dmpAreaMargin_p2_idx[0] - (int)dmpAreaMargin_p1_idx[0]);

            double sum1 = subMatrix1.Enumerate().Sum();
            //Debug.Log("dmpArea sum: " + sum1);

            return sum1;
        }


        private IEnumerator wait()
        {
            // process pre-yield
            yield return new WaitForSeconds(0.01f);
        }


        // Start is called before the first frame update
        void Start()
        {
            // 地形取得
            TerrainData terrainData = obj.GetComponent<Terrain>().terrainData;

            // Terrain のサイズ
            Vector3 t_size = terrainData.size;

            // 解像度取得
            int t_resolution = terrainData.heightmapResolution;

            // ピクセル間の距離
            GlobalVariables.step_x = t_size[0] / t_resolution;
            GlobalVariables.step_z = t_size[2] / t_resolution;


            //---------------
            // エリアのスコアリング初期設定
            //---------------
            // 掘削エリアと放⼟エリアを除いたエリア
            sumScore = 0.0;
            // 掘削エリア
            excScore = 0.0;
            // 放土エリア
            dmpScore = 0.0;

            init_sum = 0.0;
            curt_sum = 0.0;
            prev_sumDiff = 0.0;

            init_excSum = 0.0;
            curt_excSum = 0.0;
            prev_excDiff = 0.0;

            init_dmpSum = 0.0;
            curt_dmpSum = 0.0;
            prev_dmpDiff = 0.0;

            init_dmpSum_margin = 0.0;
            curt_dmpSum_margin = 0.0;
            //prev_dmpDiff_margin = 0.0;


            // AGX
            terrain = null;
            shovel = null;

            if (terrain == null)
            {
                terrain = FindObjectOfType<DeformableTerrain>();
            }

            if (shovel == null)
            {
                shovel = FindObjectOfType<DeformableTerrainShovel>();
            }


            //---------------
            // エリア設定画像の読み込み
            //---------------
            // 泥濘エリアのカウント行列
            // メッシュを0.5mで計算
            GlobalVariables.countMat = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(CreateMatrix((int)(t_size[0] / 0.5), (int)(t_size[2] / 0.5), 0.0));

            // エリア判定行列
            GlobalVariables.areaMat = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(CreateMatrix(t_resolution, t_resolution, 0.0));

            // 画像読み込み
            byte[] imgData = readPngFile(Path.Combine(WORK_FOLDER, image_file));
            // Texture2D に型変換
            Texture2D tex = readByBinary(imgData, t_resolution);

            // エリア判定行列に値をセット
            setColorMat(tex, t_resolution);



            //---------------
            // スコアリングのため初期状態保持
            //---------------
            double sum = 0.0;

            // Heightmap
            float[,] curt_heights = terrainData.GetHeights(0, 0, t_resolution, t_resolution);

            var mat = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.DenseOfArray(curt_heights);

            sum = mat.Enumerate().Sum();
            //Debug.Log("sum: " + sum);

            // 掘削エリアのHeightmap行列のSum取得
            init_excSum = calcSumExcArea(mat);

            // 放土エリアのHeightmap行列のSum取得
            init_dmpSum = calcSumDmpArea(mat);

            // 放土エリアのマージン
            init_dmpSum_margin = calcSumDmpAreaMargin(mat) - init_dmpSum;


            // 掘削エリアと放土エリアを除いたHeightmap行列のSum取得
            init_sum = sum - init_excSum - init_dmpSum;

        }

        // Update is called once per frame
        void Update()
        {
            // 地形取得
            TerrainData terrainData = obj.GetComponent<Terrain>().terrainData;
            // 解像度取得
            int t_resolution = terrainData.heightmapResolution;


            //---------------
            // スコアリングのため現在の状態を取得し比較
            //---------------
            double sum = 0.0;

            // Heightmap
            float[,] curt_heights = terrainData.GetHeights(0, 0, t_resolution, t_resolution);

            var mat = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.DenseOfArray(curt_heights);

            sum = mat.Enumerate().Sum();
            //Debug.Log("sum: " + sum);


            // 掘削エリアのHeightmap行列のSum取得
            curt_excSum = calcSumExcArea(mat);

            // 放土エリアのHeightmap行列のSum取得
            curt_dmpSum = calcSumDmpArea(mat);

            // 放土エリアのマージン
            curt_dmpSum_margin = calcSumDmpAreaMargin(mat) - curt_dmpSum;


            // 掘削エリアと放土エリアを除いたHeightmap行列のSum取得
            curt_sum = sum - curt_excSum - curt_dmpSum;


            //Debug.Log(curt_sum - init_sum);
            //Debug.Log("curt_sum: " + curt_sum + ", init_sum: " + init_sum + ", d: " + (curt_sum - init_sum));





            // スコア計算
            double excDiff = curt_excSum - init_excSum;
            Debug.Log("excDiff: " + excDiff + ", curt_excSum: " + curt_excSum + ", init_excSum: " + init_excSum);

            if (excDiff < 0.0 && excDiff < prev_excDiff)
            {
                var _diff = excDiff - prev_excDiff;
                //excScore += GlobalVariables.MiningCoef * Math.Abs(excDiff) / 0.5;
                excScore += GlobalVariables.MiningCoef * Math.Abs(_diff);

                //Debug.Log("***** excScore: " + excScore);

                if (Math.Abs(excScore) >= 1.0)
                {
                    // スコア反映
                    GlobalVariables.incrementScore((int)excScore);

                    // スコア積算リセット
                    excScore = excScore - (int)excScore;

                    // 差分保持
                    prev_excDiff = excDiff;
                }
            }


            double dmpDiff = curt_dmpSum - init_dmpSum;
            Debug.Log("dmpDiff: " + dmpDiff + ", curt_dmpSum: " + curt_dmpSum + ", init_dmpSum: " + init_dmpSum);

            Debug.Log("curt_dmpSum_margin: " + curt_dmpSum_margin + ", init_dmpSum_margin: " + init_dmpSum_margin);


            if (dmpDiff > 0.0 && dmpDiff > prev_dmpDiff && init_dmpSum_margin - curt_dmpSum_margin <= 0)
            {
                var _diff = dmpDiff - prev_dmpDiff;
                //dmpScore += GlobalVariables.UnloadSoilCoef * Math.Abs(dmpDiff) / 0.1;
                dmpScore += GlobalVariables.UnloadSoilCoef * Math.Abs(_diff);

                Debug.Log("_diff: " + _diff + ", dmpDiff: " + dmpDiff + ", prev_dmpDiff: " + prev_dmpDiff);

                //Debug.Log("***** dmpScore: " + dmpScore);

                if (Math.Abs(dmpScore) >= 1.0)
                {
                    // スコア反映
                    GlobalVariables.incrementScore((int)dmpScore);

                    // スコア積算リセット
                    dmpScore = dmpScore - (int)dmpScore;

                    // 差分保持
                    prev_dmpDiff = dmpDiff;
                }
            }

            double sumDiff = curt_sum - init_sum;
            //Debug.Log("sumDiff: " + sumDiff + ",curt_sum: " + curt_sum + ", init_sum: " + init_sum);

            if (sumDiff > 0.0 && sumDiff > prev_sumDiff)
            {
                var _diff = sumDiff - prev_sumDiff;
                //sumScore += -1.0 * GlobalVariables.MiningCoef * Math.Abs(sumDiff) / 0.5;
                sumScore += -1.0 * GlobalVariables.MiningCoef * Math.Abs(_diff);

                //Debug.Log("***** sumScore: " + sumScore);

                if (Math.Abs(sumScore) >= 1.0)
                {
                    // スコア反映
                    GlobalVariables.incrementScore((int)sumScore);

                    // スコア積算リセット
                    sumScore = sumScore - (int)sumScore;

                    // 差分保持
                    prev_sumDiff = sumDiff;
                }
            }

            wait();
        }
    }
}
