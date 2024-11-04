using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FootstepManager : MonoBehaviour
{
    private Tilemap worldTilemap;
    public static Tilemap roomTilemap;

    [Header("Values")]
    public float baseStepInterval = .5f;
    public float speedMultiplier = 0.1f;
    private float stepTimer;

    [Header("Refs")]
    public PlayerController playerController;
    public AudioSource footstepAudioSource;
    public List<AudioClip> grassFootsteps;
    public List<AudioClip> tileFootsteps;
    public List<AudioClip> asphaultFootsteps;
    public List<AudioClip> woodenFootsteps;
    public List<AudioClip> carpetFootsteps;
    public List<AudioClip> concreteFootsteps;

    public List<TileBase> grassTiles;
    public List<TileBase> tileTiles;
    public List<TileBase> asphaultTiles;
    public List<TileBase> carpetTiles;
    public List<TileBase> woodenTiles;

    private void Start()
    {
        stepTimer = baseStepInterval;
        worldTilemap = GameObject.FindWithTag("WorldTilemap").GetComponent<Tilemap>();
    }

    private void Update()
    {
        float speed = playerController.rb.linearVelocity.magnitude;

        if (speed > 0.2)
        {
            stepTimer -= Time.deltaTime;

            float footstepInterval = baseStepInterval / (1 + (speed * speedMultiplier));

            if (stepTimer <= 0)
            {
                PlayFootstep();
                stepTimer = footstepInterval;
            }
        }
        else
        {
            stepTimer = baseStepInterval;
        }
    }

    public void PlayFootstep()
    {
        Tilemap chosenMap;

        if (roomTilemap != null)chosenMap = roomTilemap;
        else chosenMap = worldTilemap;
        
        Vector3Int cellPosition = chosenMap.WorldToCell((Vector2)transform.position);
        TileBase tile = chosenMap.GetTile(cellPosition);

        List<AudioClip> chosenAudioCollection = new List<AudioClip>();

        foreach (TileBase t in grassTiles)
            if (t == tile) chosenAudioCollection = grassFootsteps;

        if(chosenAudioCollection.Count == 0)
            foreach(TileBase t in asphaultTiles) 
                if (t == tile) chosenAudioCollection = asphaultFootsteps;

        if (chosenAudioCollection.Count == 0)
            foreach (TileBase t in woodenTiles)
                if (t == tile) chosenAudioCollection = woodenFootsteps;

        if (chosenAudioCollection.Count == 0)
            foreach (TileBase t in tileTiles)
                if (t == tile) chosenAudioCollection = tileFootsteps;

        if (chosenAudioCollection.Count == 0)
            foreach (TileBase t in carpetTiles)
                if (t == tile) chosenAudioCollection = carpetFootsteps;

        if (chosenAudioCollection.Count == 0)
            chosenAudioCollection = concreteFootsteps;


        // select random step from corresponding list
        int index = Random.Range(0, chosenAudioCollection.Count);

        // play one shot of chosen sound
        footstepAudioSource.PlayOneShot(chosenAudioCollection[index]);


        float footstepSoundRadius = 3;
        if(playerController.currentState == PlayerController.PlayerState.RUNNING) footstepSoundRadius *= 2;
        Utils.MakeSoundWave(playerController.transform.position, footstepSoundRadius, playerController.isSneaking);
    }    
}
