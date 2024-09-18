using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Hat : MonoBehaviour, IInteractable
{
    [SerializeField] public HatData hatData;
    [HideInInspector] public Head head;
    private SpriteRenderer spriteRenderer;
    private float transferTime = .2f;
    public bool useDataIcon = true;

    private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
        if (useDataIcon) spriteRenderer.sprite = hatData.icon;
	}

    public void Interact(Head head)
    {
		head.HatObject = gameObject;
        spriteRenderer.sortingLayerName = "WornHat";
		gameObject.transform.parent = head.gameObject.transform;
		this.head = head;
        gameObject.layer = LayerMask.NameToLayer("WornHat");
        StartCoroutine(TransferPosition(head.hatTransform));
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
        head.hairRenderer.enabled = hatData.showHair;
    }

    public void DropHat()
    {
        if (head != null)
        {
            head.HatObject = null;
            gameObject.transform.parent = null;
			gameObject.layer = LayerMask.NameToLayer("Interactable");
			spriteRenderer.sortingLayerName = "GroundedHat";

            Vector2 dropPos;
            if(Utils.WallInFront(head.gameObject.transform)) dropPos = head.gameObject.transform.position - head.gameObject.transform.up;
            else dropPos = head.gameObject.transform.position + head.gameObject.transform.up;

            StartCoroutine(TransferPosition(dropPos, transform.rotation));

            head = null;
		}
    }

	public void Interact(bool rightHand)
    {
        throw new System.NotImplementedException();
    }
}
