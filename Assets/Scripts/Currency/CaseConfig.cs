using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct CaseData
{
    public GameCase caseType;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "CaseConfig", menuName = "Config/CaseConfig")]
public class CaseConfig : ScriptableObject
{
    [SerializeField] private List<CaseData> cases;

    public Sprite GetCaseIcon(GameCase caseType)
    {
        CaseData data = cases.Find(c => c.caseType == caseType);
        return data.icon;
    }
}