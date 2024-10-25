using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Layout Collection", menuName = "New Layout Collection")]
public class FloorLayoutCollection : ScriptableObject
{
    public GameObject[] layouts = new GameObject[0];
}
