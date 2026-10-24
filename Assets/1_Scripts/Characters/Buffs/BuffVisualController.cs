using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BuffVisualController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameEntity entity;

    [Header("Buff Visuals")]
    [SerializeField] public List<BuffVisual> buffVisuals = new List<BuffVisual>();
    [SerializeField] public List<BuffVisual> statusVisuals = new List<BuffVisual>();
    public enum BUFF_INDICES
    {
        ATTACK = 0,
        MAGIC,
        RESISTANCE
    };
    public enum STATUS_INDICES
    {
        POISON = 0,
        SPEED
    };

    private void Update()
    {
        SetBuffVis((int)BUFF_INDICES.RESISTANCE, entity.resMod);
        SetBuffVis((int)BUFF_INDICES.ATTACK, entity.atkMod);
        SetBuffVis((int)BUFF_INDICES.MAGIC, entity.magMod);

        SetStatusVals((int)STATUS_INDICES.POISON, entity.poisoned);
        SetStatusVals((int)STATUS_INDICES.SPEED, entity.spdDebuffed);
    }

    private void SetBuffVis(int index, float mod)
    {
        if (mod > 0 || mod < 0)
        {
            buffVisuals[index].gameObject.SetActive(true);
            buffVisuals[index].SetPlaying(true);
            if (buffVisuals[index].arrow != null)
            {
                float rotation = mod > 0 ? 180f : 0f; 
                buffVisuals[index].arrow.transform.rotation = new Quaternion (0f, 0f, rotation, 1.0f);
            }
        } else
        {
            buffVisuals[index].SetPlaying(false);
            buffVisuals[index].gameObject.SetActive(false);
        }
    }
    private void SetStatusVals(int index, bool afflicted)
    {
        statusVisuals[index].SetPlaying(afflicted);
    }

}
