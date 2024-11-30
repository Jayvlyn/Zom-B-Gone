using UnityEngine;

public class MusicSetter : MonoBehaviour
{
    void Start()
    {
        MusicManager.instance.SetZoneTrack(GameManager.currentZone.track);
    }
}
