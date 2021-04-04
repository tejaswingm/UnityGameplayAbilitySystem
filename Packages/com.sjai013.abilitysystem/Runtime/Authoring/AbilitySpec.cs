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
                    && CheckCooldown() <= 0;
        }

        public abstract void CancelAbility();
        public abstract bool CheckGameplayTags();
        public virtual float CheckCooldown()
        {
            if (this.AbilityScriptableObject.Cooldown == null) return 0;
            var cooldownTags = this.AbilityScriptableObject.Cooldown.gameplayEffectTags.GrantedTags;

            float longestCooldown = 0f;

            // Check if the cooldown tag is granted to the player, and if so, capture the remaining duration for that tag
            for (var i = 0; i < this.Owner.AppliedGameplayEffects.Count; i++)
            {
                var grantedTags = this.Owner.AppliedGameplayEffects[i].spec.GameplayEffect.gameplayEffectTags.GrantedTags;
                for (var iTag = 0; iTag < grantedTags.Length; iTag++)
                {
                    for (var iCooldownTag = 0; iCooldownTag < cooldownTags.Length; iCooldownTag++)
                    {
                        if (grantedTags[iTag] == cooldownTags[iCooldownTag])
                        {
                            // If this is an infinite GE, then return null to signify this is on CD
                            if (this.Owner.AppliedGameplayEffects[i].spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Infinite) return float.MaxValue;

                            var durationRemaining = this.Owner.AppliedGameplayEffects[i].spec.DurationRemaining;

                            if (durationRemaining > longestCooldown)
                            {
                                longestCooldown = durationRemaining;
                            }
                        }

                    }
                }
            }

            return longestCooldown;
        }

        protected abstract void PreActivate();
        protected abstract void ActivateAbility();
        public virtual bool CheckCost()
        {
            if (this.AbilityScriptableObject.Cost == null) return true;
            var geSpec = this.Owner.MakeOutgoingSpec(this.AbilityScriptableObject.Cost, this.Level);
            // If this isn't an instant cost, then assume it passes cooldown check
            if (geSpec.GameplayEffect.gameplayEffect.DurationPolicy != EDurationPolicy.Instant) return true;

            for (var i = 0; i < geSpec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = geSpec.GameplayEffect.gameplayEffect.Modifiers[i];

                // Only worry about additive.  Anything else passes.
                if (modifier.ModifierOperator != EAttributeModifier.Add) continue;
                var costValue = (modifier.ModifierMagnitude.CalculateMagnitude(geSpec) * modifier.Multiplier).GetValueOrDefault();

                this.Owner.AttributeSystem.GetAttributeValue(modifier.Attribute, out var attributeValue);

                // The total attribute after accounting for cost should be >= 0 for the cost check to succeed
                if (attributeValue.CurrentValue + costValue < 0) return false;

            }
            return true;
        }

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