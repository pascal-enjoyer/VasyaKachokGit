using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AttackButtonManager : MonoBehaviour, IPointerDownHandler
{
    public Button attackButton;
    public CharacterCombat characterCombat;
    private float lastAttackTime;
    private float attackCooldown = 0.1f;
    private Queue<float> inputBuffer = new Queue<float>();
    private float bufferTime = 0.25f;

    private void Start()
    {
        if (attackButton == null)
        {
            Debug.LogError("AttackButton is not assigned!");
            enabled = false;
            return;
        }
        if (characterCombat == null)
        {
            Debug.LogError("CharacterCombat is not assigned!");
            enabled = false;
            return;
        }

        attackButton.interactable = true;
        Image buttonImage = attackButton.GetComponent<Image>();
        if (buttonImage != null)
            buttonImage.raycastPadding = new Vector4(-10, -10, -10, -10);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            inputBuffer.Enqueue(Time.time);
            return;
        }

        TryAttack();
        lastAttackTime = Time.time;
        TriggerButtonFeedback();
        Handheld.Vibrate();
    }

    private void Update()
    {
        while (inputBuffer.Count > 0 && Time.time - inputBuffer.Peek() <= bufferTime)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                TryAttack();
                lastAttackTime = Time.time;
                inputBuffer.Dequeue();
                TriggerButtonFeedback();
                Handheld.Vibrate();
            }
            else
            {
                break;
            }
        }

        while (inputBuffer.Count > 0 && Time.time - inputBuffer.Peek() > bufferTime)
            inputBuffer.Dequeue();
    }

    private void TryAttack()
    {
        if (characterCombat != null && characterCombat.HasWeapon())
        {
            IWeapon weapon = characterCombat.GetCurrentWeapon();
            if (weapon is WeaponBase)
            {
                characterCombat.UseWeapon();
            }
            else
            {
                Debug.LogWarning("Invalid weapon type!");
            }
        }
    }

    private void TriggerButtonFeedback()
    {
        if (attackButton != null)
        {
            Animator buttonAnimator = attackButton.GetComponent<Animator>();
            if (buttonAnimator != null)
            {
                buttonAnimator.SetTrigger("Press");
            }
            else
            {
                RectTransform rect = attackButton.GetComponent<RectTransform>();
                rect.localScale = Vector3.one * 0.9f;
                StartCoroutine(ResetScale(rect));
            }
        }
    }

    private System.Collections.IEnumerator ResetScale(RectTransform rect)
    {
        yield return new WaitForSeconds(0.1f);
        rect.localScale = Vector3.one;
    }
}