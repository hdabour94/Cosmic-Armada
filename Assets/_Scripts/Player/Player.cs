// Player.cs

using UnityEngine;

[RequireComponent(typeof(StatsManager))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerWeapon))]

public class Player : MonoBehaviour
{
    // مراجع للمكونات التي تحتاج إلى تهيئة
    private StatsManager statsManager;
    private PlayerWeapon playerWeapon;

        private PlayerController playerController; // أضف مرجعًا لوحدة التحكم
    // يمكنك إضافة أي مكونات أخرى هنا مستقبلاً

    private void Awake()
    {
        // جلب المكونات مرة واحدة في Awake
        statsManager = GetComponent<StatsManager>();
        playerWeapon = GetComponent<PlayerWeapon>();
                playerController = GetComponent<PlayerController>(); // احصل عليه في Awake
    }

    // هذه هي الدالة الرئيسية التي سيستدعيها LevelManager
    public void Initialize(ShipData_SO shipData)
    {
        if (shipData == null)
        {
            Debug.LogError("ShipData is null! Cannot initialize player.", this);
            return;
        }

        // --- الترتيب مهم جدًا هنا ---

        // 1. تهيئة الإحصائيات أولاً وقبل كل شيء
        // تأكد من أن ShipData_SO لديه مرجع لـ CharacterStats_SO
        if (shipData.stats != null)
        {
            statsManager.Initialize(shipData.stats);
            Debug.Log($"StatsManager initialized with {shipData.stats.name}. HP: {statsManager.CurrentHP}");
            playerController.SetMoveSpeed(shipData.stats.speed);
            Debug.Log($"PlayerController speed set to: {shipData.stats.speed}");
        }
        else
        {
            Debug.LogError($"CharacterStats_SO is missing in {shipData.name}!", shipData);
        }

        // 2. الآن بعد أن تم تهيئة الإحصائيات، قم بتهيئة المكونات الأخرى التي تعتمد عليها
        // سنقوم بتعديل PlayerWeapon ليحتوي على دالة Initialize
       // playerWeapon.Initialize(statsManager.BaseStats);
       // Debug.Log($"PlayerWeapon initialized with FireRate: {playerWeapon.settings.fireRate}");

        // 3. أي تهيئة مستقبلية تأتي هنا...
    }
}