using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using agxPowerLine;
using agx;

namespace PWRISimulator
{
    public class BucketAngleToCylinderLengthConvertor : LinkAngleToCylinderLengthConvertor
    {
        [SerializeField]
        public GameObject bucketPin; // Center of rotation
        public GameObject bucketEdge; 
        public GameObject cylinderRoot;
        public GameObject cylinderBindPoint;
        public GameObject armCylinderBindPoint;
        public GameObject armPin;
        [Tooltip("End point of the bucket side of the H link")]
        public GameObject HLinkRoot; // 
        [Tooltip("End point of the arm side of the I link")]
        public GameObject ILinkRoot; //

        private float bucketPinToHLinkRoot = 0.1f; // [m]       
        private float bucketPinToILinkRoot = 0.1f; // [m]    
        private float HLinkLength = 0.1f; // [m]             
        private float ILinkLength = 0.1f; // [m]             
        private float ILinkRootToCylinderRoot = 0.4f; // [m] 
        private float bucketPinTobucketEdge = 0.75f; // [m] 

        private float alpha = 0.5f; // [rad]
        private float beta = 2.0f; // [rad]
        private float kai = 0.0f; // [rad] 

        // ===== ここから Inspector 表示用（読み取り専用風） =====
        [Header("Runtime Debug (Inspector View)")]
        [SerializeField] private float dbg_alpha;
        [SerializeField] private float dbg_beta;
        [SerializeField] private float dbg_kai;
        [Space(4)]
        [SerializeField] private float dbg_bucketPinToILinkRoot;
        [SerializeField] private float dbg_bucketPinToHLinkRoot;
        [SerializeField] private float dbg_HLinkLength;
        [SerializeField] private float dbg_ILinkLength;
        [SerializeField] private float dbg_ILinkRootToCylinderRoot;
        [SerializeField] private float dbg_bucketPinTobucketEdge;

        // 追加：最後に評価した角度と、そのときのリンク長（任意）
        [Space(8)]
        [SerializeField] private float dbg_lastAngleRad;
        [SerializeField] private float dbg_lastLinkLength;

        // 追加：一括コピー用のテキストブロック
        [Space(10)]
        [Header("Copyable Debug Block")]
        [SerializeField, TextArea(6, 18)]
        private string dbg_copyBlock;

        // ===== 値更新をこの関数に集約（要求仕様） =====
        private void UpdateCopyBlock()
        {
            dbg_copyBlock = $@"{dbg_alpha:F6},{dbg_beta:F6},{dbg_kai:F6},{dbg_bucketPinToILinkRoot:F6},{dbg_bucketPinToHLinkRoot:F6},{dbg_HLinkLength:F6},{dbg_ILinkLength:F6},{dbg_ILinkRootToCylinderRoot:F6},{dbg_bucketPinTobucketEdge:F6}";
        }

        private void RefreshInspectorDebugValues()
        {
            if (bucketPin == null || bucketEdge == null || ILinkRoot == null || HLinkRoot == null ||
                cylinderBindPoint == null || cylinderRoot == null || armPin == null)
            {
                return;
            }

            // 元 DoStart と同等のジオメトリ更新（実値反映）
            Vector3 aa = bucketEdge.transform.position - bucketPin.transform.position;
            Vector3 bb = ILinkRoot.transform.position - bucketPin.transform.position;
            Vector3 cc = HLinkRoot.transform.position - bucketPin.transform.position;

            dbg_beta = Mathf.Deg2Rad * Vector3.Angle(aa, cc);
            dbg_bucketPinToILinkRoot = bb.magnitude;
            dbg_bucketPinToHLinkRoot = cc.magnitude;

            dbg_HLinkLength = (HLinkRoot.transform.position - cylinderBindPoint.transform.position).magnitude;
            dbg_ILinkLength = (ILinkRoot.transform.position - cylinderBindPoint.transform.position).magnitude;
            dbg_ILinkRootToCylinderRoot = (cylinderRoot.transform.position - ILinkRoot.transform.position).magnitude;
            dbg_bucketPinTobucketEdge = (bucketEdge.transform.position - bucketPin.transform.position).magnitude;

            // // alpha, kai（元コードに準拠）
            dbg_alpha = Mathf.PI - Mathf.Deg2Rad *
                    Vector3.Angle(cylinderRoot.transform.position - ILinkRoot.transform.position,
                                  bucketPin.transform.position - ILinkRoot.transform.position);
            dbg_kai = Mathf.Deg2Rad * Vector3.Angle(ILinkRoot.transform.position - bucketPin.transform.position,
                                                armPin.transform.position - bucketPin.transform.position);

            // // --- Inspector 表示用フィールドにコピー ---
            // dbg_alpha = alpha;
            // dbg_beta = beta;
            // dbg_kai = kai;
            // dbg_bucketPinToILinkRoot = bucketPinToILinkRoot;
            // dbg_bucketPinToHLinkRoot = bucketPinToHLinkRoot;
            // dbg_HLinkLength = HLinkLength;
            // dbg_ILinkLength = ILinkLength;
            // dbg_ILinkRootToCylinderRoot = ILinkRootToCylinderRoot;
            // dbg_bucketPinTobucketEdge = bucketPinTobucketEdge;
        }


        protected override void DoStart()
        {
            Vector3 a = bucketEdge.transform.position - bucketPin.transform.position;
            Vector3 b = ILinkRoot.transform.position - bucketPin.transform.position;
            Vector3 c = HLinkRoot.transform.position - bucketPin.transform.position;
            beta = Mathf.Deg2Rad * Vector3.Angle(a, c);
            bucketPinToILinkRoot = b.magnitude;
            bucketPinToHLinkRoot = c.magnitude;

            HLinkLength = (HLinkRoot.transform.position - cylinderBindPoint.transform.position).magnitude;
            ILinkLength = (ILinkRoot.transform.position - cylinderBindPoint.transform.position).magnitude;
            ILinkRootToCylinderRoot = (cylinderRoot.transform.position - ILinkRoot.transform.position).magnitude;
            bucketPinTobucketEdge = (bucketEdge.transform.position - bucketPin.transform.position).magnitude;

            // alpha = Mathf.Deg2Rad * Vector3.Angle(cylinderRoot.transform.position - ILinkRoot.transform.position, armCylinderBindPoint.transform.position - ILinkRoot.transform.position);
            alpha = Mathf.PI - Mathf.Deg2Rad * Vector3.Angle(cylinderRoot.transform.position - ILinkRoot.transform.position, bucketPin.transform.position - ILinkRoot.transform.position);
            kai = Mathf.Deg2Rad * Vector3.Angle( ILinkRoot.transform.position - bucketPin.transform.position, armPin.transform.position - bucketPin.transform.position);

            // ---- Debug.Log 追記 ----
            Debug.Log($"[DoStart] alpha={alpha:F6}");
            Debug.Log($"[DoStart] beta={beta:F6}");
            Debug.Log($"[DoStart] kai={kai:F6}");
            Debug.Log($"[DoStart] bucketPinToILinkRoot={bucketPinToILinkRoot:F6}, bucketPinToHLinkRoot={bucketPinToHLinkRoot:F6}");
            Debug.Log($"[DoStart] bucketPinToILinkRoot={bucketPinToILinkRoot:F6}, bucketPinToHLinkRoot={bucketPinToHLinkRoot:F6}");
            Debug.Log($"[DoStart] HLinkLength={HLinkLength:F6}, ILinkLength={ILinkLength:F6}, ILinkRootToCylinderRoot={ILinkRootToCylinderRoot:F6}, bucketPinTobucketEdge={bucketPinTobucketEdge:F6}");

            // Debug.Log($"[DoStart] a={a}, b={b}, c={c}");
            // Debug.Log($"[DoStart] beta(rad)={beta:F6}");
            // Debug.Log($"[DoStart] bucketPinToILinkRoot={bucketPinToILinkRoot:F6}, bucketPinToHLinkRoot={bucketPinToHLinkRoot:F6}");
            // Debug.Log($"[DoStart] HLinkLength={HLinkLength:F6}, ILinkLength={ILinkLength:F6}, ILinkRootToCylinderRoot={ILinkRootToCylinderRoot:F6}, bucketPinTobucketEdge={bucketPinTobucketEdge:F6}");
            // Debug.Log($"[DoStart] alpha(rad)={alpha:F6}");
            // Debug.Log($"[DoStart] kai(rad)={kai:F6}");
        }
        public override float CalculateCylinderRodTelescoping(float _angle)
        {
            float linkLen = CalculateCylinderLinkLength(_angle);
            float telescoping = linkLen - cylinderLength - cylinderRodDefaultLength;
            // ---- Debug.Log 追記 ----
            // Debug.Log($"[CalculateCylinderRodTelescoping] _angle(rad)={_angle:F6}");
            // Debug.Log($"[CalculateCylinderRodTelescoping] linkLen={linkLen:F6}, cylinderLength={cylinderLength:F6}, cylinderRodDefaultLength={cylinderRodDefaultLength:F6}");
            // Debug.Log($"[CalculateCylinderRodTelescoping] telescoping={telescoping:F6}");

            return CalculateCylinderLinkLength(_angle) - cylinderLength - cylinderRodDefaultLength;
        }

        protected override float CalculateCylinderLinkLength(float _angle)
        {
            // jointMaxAngle <->jointMinAngle の間で各リンク角度を制限
            if (Mathf.Deg2Rad * jointMaxAngle < _angle)
                _angle = Mathf.Deg2Rad * jointMaxAngle;
                
            else if (Mathf.Deg2Rad * jointMinAngle > _angle)
                _angle = Mathf.Deg2Rad * jointMinAngle;
                
            float gamma = beta - (_angle - kai); 
            float delta = Mathf.PI - gamma;
            float diagonal = Mathf.Sqrt( Mathf.Pow(bucketPinToILinkRoot, 2.0f) + Mathf.Pow(bucketPinToHLinkRoot, 2.0f) - 2 * bucketPinToILinkRoot * bucketPinToHLinkRoot * Mathf.Cos(delta));
            float eta = Mathf.Acos((Mathf.Pow(ILinkLength, 2.0f) + Mathf.Pow(diagonal, 2.0f) - Mathf.Pow(HLinkLength, 2.0f)) / (2 * ILinkLength * diagonal));
            float omega = Mathf.Acos( (Mathf.Pow(bucketPinToILinkRoot, 2.0f) + Mathf.Pow(diagonal, 2.0f) - Mathf.Pow(bucketPinToHLinkRoot, 2.0f)) / (2 * bucketPinToILinkRoot * diagonal) );
            float lamda = delta < Mathf.PI? eta + omega: eta - omega;
            float psi = Mathf.PI - alpha - lamda;

            RefreshInspectorDebugValues();
            UpdateCopyBlock();
            
            // ---- Debug.Log 追記 ----
            // Debug.Log($"[CalculateCylinderLinkLength] gameObject={name}");
            // Debug.Log($"[CalculateCylinderLinkLength] _angle(rad)={_angle:F6}, beta(rad)={beta:F6}, alpha(rad)={alpha:F6}");
            // Debug.Log($"[CalculateCylinderLinkLength] gamma={gamma:F6}, delta={delta:F6}, diagonal={diagonal:F6}");
            // Debug.Log($"[CalculateCylinderLinkLength] eta={eta:F6}, omega={omega:F6}, lamda={lamda:F6}, psi={psi:F6}");
            // Debug.Log($"[CalculateCylinderLinkLength] ILinkRootToCylinderRoot={ILinkRootToCylinderRoot:F6}, ILinkLength={ILinkLength:F6}");

            return Mathf.Sqrt( Mathf.Pow(ILinkRootToCylinderRoot, 2.0f) + Mathf.Pow(ILinkLength, 2.0f) - 2 * ILinkRootToCylinderRoot * ILinkLength * Mathf.Cos(psi)); 
        }

        public override float CalculateCylinderRodTelescopingVelocity(float _velocity)
        {
            // パターン1　バケットピンを中心とした回転運動に近似
            return bucketPinToHLinkRoot * _velocity;

            //// パターン2　移送法
            //// 瞬間中心を求める
            //Vector3 localBucketPin = armCylinderBindPoint.transform.InverseTransformPoint(bucketPin.transform.position);
            //Vector3 localHLinkRoot = armCylinderBindPoint.transform.InverseTransformPoint(HLinkRoot.transform.position);
            //Vector3 localcylinderBindPoint = armCylinderBindPoint.transform.InverseTransformPoint(cylinderBindPoint.transform.position);
            //Vector3 localILinkRoot = armCylinderBindPoint.transform.InverseTransformPoint(ILinkRoot.transform.position);

            //Vector2 localBucketPin_2d = new Vector2(localBucketPin.z, localBucketPin.y);
            //Vector2 localHLinkRoot_2d = new Vector2(localHLinkRoot.z, localHLinkRoot.y);
            //Vector2 localcylinderBindPoint_2d = new Vector2(localcylinderBindPoint.z, localcylinderBindPoint.y);
            //Vector2 localILinkRoot_2d = new Vector2(localILinkRoot.z, localILinkRoot.y);

            //// Step1.直線の方程式を求める y = ax + b
            //float a1 = (localcylinderBindPoint_2d.y - localILinkRoot_2d.y) / (localcylinderBindPoint_2d.x - localILinkRoot_2d.x);
            //float b1 = a1 * localcylinderBindPoint_2d.x - localcylinderBindPoint_2d.y;

            //float a2 = (localHLinkRoot_2d.y - localBucketPin_2d.y) / (localHLinkRoot_2d.x - localBucketPin_2d.x);
            //float b2 = a2 * localHLinkRoot_2d.x - localHLinkRoot_2d.y;

            //// Step2.直線の交点を求める. 交点 = 瞬間中心
            //float x = (b2 -b1) / (a1 - a2);
            //float y = x * a1 + b1;
            //Vector2 instantRotCentre = new Vector2(x, y);

            //// Step3.速度を求めたい点と瞬間中心との距離を計算
            //float dist1 = Vector2.Distance(instantRotCentre, localcylinderBindPoint_2d);
            //float dist2 = Vector2.Distance(instantRotCentre, localHLinkRoot_2d);

            //// Step4.バケットの回転角速度より、Hリンクの接続点の速度を算出
            //float hLinkRootVelocity = _velocity * bucketPinToHLinkRoot;

            //// Step5.シリンダ接続点の速度は瞬間中心からの距離の比となる
            //return hLinkRootVelocity * (dist1 / dist2);
        }

        public override float CalculateCylinderRodTelescopingForce(float _force)
        {
            float alpha_2 = Mathf.Deg2Rad * Vector3.Angle(HLinkRoot.transform.position - bucketPin.transform.position, cylinderBindPoint.transform.position - bucketPin.transform.position);
            float alpha_3 = Mathf.Deg2Rad * Vector3.Angle(bucketPin.transform.position - cylinderBindPoint.transform.position, ILinkRoot.transform.position - cylinderBindPoint.transform.position);
            float alpha_4 = Mathf.Deg2Rad * Vector3.Angle(bucketPin.transform.position - cylinderBindPoint.transform.position, HLinkRoot.transform.position - cylinderBindPoint.transform.position);

            float gamma = Mathf.Deg2Rad * Vector3.Angle(ILinkRoot.transform.position - cylinderBindPoint.transform.position, cylinderRoot.transform.position - cylinderBindPoint.transform.position);
            float required_torque = ((ILinkLength * Mathf.Sin(alpha_3 + alpha_4)) / (bucketPinToHLinkRoot * Mathf.Sin(alpha_2 + alpha_4))) * _force * bucketPinTobucketEdge;
            return (required_torque / ILinkLength) / Mathf.Sin(gamma);
        }


    }
}
