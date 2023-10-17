﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class BoxPositionAtTrackerSideWithTwoBoxes : MonoBehaviour
    {
        public List<Box> Boxes;
        public List<BoxCouple> BoxCoupleList;
        public int Index = 0;

        [SerializeField] private Transform Tracker;
        [SerializeField] private float DistanceBetweenBoxes = .2f;
        [SerializeField] private ArdityOneTrackerBoxFollowWithLeapMotion PositionFollower;
        private bool _isPositioned = false;

        private void Awake()
        {

            for (int i = 0; i < BoxCoupleList.Count; i++)
            {
                BoxCouple temp = BoxCoupleList[i];
                int randomIndex = Random.Range(i, BoxCoupleList.Count);
                BoxCoupleList[i] = BoxCoupleList[randomIndex];
                BoxCoupleList[randomIndex] = temp;
            }
            
            var boxCouple = new BoxCouple();
            boxCouple.BoxType1 = BoxTypeEnum.One;
            boxCouple.BoxMaterial1 = BoxCoupleList[0].BoxMaterial1;
            boxCouple.BoxType2 = BoxTypeEnum.One;
            boxCouple.BoxMaterial2 = BoxCoupleList[0].BoxMaterial1;
            BoxCoupleList.Insert(0,boxCouple);

            PositionFollower.Boxes.AddRange(Boxes);

        }

        private void Update()
        {
            Debug.DrawLine(Tracker.position, Tracker.position - Tracker.up*50, Color.red, 100);
            Debug.DrawLine(Tracker.position, Tracker.position - Tracker.right*50, Color.green, 100);
            if(!_isPositioned)
            {
                _isPositioned = true;
                for (int i = 0; i < Boxes.Count; i++)
                {
                    var pos = Tracker.position - (Tracker.up * (1f + i * DistanceBetweenBoxes)); //new Vector3(1f + i * DistanceBetweenBoxes, .01286f, 0.006f);
                    Boxes[i].transform.position = pos;
                }
                UpdateBoxes();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                UpdateBoxes();
                Debug.Log("Changed");
            }
        }

        public void UpdateBoxes()
        {
            if (Index >= BoxCoupleList.Count)
            {
                Debug.Log("All Cases Are Played");
                Index = 0;
            }

            var rand = Random.value;
            if (rand < .5f)
            {
                Boxes[0].UpdateBox(BoxCoupleList[Index].BoxType1,BoxCoupleList[Index].BoxMaterial1);
                Boxes[1].UpdateBox(BoxCoupleList[Index].BoxType2,BoxCoupleList[Index].BoxMaterial2);
            }
            else
            {
                Boxes[1].UpdateBox(BoxCoupleList[Index].BoxType1,BoxCoupleList[Index].BoxMaterial1);
                Boxes[0].UpdateBox(BoxCoupleList[Index].BoxType2,BoxCoupleList[Index].BoxMaterial2);
            }
            PositionFollower.ResetPrevBox();
            Index++;
        }
    }

    [Serializable]
    public class BoxCouple
    {
        public BoxTypeEnum BoxType1;
        public Material BoxMaterial1;
        public BoxTypeEnum BoxType2;
        public Material BoxMaterial2;
    }
}