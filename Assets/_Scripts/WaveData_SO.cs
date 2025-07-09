using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public EnemyData_SO enemyData; // أي نوع من الأعداء سيظهر
    public float delayBeforeSpawn; // كم ثانية ننتظر قبل توليد هذا العدو

  //  public GameObject formationPrefab;

}

[CreateAssetMenu(fileName = "New Wave Data", menuName = "Enemies/Wave Data")]
public class WaveData_SO : ScriptableObject
{
    public EnemySpawnInfo[] enemiesToSpawn; // قائمة الأعداء في هذه الموجة
        public GameObject formationPrefab;

}