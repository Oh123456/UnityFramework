using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AddressableBuildLabels : ScriptableObject
{
    public const string NAME = "AddressableBuildLabels";

    [SerializeField] List<string> labels = new List<string>();

    public List<string> Labels => this.labels;

    public void UpdateLabels(List<string> newLabels)
    {
        this.labels.Clear();

        int count = newLabels.Count;

        for (int i = 0; i < count; i++)
        {
            this.labels.Add(newLabels[i]);
        }
    }

    public void UpdateLabels(HashSet<string> newLabels)
    {
        this.labels.Clear();

        foreach (string label in newLabels)
        {
            this.labels.Add(label);
        }
    }
}
