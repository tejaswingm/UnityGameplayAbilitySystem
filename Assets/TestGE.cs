using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

public class TestGE : MonoBehaviour
{
    public GameplayEffectScriptableObject Test;
    private AbilitySystemCharacter character;

    void Start()
    {
        this.character = GetComponent<AbilitySystemCharacter>();
    }

    // Start is called before the first frame update
    void OnGUI()
    {
        var abilityCd = this.character.GrantedAbilities[0].CheckCooldown();
        if (abilityCd > 0) return;
        if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
        {
            var geSpec = character.MakeOutgoingSpec(this.Test);
            //character.ApplyGameplayEffectSpecToSelf(geSpec);
            this.character.GrantedAbilities[0].TryActivateAbility();
        }
    }


}
