using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour
{
    public enum eType
    {
        ZONE,CHECKOUT,PARKING,OFFICE,NONE,INVALID
    }

    public eType type;
}
