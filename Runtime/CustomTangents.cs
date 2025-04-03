using UnityEngine;

namespace com.logandlp.prefabrush.runtime
{
    public class CustomTangents
    {
        public Vector3 Tangent1 { get; private set; }
        public Vector3 Tangent2 { get; private set; }
        
        public CustomTangents(Vector3 tangent1, Vector3 tangent2)
        {
            Tangent1 = tangent1;
            Tangent2 = tangent2;
        }

        public CustomTangents(Vector3 normal)
        {
            Tangent1 = Vector3.Cross(normal, Vector3.up).normalized;
            if (Tangent1 == Vector3.zero)
            {
                Tangent1 = Vector3.Cross(normal, Vector3.right).normalized;
            }
            Tangent2 = Vector3.Cross(normal, Tangent1).normalized;
        }
    }
}