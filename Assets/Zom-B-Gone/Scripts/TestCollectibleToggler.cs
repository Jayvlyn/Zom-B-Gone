using UnityEngine;

public class TestCollectibleToggler : MonoBehaviour
{
    public GameObject collectibleHolder;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            collectibleHolder.SetActive(!collectibleHolder.activeSelf);
        }
    }
}
