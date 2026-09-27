using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Objects/Skill Data")]
public class SkillDataSO : ScriptableObject
{
    [SerializeField] private Transform _skillPrefab;

    public Transform SkillPrefab => _skillPrefab;
}
