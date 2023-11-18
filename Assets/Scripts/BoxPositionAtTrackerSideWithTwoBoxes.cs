using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class BoxPositionAtTrackerSideWithTwoBoxes : MonoBehaviour
    {
        public List<Box> Boxes;
        public List<BoxCouple> BaseBoxCoupleList;
        public List<BoxCouple> BoxCoupleList;
        public int Index = 0;

        [SerializeField] private Transform Tracker;
        [SerializeField] private Transform Tracker2;
        [SerializeField] private float DistanceBetweenBoxes = .2f;
        [SerializeField] private ArdityOneTrackerBoxFollowWithLeapMotion PositionFollower;
        private bool _isPositioned = false;

        private void Awake()
        {
            for (int i = 0; i < 10; i++)
            {
                BoxCoupleList.AddRange(BaseBoxCoupleList);
            }
            for (int i = 0; i < BoxCoupleList.Count; i++)
            {
                BoxCouple temp = BoxCoupleList[i];
                int randomIndex = Random.Range(i, BoxCoupleList.Count);
                BoxCoupleList[i] = BoxCoupleList[randomIndex];
                BoxCoupleList[randomIndex] = temp;
            }
            
            var boxCouple = new BoxCouple();
            boxCouple.BoxType1 = BoxTypeEnum.Three;
            boxCouple.BoxMaterial1 = BoxCoupleList[0].BoxMaterial1;
            boxCouple.BoxType2 = BoxTypeEnum.Three;
            boxCouple.BoxMaterial2 = BoxCoupleList[0].BoxMaterial1;
            BoxCoupleList.Insert(0,boxCouple);

            PositionFollower.Boxes.AddRange(Boxes);

        }

        private void Update()
        {
            Debug.DrawLine(Tracker.position, Tracker2.position, Color.red, 100);
            if(!_isPositioned)
            {
                _isPositioned = true;
                for (int i = 0; i < Boxes.Count; i++)
                {
                    var dir = 
                        new Vector3(Tracker.position.x - Tracker2.position.x, 0,Tracker.position.z - Tracker2.position.z).normalized;
                    var pos = Tracker.position - (dir * (1f + i * DistanceBetweenBoxes)); //new Vector3(1f + i * DistanceBetweenBoxes, .01286f, 0.006f);
                    Boxes[i].transform.position = pos - new Vector3(0,0.05f,0);
                    Boxes[i].transform.right = dir;
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