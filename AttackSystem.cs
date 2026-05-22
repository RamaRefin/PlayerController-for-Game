using UnityEngine;
using System.Collections;

public class AttackSystem : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack"; // Buat Trigger bernama "Attack" di Animator
    [SerializeField] private string comboParameterName = "ComboStep"; // Parameter Int

    [Header("Combo Settings")]
    [SerializeField] private int maxComboSteps = 3;
    [SerializeField] private float comboResetTime = 0.8f; // Waktu toleransi antar klik
    [SerializeField] private float timeBetweenAttacks = 0.5f; // Jeda antar serangan dalam kombo
    [SerializeField] private float globalCooldown = 1.5f; // Jeda setelah kombo selesai/reset

    private int currentComboStep = 0;
    private float lastAttackTime;
    private bool canAttack = true;
    private bool isGlobalCooldown = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Jangan baca input kalau lagi cooldown besar
        if (isGlobalCooldown) return;

        // Reset combo jika terlalu lama tidak memencet L
        if (Time.time - lastAttackTime > comboResetTime && currentComboStep > 0)
        {
            ResetCombo();
        }

        if (Input.GetKeyDown(KeyCode.L) && canAttack)
        {
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        canAttack = false; 
        lastAttackTime = Time.time;
        currentComboStep++;

        animator.SetInteger(comboParameterName, currentComboStep);
        animator.SetTrigger(attackTriggerName);

        // Jika sudah mencapai akhir kombo, aktifkan Global Cooldown
        if (currentComboStep >= maxComboSteps)
        {
            StartCoroutine(StartGlobalCooldown());
        }
        else
        {
            StartCoroutine(ResetAttackCooldown());
        }
    }

    IEnumerator StartGlobalCooldown()
    {
        isGlobalCooldown = true;
        Debug.Log($"Combo Finished! Global Cooldown for {globalCooldown}s...");
        
        // Reset parameter agar balik ke Idle
        currentComboStep = 0;
        animator.SetInteger(comboParameterName, 0);

        yield return new WaitForSeconds(globalCooldown);
        
        isGlobalCooldown = false;
        canAttack = true;
        Debug.Log("Global Cooldown Finished. Ready to attack!");
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
        Debug.Log("Can attack again!");
    }

    public void ResetCombo()
    {
        currentComboStep = 0;
        animator.SetInteger(comboParameterName, 0);
        animator.ResetTrigger(attackTriggerName);
        Debug.Log("Combo Reset to 0");
    }

    // Panggil ini lewat Animation Event HANYA jika ingin reset paksa di akhir animasi terakhir
    public void FinalAttackReset()
    {
        if (currentComboStep == maxComboSteps)
        {
            ResetCombo();
        }
    }
}
