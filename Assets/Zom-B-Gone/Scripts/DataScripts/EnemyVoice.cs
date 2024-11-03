using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Voice", menuName = "Enemies/Enemy Voice")]
public class EnemyVoice : ScriptableObject
{
    public List<AudioClip> droneSounds;
    public List<AudioClip> aggroSounds;
    public List<AudioClip> hurtSounds;
}
