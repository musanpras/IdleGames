using UnityEngine;

    public interface IUnitWalkEvents
    {
        void OnSegmentCompleted(Vector3 newDirection);
        void OnPathCompleted();
    }
