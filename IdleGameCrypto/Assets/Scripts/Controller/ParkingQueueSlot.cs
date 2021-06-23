using UnityEngine;
    public class ParkingQueueSlot : MonoBehaviour
    {
        public bool IsUsed;
        public int Index;
        public Vector3 Position => base.transform.position;
        public Transform waitPos => base.transform;
        public CarMovementView Car
        {
            get;
            private set;
        }
        public void Use(CarMovementView car)
        {
            IsUsed = true;
            Car = car;
        }
        public void Free()
        {
            IsUsed = false;
            Car = null;
        }
        protected void OnDrawGizmos()
        {
            Gizmos.color = (IsUsed ? Color.red : Color.green);
            float num = 0.5f;
            float num2 = 0.5f;
            Vector3 vector = base.transform.position + new Vector3(num, 0f, num2);
            Vector3 vector2 = base.transform.position + new Vector3(num, 0f, 0f - num2);
            Vector3 vector3 = base.transform.position + new Vector3(0f - num, 0f, 0f - num2);
            Vector3 vector4 = base.transform.position + new Vector3(0f - num, 0f, num2);
            Gizmos.DrawLine(vector, vector2);
            Gizmos.DrawLine(vector2, vector3);
            Gizmos.DrawLine(vector3, vector4);
            Gizmos.DrawLine(vector4, vector);
            if (IsUsed)
            {
                Gizmos.DrawLine(base.transform.position, Car.transform.position);
            }
        }
       private void Awake()
       {
        Index = transform.GetSiblingIndex();
       }
}






