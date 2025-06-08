using UnityEngine;

[CreateAssetMenu(fileName = "NewComboData", menuName = "Combat/Combo Data")] 
public class ComboData : ScriptableObject 
{ 
    public AnimationClip[] comboAnimationClips; 
}