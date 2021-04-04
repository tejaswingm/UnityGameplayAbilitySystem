namespace AbilitySystem.Authoring
{
    public class InitialiseStatsAbility : AbilitySpec
    {
        public InitialiseStatsAbility(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
        {
        }

        public override void CancelAbility()
        {
        }

        public override bool CheckGameplayTags()
        {
            return AscHasAllTags(Owner, this.AbilityScriptableObject.AbilityTags.OwnerTags.RequireTags)
                    && AscHasNoneTags(Owner, this.AbilityScriptableObject.AbilityTags.OwnerTags.IgnoreTags);
        }

        protected override void ActivateAbility()
        {
            // Apply cost and cooldown
            var cdSpec = this.Owner.MakeOutgoingSpec(this.AbilityScriptableObject.Cooldown);
            var costSpec = this.Owner.MakeOutgoingSpec(this.AbilityScriptableObject.Cost);
            this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
            this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

            InitialiseStatsAbilityScriptableObject abilitySO = this.AbilityScriptableObject as InitialiseStatsAbilityScriptableObject;
            for (var i = 0; i < abilitySO.InitialisationGE.Length; i++)
            {
                var effectSpec = this.Owner.MakeOutgoingSpec(abilitySO.InitialisationGE[i]);
                this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);
                this.Owner.AttributeSystem.UpdateAttributeCurrentValues();
            }


        }

        protected override void PreActivate()
        {
        }
    }

}