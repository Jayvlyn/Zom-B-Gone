using CodeMonkey;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("Disappearing")]
    [SerializeField,Tooltip("How long number sticks around before starting disappear")] 
    private float sustainTime = 0.4f;

    [SerializeField,Tooltip("How long number grows before sustain")] 
    private float growTime = 0.2f;

    [SerializeField,Tooltip("How fast number disappears after sustain")] 
    private float disappearSpeed = 5f;

    [Header("Font size")]
    [SerializeField] private int regularFontSize = 8;
    [SerializeField] private int criticalFontSize = 12;

    [Header("Scaling")]
    [SerializeField] private float increaseScaleAmount = 1;
    [SerializeField] private float decreaseScaleAmount = 1;

    [SerializeField,Tooltip("Should number 'land' by getting smaller at the end of grow")] 
    private bool land = false;

    [SerializeField,Range(0f, 1f),Tooltip("What percent of the grow should be the landing")] 
    private float landPercent = 0.4f;

    [SerializeField, Tooltip("Scale amount as landing happens")]
    private float landingDecreaseScaleAmount = 1;

    [Header("Movement")]
    [SerializeField,Tooltip("Direction and speed")] 
    private Vector3 initialMoveVector;
    [SerializeField,Tooltip("How fast the movement stops")] 
    private float moveReductionScalar = 8f;

    [SerializeField] private float growRotateAmount = 50;
    [SerializeField] private float disappearRotateAmount = -50;

    [Header("Color Override")]
    [SerializeField] private bool overrideRegularColor = false;
    [SerializeField] private Color newRegularColor = new Color(0, 0, 0, 1);
    [SerializeField] private bool overrideCriticalColor = false;
    [SerializeField] private Color newCriticalColor = new Color(0, 0, 0, 1);

    private static int sortingOrder;

    private TextMeshPro textMesh;
    private float growTimer;
    private float sustainTimer;

    private Color textColor;
    private string regularColorHex = "bf6b30";
    private string critColorHex = "9c0f0f";

    private Vector3 moveVector;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * moveReductionScalar * Time.deltaTime;

        if(growTimer > 0)
        { // count down grow timer first
            growTimer -= Time.deltaTime;

            if(land && growTimer < growTime * landPercent)
            { // last little percentage of grow
                transform.localScale -= Vector3.one * landingDecreaseScaleAmount * Time.deltaTime;
            }
            else
            {
                transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
                transform.Rotate(new Vector3(0, 0, 1), growRotateAmount * Time.deltaTime);
            }
        }
        else 
        { // grow timer finished, count down sustain
            sustainTimer -= Time.deltaTime;
            if (sustainTimer < 0)
            {
                Disappearing();
            }
        }
    }

    public static DamagePopup Create(Vector3 position, int damageAmount, Vector3 inputMoveVec = default, bool isCriticalHit = false, bool invertRotate = false)
    {
        Transform damagePopupT = Instantiate(Assets.i.damagePopup, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupT.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, inputMoveVec, isCriticalHit, invertRotate);

        return damagePopup;
    }

    public static DamagePopup Create(Vector3 position, float damageAmount, Vector3 inputMoveVec = default, bool isCriticalHit = false, bool invertRotate = false)
    {
        return Create(position,Mathf.RoundToInt(damageAmount), inputMoveVec, isCriticalHit, invertRotate);
    }

    public void Setup(int damageAmount, Vector3 inputMoveVec = default, bool isCriticalHit = false, bool invertRotate = false)
    {
        textMesh.SetText(damageAmount.ToString());

        if(isCriticalHit)
        { // Critical Hit

            // crit font size
            textMesh.fontSize = criticalFontSize;

            // crit color
            if (overrideCriticalColor) textColor = newCriticalColor;
            else textColor = UtilsClass.GetColorFromString(critColorHex);
        }
        else
        { // Regular Hit

            // reg font size
            textMesh.fontSize = regularFontSize;

            // reg color
            if (overrideRegularColor) textColor = newRegularColor;
            else textColor = UtilsClass.GetColorFromString(regularColorHex);
        }

        textMesh.color = textColor;

        growTimer = growTime;
        sustainTimer = sustainTime;

        if (inputMoveVec == default) moveVector = initialMoveVector;
        else moveVector = inputMoveVec;

        if(invertRotate)
        {
            growRotateAmount = -growRotateAmount;
            disappearRotateAmount = -disappearRotateAmount;
        }

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
    }

    private void Disappearing()
    {
        textColor.a -= disappearSpeed * Time.deltaTime;
        transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        transform.Rotate(new Vector3(0, 0, 1), disappearRotateAmount * Time.deltaTime);
        textMesh.color = textColor;
        if (textColor.a < 0)
        { // Destroy once text turns completely transparent
            Destroy(gameObject);
        }
    }
}
