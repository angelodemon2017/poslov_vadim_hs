using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sequences
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private List<SequencePart> _sequences = new();
        private List<Sequence> _currentSequences = new();

        public void StartSequences(SequenceType sequenceType)
        {
            _currentSequences.Clear();
            _currentSequences.AddRange(
                _sequences
                .Where(x => x.SequenceType == sequenceType)
                .Select(x => x.SequenceBundle.Get(x.Transform, x.EndSequence)));

            foreach (var cs in _currentSequences)
            {
                cs.Play();
            }
        }

        public void EndIdle()
        {
            foreach (var cs in _currentSequences)
            {
                cs.Kill();
            }
        }
    }
}