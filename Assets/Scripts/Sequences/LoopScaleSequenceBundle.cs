using DG.Tweening;
using System;
using UnityEngine;

namespace Sequences
{
    [CreateAssetMenu(menuName = "Sequences/LoopScaleSequence", order = 3, fileName = "LoopScaleSequence")]
    public class LoopScaleSequenceBundle : SequenceBundle
    {
        public bool SimpleSetting;
        public float FromfScale = 1f;
        public float TofScale = 1f;
        public Vector3 FromScale = Vector3.one;
        public Vector3 ToScale = Vector3.one;
        public float ScaleDuration = 1f;

        private void OnValidate()
        {
            if (ScaleDuration <= 0f)
            {
                ScaleDuration = 0;
            }
        }

        public override Sequence Get(Transform transform, Action onKillAction = null)
        {
            Sequence sequence = DOTween.Sequence();

            if (SimpleSetting)
            {
                sequence.Append(transform.DOScale(TofScale, ScaleDuration));
                sequence.Append(transform.DOScale(FromfScale, ScaleDuration));
            }
            else
            {
                sequence.Append(transform.DOScale(ToScale, ScaleDuration));
                sequence.Append(transform.DOScale(FromScale, ScaleDuration));
            }
            sequence.SetLoops(-1, LoopType.Restart);
            sequence.OnKill(() => onKillAction?.Invoke());

            return sequence;
        }
    }
}