using System.Collections;
using System.Collections.Generic;
//using System.IO.
using UnityEngine.UI;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace VK.BikeLab
{
    /// <summary>
    /// ManualControl receives data from the BikeInput script and controls the BikeController script using apropriate methods.
    /// </summary>
    [RequireComponent(typeof(BikeInput))]
    public class ManualControl : MonoBehaviour
    {
        [Tooltip("Optional field. The slider visualizes user input along the X axis.")]
        public Slider sliderX;
        [Tooltip("Optional field. The slider visualizes the current steering angle.")]
        public Slider sliderSteer;
        [Tooltip("BikeController")]
        public BikeController bike;
        [Tooltip("Sets the [Time.timeScale.")]
        [Range(0.01f, 10)]
        public float timeScale;
        [Tooltip("Scale of X axis. If X axis = 1 velocity = maxVelocity.")]
        [Range(0, 200)]
        public float maxVelocity;
        [Space]
        [Tooltip("If true, balance is carried out automatically else the user must balance manually. In the last case, steering angle calculated as mix between user input and balanced steering angle + dumper.")]
        public bool fullAuto;
        [Tooltip("The interpolation value between user input and balanced steering angle.")]
        [Range(0, 1)]
        public float autoBalance;
        [Tooltip("Dumper factor.")]
        [Range(0, 1)]
        public float dumper;
        [Tooltip("These fields are calculated automatically at runtime.")]
        [Space]
        public Info info;
        //public BikeInput1 bikeInput1;

        private BikeInput bikeInput;
        private Rigidbody rb;
        private float goldVelocity;

        void Start()
        {
            //QualitySettings.vSyncCount = 0;
            //Application.targetFrameRate = 60;

            bike.Init();
            rb = bike.GetRigidbody();

            bikeInput = GetComponent<BikeInput>();

            goldVelocity = goldV();
        }
        private void FixedUpdate()
        {
            Time.timeScale = timeScale;
            setVelo();
            setSteer();

            info.currentLean = bike.getLean();
            info.currentSteer = bike.frontCollider.steerAngle;
            info.currentVelocity = rb.velocity.magnitude;
        }
        void Update()
        {
            bool pKey;
            bool rKey;
#if ENABLE_INPUT_SYSTEM
            pKey = Keyboard.current.pKey.wasPressedThisFrame;
            rKey = Keyboard.current.rKey.wasPressedThisFrame;
#else
        pKey = Input.GetKey(KeyCode.P);
        rKey = Input.GetKey(KeyCode.R);
#endif
            if (pKey)
                Debug.Break();
            if (rKey)
                bike.reset();

            if (sliderX != null)
                sliderX.value = bikeInput.xAxis + 0.5f;
            if (sliderSteer != null)
                sliderSteer.value = bike.frontCollider.steerAngle / bike.maxSteer + 0.5f;
        }
        private void setSteer()
        {
            float steer = bikeInput.xAxis * bike.maxSteer;
            //steer = Mathf.Clamp(steer, -bike.info.safeSteer, bike.info.safeSteer);
            //steer = roundAngle(steer, 4);
            info.targetSteer = steer;
            if (fullAuto)
                setAutoSteer();
            else
                setMixedSteer();
        }
        private void setVelo()
        {
            info.targetVelocity = bikeInput.yAxis * maxVelocity;
            Vector3 localV = transform.InverseTransformVector(rb.velocity);
            float diff = info.targetVelocity - localV.z;
            float a = Mathf.Clamp(diff * 1.0f, -10f, 10f);

            if (a > 0)
            {
                bike.setAcceleration(a);
                bike.safeBrake(0);
            }
            else
            {
                bike.setAcceleration(0);
                bike.safeBrake(-a);
            }
        }
        private void setAutoSteer()
        {
            if (rb.velocity.magnitude > goldVelocity)
                bike.setSteerByLean(info.targetSteer);
                //bike.setLean(-info.targetSteer / bike.maxSteer * bike.maxLean);
            else if (rb.velocity.magnitude < 1)
                bike.setSteer(0);
            else
                bike.setSteer(info.targetSteer);

        }
        private float setMixedSteer()
        {
            float balanceSteer = bike.GetBalanceSteer();
            if (rb.velocity.magnitude < 1)
                balanceSteer = 0;
            //float dumper = bike.damper() * (Mathf.Pow(localV.z * 1.0f, 0.4f) + 3);
            float dmp = getDumper() * dumper;
            float mix = Mathf.Lerp(info.targetSteer, balanceSteer, autoBalance);
            bike.setSteerDirectly(mix + dmp);
            return balanceSteer;
        }
        private float getDumper()
        {
            Vector3 av = transform.InverseTransformVector(rb.angularVelocity);
            float veloFactor = 1 / (rb.velocity.magnitude + 1);
            float lean = bike.getLean();
            float damper = -(av.z * 100 + lean * 1.3f) * veloFactor;
            damper = Mathf.Clamp(damper, -20, 20);
            return damper;
        }
        private float goldV()
        {
            float minD = 1000;
            float gold = 0;
            for (int i = 30; i < 60; i++)
            {
                float v = (float)i * 0.1f;
                float d = 0;
                //Debug.Log("******** v = " + v);
                for (int j = 0; j < 30; j++)
                {
                    float lean = (float)j;
                    float steer = bike.geometry.getSteer(-lean, v);
                    d += Mathf.Abs(lean - steer);
                    //Debug.Log("l = " + lean + "  s = " + steer);
                }
                if (d < minD)
                {
                    minD = d;
                    gold = v;
                }
            }
            //Debug.Log("gold = " + gold);
            return gold;
        }
        /// <summary>
        /// Rounds angle to part of Pi
        /// </summary>
        /// <param name="angle">angle</param>
        /// <param name="PIpart">Part of Pi. If PIpart = 4, angle will be rounded to Pi/4</param>
        /// <returns>Rounded angle</returns>
        private float roundAngle(float angle, int PIpart)
        {
            float pi = angle * Mathf.Deg2Rad / Mathf.PI;
            float r = Mathf.Round(pi * PIpart) / PIpart * Mathf.PI * Mathf.Rad2Deg;
            return r;
        }

        [System.Serializable]
        public class Info
        {
            [Space]
            [Range(-30, 30)] public float targetSteer;
            [Range(-30, 30)] public float currentSteer;
            [Space]
            [Range(-70, 70)] public float targetLean;
            [Range(-70, 70)] public float currentLean;
            [Tooltip("m/s")]
            [Space]
            [Range(0, 200)] public float targetVelocity;
            [Range(0, 200)] public float currentVelocity;
        }
    }
}