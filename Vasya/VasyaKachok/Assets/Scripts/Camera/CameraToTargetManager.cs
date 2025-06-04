using UnityEngine;

public class CameraToTargetManager : MonoBehaviour
{
    public CharacterCombat characterCombat;
    public CameraController controller;


    public void Start()
    {
        if (characterCombat != null && controller != null)
        {
            Debug.Log("nigetr");
            characterCombat.WeaponOnTargetUsed.AddListener(controller.RotateTowardsTarget);
        }
    }


}
