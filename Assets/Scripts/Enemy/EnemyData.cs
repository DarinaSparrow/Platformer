using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyData {
    public Vector3 position;
    public Vector2 direction;
    public int health;
    public bool isAlive;
    public bool canMove;
    public bool isHit;
    public bool isDestroyed;
}

[Serializable]
public class EnemiesData {
    public List<EnemyData> enemies = new List<EnemyData>();
}