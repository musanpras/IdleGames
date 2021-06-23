using System.Text;
using UnityEngine;

    public class ParkingSlot : MonoBehaviour
    {
        public bool IsReserved;

        public bool IsCarParked;

        public int GlobalIndex;

        public Transform InPoint;

        public Transform ParkedPoint;

        public Transform OutPoint;

        public Transform WalkPoint;

        public bool _associateWithCar;

        public bool IsUsed
        {
            get
            {
              //  if (!IsReserved)
                {
                    return IsCarParked;
                }
               // return true;
            }
        }
        public ParkingLane Lane
        {
            get;
            private set;
        }

    public void Initialize()
    {
        InPoint = base.transform.Find("IN");
        ParkedPoint = base.transform.Find("PARKED");
        OutPoint = base.transform.Find("OUT");
        WalkPoint = base.transform.Find("WalkPoint");
            
    }

    public void AssociateWith(ParkingLane lane)
        {
            Lane = lane;
        }
        protected void OnDrawGizmos()
        {
            if (!(ParkedPoint == null))
            {
                Color color2 = Gizmos.color = (IsCarParked ? Color.red : ((!IsReserved) ? Color.green : Color.yellow));
                float num = 0.5f;
                float num2 = 0.5f;
                Vector3 vector = ParkedPoint.position + new Vector3(num, 0f, num2);
                Vector3 vector2 = ParkedPoint.position + new Vector3(num, 0f, 0f - num2);
                Vector3 vector3 = ParkedPoint.position + new Vector3(0f - num, 0f, 0f - num2);
                Vector3 vector4 = ParkedPoint.position + new Vector3(0f - num, 0f, num2);
                Gizmos.DrawLine(vector, vector2);
                Gizmos.DrawLine(vector2, vector3);
                Gizmos.DrawLine(vector3, vector4);
                Gizmos.DrawLine(vector4, vector);
            }
        }
        public string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.name);
            stringBuilder.Append("@");
            stringBuilder.Append(Lane.name);
            return stringBuilder.ToString();
        }
    }






