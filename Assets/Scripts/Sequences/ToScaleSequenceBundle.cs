using DG.Tweening;
using System;
using UnityEngine;

namespace Sequences
{
    [CreateAssetMenu(menuName = "Sequences/ToScaleSequence", order = 3, fileName = "ToScaleSequence")]
    public class ToScaleSequenceBundle : SequenceBundle
    {
        public bool SimpleSetting;
        public float ToFScale = 1f;
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
            var sequence = DOTween.Sequence();

            if (SimpleSetting)
            {
                sequence.Append(transform.DOScale(ToFScale, ScaleDuration));
            }
            else
            {
                sequence.Append(transform.DOScale(ToScale, ScaleDuration));
            }
            
            sequence.OnKill(() => onKillAction?.Invoke());

            return sequence;
        }
    }
}