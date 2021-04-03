namespace AbilitySystem.Authoring
{
    public class SimpleAbilitySpec : AbilitySpec
    {

        protected AbilitySystemCharacter source;
        protected AbilitySystemCharacter target;

        public SimpleAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
        {
        }

        public override void CancelAbility()
        {

        }

        protected override void ActivateAbility()
        {
            // Apply cost and cooldown
            var cdSpec = this.Owner.MakeOutgoingSpec(this.AbilityScriptableObject.Cooldown);
            var costSpec = this.Owner.MakeOutgoingSpec(this.AbilityScriptableObject.Cost);

            var effectSpec = this.Owner.MakeOutgoingSpec((this.AbilityScriptableObject as SimpleAbilityScriptableObject).GameplayEffect);

            this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
            this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);
            this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);
        }

        public override bool CheckGameplayTags()
        {

            // Source ASC must have all tags in SourceTags.RequireTags
            // Source ASC must have none of tags in SourceTags.IgnoreTags
            // Target ASC must have all tags in TargetTags.RequireTags
            // Target ASC must have none of tags in TargetTags.IgnoreTags

            return AscHasAllTags(Owner, this.AbilityScriptableObject.AbilityTags.OwnerTags.RequireTags)
                    && AscHasNoneTags(Owner, this.AbilityScriptableObject.AbilityTags.OwnerTags.IgnoreTags)
                    && AscHasAllTags(source, this.AbilityScriptableObject.AbilityTags.SourceTags.RequireTags)
                    && AscHasNoneTags(source, this.AbilityScriptableObject.AbilityTags.SourceTags.IgnoreTags)
                    && AscHasAllTags(target, this.AbilityScriptableObject.AbilityTags.TargetTags.RequireTags)
                    && AscHasNoneTags(target, this.AbilityScriptableObject.AbilityTags.TargetTags.IgnoreTags);
        }

        public override float CheckCooldown()
        {
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

        protected override void PreActivate()
        {
            //throw new System.NotImplementedException();
        }

        public override bool CheckCost()
        {
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
    }

}