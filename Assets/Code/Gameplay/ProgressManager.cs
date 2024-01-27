using System;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    private void Start()
    {
        ProgressFlags._1_Showered = false;
        ProgressFlags._1_Breakfast = false;
        ProgressFlags._1_Work = false;
        ProgressFlags._1_Dinner = false;
        ProgressFlags._1_Sofa = false;
        ProgressFlags._1_Laugh = false;
    }
}