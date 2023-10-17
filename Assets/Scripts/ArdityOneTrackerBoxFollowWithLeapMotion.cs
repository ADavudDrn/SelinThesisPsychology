using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using Valve.VR;
using Sirenix.OdinInspector;

namespace DefaultNamespace
{
    public class ArdityOneTrackerBoxFollowWithLeapMotion : MonoBehaviour
    {
        public SerialController serialController;

        public Transform IndexFinger;

        public SteamVR_Action_Boolean clickMove;
        public SteamVR_Action_Boolean clickCalibrate;

        public SteamVR_Input_Sources handType;
        
        public List<Box> Boxes = new List<Box>();
        
        [SerializeField] private TextMeshProUGUI FollowingText;
        [SerializeField] private bool UpdateToggle;
        [SerializeField] private GameObject Tracker;
        
        [SerializeField, FoldoutGroup("Constants")] private float StepToMMForZ = 15.385f;
        [SerializeField, FoldoutGroup("Constants")] private float StepToMMForX = 15.385f;

        [SerializeField, FoldoutGroup("Offsets")] private float XAxisOffset = -.5f;
        [SerializeField, FoldoutGroup("Offsets")] private float OffsetForOne = .067f;
        [SerializeField, FoldoutGroup("Offsets")] private float OffsetForTwo = .067f;
        [SerializeField, FoldoutGroup("Offsets")] private float OffsetForThree = 0;
        [SerializeField, FoldoutGroup("Offsets")] private float OffsetForFour = -.067f;
        [SerializeField, FoldoutGroup("Offsets")] private float OffsetForFive = -.067f;
        

        private Vector3 _trackerPos;
        private bool _isTrackerPosed;
        
        private Box _choosenBox;
        private Box _previousBox;

        // Start is called before the first frame update
        void Start()
        {
            serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
            clickMove.AddOnStateDownListener(ToggleFollow, handType);
            clickCalibrate.AddOnStateDownListener(CalibrateChoosenBox, handType);
            FollowingText.gameObject.SetActive(UpdateToggle);
        }

        // Update is called once per frame
        void Update()
        {
            if (!UpdateToggle)
                return;

            if (!_isTrackerPosed)
            {
                _isTrackerPosed = true;
                _trackerPos = Tracker.transform.position;
            }
            TrackerTracking();
        }


        public void ToggleFollow(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            UpdateToggle = !UpdateToggle;
            FollowingText.gameObject.SetActive(UpdateToggle);
        }

        public void CalibrateChoosenBox(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (!_choosenBox)
                return;
            _choosenBox.CalibrateBox(IndexFinger);
            Debug.Log(_choosenBox.name + " Calibrated");
        }

        public void TrackerTracking()
        {
            if (!IndexFinger.gameObject.activeInHierarchy)
                return;

            float prevDistance = 99999f;
            foreach (var box in Boxes)
            {
                var distance = (IndexFinger.position - box.transform.position).magnitude;

                if (distance < prevDistance)
                {
                    prevDistance = distance;
                    _choosenBox = box;
                }
            }

            var modelDistance = (IndexFinger.position - _choosenBox.Model.position).magnitude;
            if (modelDistance <= .05f)
            {
                _choosenBox.DeformBox(IndexFinger);
            }
            else
            {
                _choosenBox.ResetDeform();
            }

            //Debug.Log(_choosenBox.gameObject.name);
            var hapticDeviceStep = GetPosition();

            float PositionDifferenceZ;
            float PositionDifferenceX;

            float offset = 0;
            if (_choosenBox.BoxType == BoxTypeEnum.One)
                offset = OffsetForOne;

            else if (_choosenBox.BoxType == BoxTypeEnum.Two)
                offset = OffsetForTwo;

            else if (_choosenBox.BoxType == BoxTypeEnum.Three)
                offset = OffsetForThree;
            
            else if (_choosenBox.BoxType == BoxTypeEnum.Four)
                offset = OffsetForFour;
            
            else if (_choosenBox.BoxType == BoxTypeEnum.Five)
                offset = OffsetForFive;

            //Position difference in terms of steps
            PositionDifferenceZ =
                (Math.Abs(IndexFinger.position.z - _trackerPos.z) + offset) * 1000 *
                StepToMMForZ; //m * mm/m * step/mm = step       
            PositionDifferenceX =
                (Math.Abs(_choosenBox.transform.position.x - (_trackerPos.x + XAxisOffset)) + offset) *
                1000 *
                StepToMMForX; //m * mm/m * step/mm = step       
            if (_previousBox != _choosenBox)
            {
                _choosenBox.SetBoxVisible();
                if(_previousBox != null)
                    _previousBox.SetBoxInvisible();
                
                serialController.SendSerialMessage("s" + PositionDifferenceX + " " + PositionDifferenceZ + " " +
                                                   hapticDeviceStep);
                /*Debug.Log("<color=green> s" + PositionDifferenceX + " " + PositionDifferenceZ + " " +
                          hapticDeviceStep + _previousBox + _choosenBox + offset + "</color>");*/

                _previousBox = _choosenBox;
            }
        }

        private int GetPosition()
        {
            int step = 0;
            return step;
        }

        public void ResetPrevBox()
        {
            _previousBox = null;
        }
    }
}