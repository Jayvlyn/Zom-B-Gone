using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    [CreateAssetMenu(fileName = "New Collectible Event", menuName = "Game Events/Collectible Event")]
    public class CollectibleEvent : BaseGameEvent<CollectibleData> { }
}
