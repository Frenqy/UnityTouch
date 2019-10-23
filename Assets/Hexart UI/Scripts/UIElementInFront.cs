using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIC.Creator.UI
{
    public class UIElementInFront : MonoBehaviour
    {
        void Start()
        {
            this.transform.SetAsFirstSibling();
        }
    }
}