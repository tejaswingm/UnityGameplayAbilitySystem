using GameplayTag.Authoring;

namespace AbilitySystem.Authoring
{
    public abstract class AbilitySpec
    {
        public AbstractAbilityScriptableObject AbilityScriptableObject;
        protected AbilitySystemCharacter Owner;
        public float Level;

        public AbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner)
        {
            this.AbilityScriptableObject = abilitySO;
            this.Owner = owner;
        }
        public virtual bool TryActivateAbility()
        {
            if (CanActivateAbility())
            {
                PreActivate();
                ActivateAbility();
                return true;
            }

            return false;
        }

        public virtual bool CanActivateAbility()
        {
            return CheckGameplayTags()
                    && CheckCost()
                    && CheckCooldown() == null;
        }

        public abstract void CancelAbility();
        public abstract bool CheckGameplayTags();
        public abstract float? CheckCooldown();
        protected abstract void PreActivate();
        protected abstract void ActivateAbility();
        public abstract bool CheckCost();

        protected virtual bool AscHasAllTags(AbilitySystemCharacter asc, GameplayTagScriptableObject[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!asc) return true;

            for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
            {
                var abilityTag = tags[iAbilityTag];

                bool requirementPassed = false;
                for (var iAsc = 0; iAsc < asc.AppliedGameplayEffects.Count; iAsc++)
                {
                    GameplayTagScriptableObject[] ascGrantedTags = asc.AppliedGameplayEffects[iAsc].spec.GameplayEffect.gameplayEffectTags.GrantedTags;
                    for (var iAscTag = 0; iAscTag < ascGrantedTags.Length; iAscTag++)
                    {
                        if (ascGrantedTags[iAscTag] == abilityTag)
                        {
                            requirementPassed = true;
                        }
                    }
                }
                // If any ability tag wasn't found, requirements failed
                if (!requirementPassed) return false;
            }
            return true;
        }

        protected virtual bool AscHasNoneTags(AbilitySystemCharacter asc, GameplayTagScriptableObject[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!asc) return true;

            for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
            {
                var abilityTag = tags[iAbilityTag];

                bool requirementPassed = true;
                for (var iAsc = 0; iAsc < asc.AppliedGameplayEffects.Count; iAsc++)
                {
                    GameplayTagScriptableObject[] ascGrantedTags = asc.AppliedGameplayEffects[iAsc].spec.GameplayEffect.gameplayEffectTags.GrantedTags;
                    for (var iAscTag = 0; iAscTag < ascGrantedTags.Length; iAscTag++)
                    {
                        if (ascGrantedTags[iAscTag] == abilityTag)
                        {
                            requirementPassed = false;
                        }
                    }
                }
                // If any ability tag wasn't found, requirements failed
                if (!requirementPassed) return false;
            }
            return true;
        }
    }

}