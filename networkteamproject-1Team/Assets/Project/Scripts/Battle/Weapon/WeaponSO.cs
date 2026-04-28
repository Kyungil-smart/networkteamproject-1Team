using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public string Name;
    public int damage;
    public float range;
    public float cooltime;

    // 총기류 확장시 사용
    // public int maxAmmo;
    // public int magCapacity;
    // public float reloadTime;

    // 연출
    public AudioResource attackClip;
    //public AudioResource reloadClip;
}
