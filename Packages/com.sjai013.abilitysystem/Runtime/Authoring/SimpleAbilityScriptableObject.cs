using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Simple Ability")]
    public class SimpleAbilityScriptableObject : AbstractAbilityScriptableObject
    {
        public GameplayEffectScriptableObject GameplayEffect;
        public override AbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new SimpleAbilitySpec(this, owner);
            spec.Level = owner.Level;
            return spec;
        }
    }

}