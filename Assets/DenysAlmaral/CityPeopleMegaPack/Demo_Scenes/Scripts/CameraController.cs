using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityPeople
{
    public class CameraController : MonoBehaviour
    {
        public AnimationCurve easingCurve;
        public float motionTimeTotal = 1.0f;
        public Camera[] cameraTargets;
        public GameObject TextLeft;
        public GameObject TextRight;
        public GameObject TextUp;
        public GameObject TextDown;
        public Material MaterialRef;
        public Texture[] Textures;

        private int currentCamera = 0;
        private int currentTexture = 0;
        private Vector3 targetPosition;
        private Vector3 startPosition;
        private float motionTime = 2.1f;

        void Start()
        {
            motionTime = motionTimeTotal + 0.1f;
            DisableArrows();
            var startCam = cameraTargets[currentCamera];
            if (startCam != null)
            {
                transform.position = cameraTargets[currentCamera].transform.position;
            }
        }

        void Update()
        {
            if (motionTime < motionTimeTotal)
            {
                motionTime = motionTime + Time.deltaTime;
                var normalizedTime = motionTime / motionTimeTotal;
                var easedTime = easingCurve.Evaluate(normalizedTime);
                transform.position = Vector3.Lerp(startPosition, targetPosition, easedTime);
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnRightClick();
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnLeftClick();
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnDownClick();
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnUpClick();
            }
        }

        void GoTo(Camera cam)
        {
            startPosition = transform.position;
            targetPosition = cam.transform.position;
            motionTime = 0;
        }

        public void OnLeftClick()
        {
            if (currentCamera > 0)
            {
                currentCamera--;
                var nextCam = cameraTargets[currentCamera];
                GoTo(nextCam);
                DisableArrows();
            }
        }

        public void OnRightClick()
        {
            if (currentCamera < cameraTargets.Length - 1)
            {
                currentCamera++;
                var nextCam = cameraTargets[currentCamera];
                GoTo(nextCam);
                DisableArrows();
            }
        }

        public void OnUpClick()
        {
            if (currentTexture > 0)
            {
                currentTexture--;
                if (MaterialRef != null)
                {
                    MaterialRef.SetTexture("_MainTex", Textures[currentTexture]);
                }
                DisableArrows();
            }
        }
        public void OnDownClick()
        {
            if (currentTexture < Textures.Length - 1)
            {
                currentTexture++;
                if (MaterialRef != null)
                {
                    MaterialRef.SetTexture("_MainTex", Textures[currentTexture]);
                }
                DisableArrows();
            }
        }

        private void DisableArrows()
        {
            if (TextRight != null && TextLeft != null)
            {
                TextRight.SetActive(currentCamera < cameraTargets.Length - 1);
                TextLeft.SetActive(currentCamera > 0);
            }
            if (TextUp != null && TextDown != null)
            {
                TextUp.SetActive(currentTexture > 0);
                TextDown.SetActive(currentTexture < Textures.Length - 1);
            }
        }
    }

}