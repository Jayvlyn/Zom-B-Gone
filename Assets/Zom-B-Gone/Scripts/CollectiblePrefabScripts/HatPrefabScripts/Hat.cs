using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Hat : Collectible
{
    [HideInInspector] public HatData hatData;


    public SpriteRenderer spriteRenderer;
    public bool useDataIcon = true;

    [Header("Optional:")]
    [Tooltip("Game Object that will get activated on wear, and deactivate for hiding and driving")] public GameObject activateOnWear;

    [HideInInspector] public Head head;
    [HideInInspector] public int lowerSortingLayerID;
    [HideInInspector] public int wornSortingLayerID;
    private float transferTime = .2f;

    private bool worn;
    public bool Worn
    {
        get { return worn; }
        set { worn = value; 
            if(activateOnWear)
            {
                activateOnWear.SetActive(worn);
            }
        }
    }

    private void Awake()
	{
        lowerSortingLayerID = SortingLayer.NameToID("LowerHat");
        wornSortingLayerID = SortingLayer.NameToID("WornHat");
        hatData = data as HatData;
        if (useDataIcon) spriteRenderer.sprite = hatData.icon;
	}

    public override void Interact(bool rightHand, PlayerController playerController)
    {
        base.Interact(rightHand, playerController);
        Worn = true;
		playerController.head.HatObject = gameObject;
		gameObject.transform.parent = playerController.head.gameObject.transform;
		head = playerController.head;
        gameObject.layer = LayerMask.NameToLayer("WornHat");

        ChangeSortingLayer(wornSortingLayerID);

        StartCoroutine(TransferPosition(head.hatTransform));

        if(hatData.camo)
        {
            playerController.gameObject.layer = LayerMask.NameToLayer("DisguisedPlayer");
        }
        else
        {
            playerController.gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

	public IEnumerator TransferPosition(Vector3 position, Quaternion rotation)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < transferTime)
        {
            transform.position = Vector3.Lerp(startPosition, position, elapsedTime / transferTime);
            transform.rotation = Quaternion.Lerp(startRotation, rotation, elapsedTime / transferTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = position;
        transform.rotation = rotation;
    }

    public IEnumerator TransferPosition(Transform target)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < transferTime)
        {
            transform.position = Vector3.Lerp(startPosition, target.position, elapsedTime / transferTime);
            transform.rotation = Quaternion.Lerp(startRotation, target.rotation, elapsedTime / transferTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;
        if(head) head.hairRenderer.enabled = hatData.showHair;
    }

    public void DropHat(Vector2? inputDropPosition = null)
    {
        if (head != null)
        {
			if (hatData.camo)
			{
				playerController.gameObject.layer = LayerMask.NameToLayer("Player");
			}

			Worn = false;
            head.HatObject = null;
            gameObject.transform.parent = null;
			gameObject.layer = LayerMask.NameToLayer("Interactable");
            ChangeSortingLayer(lowerSortingLayerID);

            Vector2 dropPos;

            if(inputDropPosition == null)
            {
                if(Utils.WallInFront(head.gameObject.transform)) dropPos = head.gameObject.transform.position - head.gameObject.transform.up;
                else dropPos = head.gameObject.transform.position + head.gameObject.transform.up;
            }
            else
            {
                dropPos = (Vector2)inputDropPosition;
            }

            StartCoroutine(TransferPosition(dropPos, transform.rotation));

            head = null;
		}
    }

    public void ChangeSortingLayer(int id)
    {
        spriteRenderer.sortingLayerID = id;

        if (transform.childCount > 0)
        {
            SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer childRenderer in childRenderers) childRenderer.sortingLayerID = id;
        }
    }
}
