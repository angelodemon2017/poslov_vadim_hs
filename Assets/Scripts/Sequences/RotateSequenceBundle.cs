using DG.Tweening;
using System;
using UnityEngine;

namespace Sequences
{
    [CreateAssetMenu(menuName = "Sequences/RotateSequence", order = 3, fileName = "RotateSequence")]
    public class RotateSequenceBundle : SequenceBundle
    {
        [SerializeField] private float _rotationDuration;

        public override Sequence Get(Transform transform, Action onKillAction = null)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DORotate(new Vector3(0, 360, 0), _rotationDuration, RotateMode.FastBeyond360));
            sequence.SetLoops(-1, LoopType.Restart);
            sequence.OnKill(() => onKillAction?.Invoke());

            return sequence;
        }
    }
}