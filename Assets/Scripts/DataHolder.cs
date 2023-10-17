using System;
using UnityEngine;
using System.IO;

namespace DefaultNamespace
{
    public class DataHolder : MonoBehaviour
    {
        [SerializeField] private string FolderName;
        private BoxPositionAtTrackerSideWithTwoBoxes _boxHolder;
        private string filename;
        private const string ExperimentNumber = "-Exp1";

        private void Start()
        {
            _boxHolder = FindObjectOfType<BoxPositionAtTrackerSideWithTwoBoxes>();
            filename = Application.dataPath + "/" + FolderName+ExperimentNumber + ".csv";
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("Box1Compliance, Box1Material, Box2Compliance,Box2Material,Answer");
            tw.Close();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1)|| Input.GetKeyDown(KeyCode.Keypad1))
                WriteCSV("1");
            else if(Input.GetKeyDown(KeyCode.Alpha2)|| Input.GetKeyDown(KeyCode.Keypad2))
                WriteCSV("2");
            
        }


        private void WriteCSV(string answer)
        {
            Debug.Log(answer + " Clicked");
            TextWriter tw = new StreamWriter(filename, true);
            var box1Compliance = GetComplianceString(_boxHolder.Boxes[0]);
            var box2Compliance = GetComplianceString(_boxHolder.Boxes[1]);
            var box1Material = GetMaterialName(_boxHolder.Boxes[0]);
            var box2Material = GetMaterialName(_boxHolder.Boxes[1]);
            tw.WriteLine(box1Compliance + "," + box1Material + "," + box2Compliance + "," + box2Material+","+ answer);
            tw.Close();
        }

        private String GetComplianceString(Box box)
        {
            if (box.BoxType == BoxTypeEnum.One)
            {
                return "One";
            }
            if (box.BoxType == BoxTypeEnum.Two)
            {
                return "Two";
            }
            if (box.BoxType == BoxTypeEnum.Three)
            {
                return "Three";
            }
            if (box.BoxType == BoxTypeEnum.Four)
            {
                return "Four";
            }
            if (box.BoxType == BoxTypeEnum.Five)
            {
                return "Five";
            }
            return null;
        }

        private String GetMaterialName(Box box)
        {
            return box.Renderer.material.name;
        }
    }
}