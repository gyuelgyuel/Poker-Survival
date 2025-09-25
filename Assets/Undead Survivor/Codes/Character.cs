using UnityEngine;

public class Character : MonoBehaviour
{
    public static float WeaponSpeed
    {
        get { return GameManager.instance.playerId == 0 ? 1f : 1.1f; }
    }

    public static float WeaponRate
    {
        get { return GameManager.instance.playerId == 0 ? 1f : 0.9f; }
    }
}
