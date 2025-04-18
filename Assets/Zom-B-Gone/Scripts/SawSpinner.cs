using UnityEngine;

public class SawSpinner : MonoBehaviour
{
    [SerializeField] private Transform sawT;
    private float rotSpeed = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(rotSpeed < 100)
        {
            rotSpeed += 10;
        }
    }

    private void Update()
    {
        if(rotSpeed > 0)
        {
            rotSpeed -= Time.deltaTime;
        }
        sawT.Rotate(0,0,rotSpeed);
    }
}