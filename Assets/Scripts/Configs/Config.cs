using UnityEngine;
using Data;

[CreateAssetMenu(menuName = "ScriptableObjects/Config", order = 2, fileName = "Config")]
public class Config : ScriptableObject
{
    [Range(2, 50)]
    public int PatrolPoints;
    [Range(1, 10)]
    public float MinimalPointDistance;
    public int StartHPCharacter;
    public int DamageValue;

    private void OnValidate()
    {
        if (DamageValue <= 1)
        {
            DamageValue = 1;
        }
        if (StartHPCharacter <= 1)
        {
            StartHPCharacter = 1;
        }
    }

    public PersonModel GeneratePerson(Vector3 posit)
    {
        return new PersonModel(StartHPCharacter, posit);
    }
}