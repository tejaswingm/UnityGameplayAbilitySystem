using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public AbstractAbilityScriptableObject[] Abilities;
    private AbilitySystemCharacter abilitySystemCharacter;

    // Start is called before the first frame update
    void Start()
    {
        this.abilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
        var spec = Abilities[0].CreateSpec(this.abilitySystemCharacter);
        this.abilitySystemCharacter.GrantAbility(spec);

        spec = Abilities[1].CreateSpec(this.abilitySystemCharacter);
        this.abilitySystemCharacter.GrantAbility(spec);
        spec.TryActivateAbility();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
