using DG.Tweening;
using System;
using UnityEngine;

namespace Sequences
{
    public abstract class SequenceBundle : ScriptableObject
    {
        public abstract Sequence Get(Transform transform, Action onKillAction = null);
    }
}