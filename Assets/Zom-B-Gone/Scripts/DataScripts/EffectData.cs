using UnityEngine;

[CreateAssetMenu(fileName = "New EffectData", menuName = "New Effect")]
public class EffectData : ScriptableObject
{
    public Sprite effectSprite;
    public float effectDuration = 5;
    public float spreadDistance = 3; // dist to spread to nearby enemy
    public float spreadDuration = 1;
    [Range(0, 100)] public int spreadChance;

    public float speedMult = 1;
    public float onSpreadDamage = 0;
    public float passiveDamage = 0;
    public float passiveDamageTick = 1; // in seconds
    public float spreadTick = .5f; // in seconds
    public Color damageColor = new Color(0,1,1,1);
}
