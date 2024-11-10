using System.Drawing.Drawing2D;
using UnityEditor;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    public Transform hatTransform;
    [HideInInspector] public Hat wornHat;

    [HideInInspector] public GameObject hatObject;
    public GameObject HatObject
    {
        get { return hatObject; }
        set
        {
            hatObject = value;
            if (hatObject != null && hatObject.TryGetComponent(out Hat hat)) // Hat added or swapped
            {
                wornHat = hat;
                wornHat.ChangeSortingLayer(wornHat.wornSortingLayerID);
                if(wornHat.activateOnWear) wornHat.activateOnWear.SetActive(true);
            }
            else // Hat taken off
            {
                wornHat = null;
            }
        }
    }

    private void Awake()
    {
        if(GameManager.currentZoneLootTable != null && Random.Range(0,20) == 0)
        {
            HatData hatData = GameManager.currentZoneLootTable.GetRandomHat();
            if(hatData != null)
            {
                GameObject prefab = Resources.Load<GameObject>(hatData.name);
                HatObject = Instantiate(prefab, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
                hatObject.transform.parent = hatTransform;
                hatObject.transform.position = hatTransform.position;
                hatObject.transform.rotation = hatTransform.rotation;
                hatObject.layer = LayerMask.NameToLayer("WornHat");
            }
        }
    }

    public void RemoveHat()
    {
        if (wornHat)
        {
            wornHat.ChangeSortingLayer(wornHat.lowerSortingLayerID);
            hatObject.transform.parent = null;
            hatObject.layer = LayerMask.NameToLayer("Interactable");

            HatObject = null;
        }
    }
}
