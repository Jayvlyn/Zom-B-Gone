using System.Collections;
using UnityEngine;

public class UnitMusicSetter : MonoBehaviour
{
    public static UnitMusicSetter instance;

    public ZoneTrack brutusTrack;
    public ZoneTrack victorTrack;
    public ZoneTrack ratTrack;
    public ZoneTrack lockerTrack;

    void Start()
    {
        instance = this;
    }

	public void SwapTrack(int track)
	{
		switch (track)
		{
			case 0:
				MusicManager.instance.SetZoneTrack(brutusTrack);
				break;
			case 1:
				MusicManager.instance.SetZoneTrack(victorTrack);
				break;
			case 2:
				MusicManager.instance.SetZoneTrack(ratTrack);
				break;
			case 3:
				MusicManager.instance.SetZoneTrack(lockerTrack);
				break;
		}
	}

	public void SwapTrack(Tracks track)
    {
		switch (track)
		{
			case Tracks.BRUTUS:
				MusicManager.instance.SetZoneTrack(brutusTrack);
				break;
			case Tracks.VICTOR:
				MusicManager.instance.SetZoneTrack(victorTrack);
				break;
			case Tracks.RAT:
				MusicManager.instance.SetZoneTrack(ratTrack);
				break;
			case Tracks.LOCKER:
				MusicManager.instance.SetZoneTrack(lockerTrack);
				break;
		}
	}

	public enum Tracks
    {
        BRUTUS, VICTOR, RAT, LOCKER
    }

}
