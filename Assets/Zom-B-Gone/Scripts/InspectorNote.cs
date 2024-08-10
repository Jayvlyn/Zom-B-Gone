using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class InspectorNote : MonoBehaviour
{
    [TextArea]
    [Tooltip("Doesn't do anything. Just comments shown in inspector")]
    public string Notes = "";
}
