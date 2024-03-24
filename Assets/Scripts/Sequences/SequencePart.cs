using DG.Tweening;
using UnityEngine;

namespace Sequences
{
    [System.Serializable]
    public class SequencePart
    {
        [SerializeField] private Transform _transform;

        [SerializeField] private SequenceBundle _sequenceBundle;
        [SerializeField] private SequenceBundle _endSequence;
        [SerializeField] private SequenceType _sequenceType;
        public Transform Transform => _transform;
        public SequenceBundle SequenceBundle => _sequenceBundle;
        public SequenceType SequenceType => _sequenceType;

        public void EndSequence()
        {
            if (_endSequence == null)
            {
                return;
            }

            var seq = _endSequence.Get(_transform);
            seq.Play();
        }
    }
}