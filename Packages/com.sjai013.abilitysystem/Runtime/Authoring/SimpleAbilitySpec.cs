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
            return AscHasAllTags(Owner, this.AbilityScriptableObject.AbilityTags.OwnerTags.RequireTags)
                    && AscHasNoneTags(Owner, this.AbilityScriptableObject.AbilityTags.OwnerTags.IgnoreTags)
                    && AscHasAllTags(source, this.AbilityScriptableObject.AbilityTags.SourceTags.RequireTags)
                    && AscHasNoneTags(source, this.AbilityScriptableObject.AbilityTags.SourceTags.IgnoreTags)
                    && AscHasAllTags(target, this.AbilityScriptableObject.AbilityTags.TargetTags.RequireTags)
                    && AscHasNoneTags(target, this.AbilityScriptableObject.AbilityTags.TargetTags.IgnoreTags);
        }

        protected override void PreActivate()
        {
            //throw new System.NotImplementedException();
        }

    }
}