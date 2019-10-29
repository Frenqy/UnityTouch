using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VIC.Creator.Marker
{

    public class VirtualMarker : MonoBehaviour
    {
        public Text mkID;

        public void SetMkInfo(int id)
        {
            mkID.text = id.ToString();
        }
    }
}
