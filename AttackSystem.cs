using UnityEngine;
using System.Collections;

public class AttackSystem : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack"; 
    [SerializeField] private string comboParameterName = "ComboStep"; 

    [Header("Combo Settings")]
    [SerializeField] private int maxComboSteps = 3;
    [SerializeField] private float comboResetTime = 0.8f; 
    [SerializeField] private float timeBetweenAttacks = 0.5f; 
    [SerializeField] private float globalCooldown = 1.5f; 

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

        if (isGlobalCooldown) return;

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
        
        currentComboStep = 0;
        animator.SetInteger(comboParameterName, 0);

        yield return new WaitForSeconds(globalCooldown);
        
        isGlobalCooldown = false;
        canAttack = true;
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
        
    }

    public void ResetCombo()
    {
        currentComboStep = 0;
        animator.SetInteger(comboParameterName, 0);
        animator.ResetTrigger(attackTriggerName);
    }

    public void FinalAttackReset()
    {
        if (currentComboStep == maxComboSteps)
        {
            ResetCombo();
        }
    }
}
