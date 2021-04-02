using System;
using UnityEngine;

namespace AbilitySystem.Authoring
{

    public abstract class AbstractAbilityScriptableObject : ScriptableObject
    {
        [SerializeField] private string AbilityName;
        /// <summary>
        /// Tags for this ability
        /// </summary>
        [SerializeField] public AbilityTags AbilityTags;

        [SerializeField] public GameplayEffectScriptableObject Cost;
        [SerializeField] public GameplayEffectScriptableObject Cooldown;

        public abstract AbilitySpec CreateSpec(AbilitySystemCharacter owner);
    }
}