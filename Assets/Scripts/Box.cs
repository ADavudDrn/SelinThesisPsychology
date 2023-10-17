using System;
using System.Collections.Generic;
using DG.Tweening;
using Leap.Unity;
using ReferenceValue;
using UnityEngine;

namespace DefaultNamespace
{
    public class Box : MonoBehaviour
    {
        public Transform Model;
        public BoxTypeEnum BoxType;
        public List<Color> Colors;
        public List<Texture> Textures;
        public string SaveKey;
        [SerializeField]public SkinnedMeshRenderer Renderer;
        [SerializeField] private ParticleSystem ParticleSystem;
        private Vector3 _boxOffset;
        private Vector3 _initialRotation;

        private Tween _colorTween;

        private bool _isCalibrated;
        private Transform _headsetTransform;
        private Vector3 _modelLocalPos;
        private Transform _tracker;

        [SerializeField] private float HeadsetRotationLimit = 15f;
        [SerializeField] private float ModelTranslationLimitZ = 0.02f;
        [SerializeField] private float ModelTranslationLimitX = 0.03f;

        private void Awake()
        {
            _headsetTransform = FindObjectOfType<Camera>().transform;
            _tracker = FindObjectOfType<Tracker>().transform;
            Renderer.material.mainTextureScale = new Vector2(.25f, .25f);
        }

        private void Update()
        {
            if(!_isCalibrated)
                return;

            var difference = _initialRotation.y - _headsetTransform.eulerAngles.y;
            var posDifX = Mathf.Lerp(0f, ModelTranslationLimitX, Mathf.Abs(difference / HeadsetRotationLimit));
            var posDifZ = Mathf.Lerp(0f, ModelTranslationLimitZ, Mathf.Abs(difference/HeadsetRotationLimit));
            if (difference < 0)
            {
                posDifZ = -posDifZ;
            }

            Model.localPosition = _modelLocalPos + _tracker.up * posDifZ + _tracker.right * posDifX;
        }


        public void CalibrateBox(Transform indexTip)
        {
            Model.position = new Vector3(indexTip.position.x, indexTip.position.y - 0.045f, indexTip.position.z) - indexTip.right * .005f;
            _modelLocalPos = Model.localPosition;
            _boxOffset = Model.localPosition;
            _initialRotation = _headsetTransform.eulerAngles;
            _isCalibrated = true;
        }

        public void SetBoxLocal()
        {
            Model.localPosition = _boxOffset;
            _modelLocalPos = _boxOffset;
            _isCalibrated = true;
        }

        public void SetBoxVisible()
        {
            _colorTween?.Kill();
            _colorTween = Renderer.material.DOFade(1, 2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                ParticleSystem.Play();
            });
        }

        public void SetBoxInvisible()
        {
            _colorTween?.Kill();
            _colorTween = Renderer.material.DOFade(75f/255f, .5f).SetEase(Ease.InCubic);
            ResetDeform();
        }

        public void UpdateBox(BoxTypeEnum boxTypeEnum, Material material)
        {
            _isCalibrated = false;
            BoxType = boxTypeEnum;
            Renderer.material = material;
            Renderer.material.color = Renderer.material.color.WithAlpha(75f/255f);
            SetBoxLocal();
        }

        public void DeformBox(Transform indexTip)
        {
            var surfaceY = Model.position.y + .04f;
            var maxDepth = surfaceY - .0115f;
            var weight = Mathf.InverseLerp(surfaceY, maxDepth, indexTip.position.y) * 100;
            Renderer.SetBlendShapeWeight(0, weight);
        }

        public void ResetDeform()
        {
            Renderer.SetBlendShapeWeight(0, 0);
        }
    }
}