using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    public Transform hatTransform;
    [SerializeField] private List<Sprite> possibleSprites;
    public SpriteRenderer spriteRenderer;
    [HideInInspector] public Hat wornHat;
    [HideInInspector] public Enemy owner;
    [HideInInspector] public Rigidbody2D rb;
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
        rb = GetComponent<Rigidbody2D>();
        if (transform.parent.gameObject.TryGetComponent(out Enemy owner))
        {
            this.owner = owner;
        }

        spriteRenderer.sprite = possibleSprites[UnityEngine.Random.Range(0, possibleSprites.Count)];

        if (GameManager.currentZone && GameManager.currentZone.lootTable != null && UnityEngine.Random.Range(0,20) == 0)
        {
            HatData hatData = GameManager.currentZone.lootTable.GetRandomHat();
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

    private void Update()
    {
        if (detached)
        {
            if (detachedTimer > 0) detachedTimer -= Time.deltaTime;
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, disappearSpeed * Time.deltaTime);
                detachBleeding.transform.localScale = Vector3.Lerp(detachBleeding.transform.localScale, Vector3.zero, disappearSpeed * Time.deltaTime);
                if (transform.localScale.x < 0.3)
                {
                    Destroy(gameObject);
                }
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
    public void RemoveHat(Vector2 newPosition)
    {
        if (wornHat)
        {
            wornHat.ChangeSortingLayer(wornHat.lowerSortingLayerID);
            hatObject.transform.parent = null;
            hatObject.layer = LayerMask.NameToLayer("Interactable");

            wornHat.TransferPosition(newPosition, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));

            HatObject = null;
        }
    }

    private float detachedLifetime = 1.0f;
    private float disappearSpeed = 8.0f;
    private bool detached = false;
    private float detachedTimer;
    [SerializeField] private Transform detachPoint;
    [SerializeField] private Transform attachPoint;
    [SerializeField] private GameObject bleedParticles;
    private GameObject detachBleeding;
    public void DetachFromOwner()
    {
        if(wornHat)
        {
            float xOffset = UnityEngine.Random.Range(-2, 2);
            float yOffset = UnityEngine.Random.Range(-2, 2);
            Vector2 pos = new Vector2(transform.position.x + xOffset, transform.position.x + yOffset);
            RemoveHat(pos);
        }
        detachBleeding = Instantiate(bleedParticles, detachPoint);
        detachBleeding.transform.position = detachPoint.position;
        GameObject attachBleeding = Instantiate(bleedParticles, attachPoint);
        attachBleeding.transform.position = attachPoint.position;
        //owner.bleedingParticles.Add(attachBleeding);
        if (transform.parent) transform.parent = null;

        detachedTimer = detachedLifetime;
        detached = true;
    }
}
