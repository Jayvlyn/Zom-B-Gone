using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Voice", menuName = "Enemies/Enemy Voice")]
public class EnemyVoice : ScriptableObject
{
    public List<AudioClip> passiveSounds;
    public List<AudioClip> hurtSounds;
    public List<AudioClip> deathSounds;
}
