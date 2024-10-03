using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextClearer : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    public void ClearTextMesh()
    {
        inputField.text = "";
    }
}
