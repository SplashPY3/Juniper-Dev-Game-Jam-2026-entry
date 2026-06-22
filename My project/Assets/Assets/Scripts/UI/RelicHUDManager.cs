using System.Collections.Generic;
using UnityEngine;

public class RelicHUDManager : MonoBehaviour
{
    [SerializeField] private RelicSlotUI relicSlotPrefab;
    [SerializeField] private Transform slotsContainer;

    private readonly List<RelicSlotUI> spawnedSlots = new();

    private void OnEnable()
    {
        RelicManager.OnRelicsChanged += Refresh;
    }

    private void OnDisable()
    {
        RelicManager.OnRelicsChanged -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (RelicManager.Instance == null) return;

        IReadOnlyList<RelicInstance> relics = RelicManager.Instance.ActiveRelics;

        while (spawnedSlots.Count < relics.Count)
        {
            RelicSlotUI slot = Instantiate(relicSlotPrefab, slotsContainer);
            spawnedSlots.Add(slot);
        }

        for (int i = 0; i < relics.Count; i++)
        {
            spawnedSlots[i].gameObject.SetActive(true);
            spawnedSlots[i].Bind(relics[i]);
        }

        // Hide unused slots from a previous state
        for (int i = relics.Count; i < spawnedSlots.Count; i++)
        {
            spawnedSlots[i].gameObject.SetActive(false);
        }
    }
}
