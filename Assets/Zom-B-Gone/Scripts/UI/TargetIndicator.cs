using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    public Transform Target;
    public Transform playerT;
    public RectTransform targetImage;
    public Camera mainCamera;
    public float offset;

	public void Initialize(Transform target, Camera camera, float indicatorOffset)
	{
        Target = target;
        mainCamera = camera;
        offset = indicatorOffset;
	}

    public void UpdateTargetPosition()
    {
        if (Target != null)
        {
            targetImage.rotation = Target.rotation;

            Vector3 targetPosition = mainCamera.WorldToScreenPoint(Target.position);

            if (targetPosition.z >= 0 && !IsInCameraView(targetPosition))
            {
                transform.position = targetPosition;

				Vector2 direction = ((Vector2)targetPosition - (Vector2)playerT.position);
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

                ClampToScreen();
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else {
			gameObject.SetActive(false);
		}
    }

    bool IsInCameraView(Vector3 screenPosition)
    {
        return screenPosition.x > -200 && screenPosition.x < Screen.width + 200 &&
               screenPosition.y > -200 && screenPosition.y < Screen.height + 200;
    }

    void ClampToScreen()
    {
        Vector3 clampedPosition = transform.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, offset, Screen.width - offset);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, offset, Screen.height - offset);

        transform.position = clampedPosition;
    }
}
