using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vbertz.PBSC
{
    public class Deep_Indicator : MonoBehaviour
    {

        public Physics_Based_Ship_Controller Ship;
        public Slider DeepIndicator;
        public Text DeepText;

        public void Update()
        {
            DeepIndicator.value = Ship.m_shipHeightIndex;
            DeepText.text = Ship.m_shipHeightIndex.ToString();
        }
    }
}