using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public List<Transform> patrolPoints = new List<Transform>();
    public float speed;
    public Vector3 offset;
    public float chaseRange;
    public float attackRange;
}
