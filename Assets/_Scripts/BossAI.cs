using System.Collections;
using UnityEngine;

[RequireComponent(typeof(StatsManager))]
public class BossAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] movePoints;

    [Header("Attacks")]
    [SerializeField] private GameObject bossProjectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float timeBetweenPatterns = 3f;

    private StatsManager stats;
    public CharacterStats_SO bossState;


    void Start()
    {
        stats = GetComponent<StatsManager>();
        stats.Initialize(bossState); 
        stats.OnDie.AddListener(() => GameManager.Instance.EndLevel(true)); // عند موت الزعيم، نفوز
        StartCoroutine(BossPatternRoutine());
    }

    private IEnumerator BossPatternRoutine()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            yield return StartCoroutine(MoveToRandomPoint());
            yield return StartCoroutine(CircularAttack());
            yield return new WaitForSeconds(timeBetweenPatterns);
        }
    }

    private IEnumerator MoveToRandomPoint()
    {
        if (movePoints.Length == 0) yield break;
        Vector3 targetPosition = movePoints[Random.Range(0, movePoints.Length)].position;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator CircularAttack()
    {
        if (bossProjectilePrefab == null || firePoint == null) yield break;
        int numberOfProjectiles = 12;
        float angleStep = 360f / numberOfProjectiles;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Instantiate(bossProjectilePrefab, firePoint.position, rotation);
        }
        yield return new WaitForSeconds(1f);
    }

    
}