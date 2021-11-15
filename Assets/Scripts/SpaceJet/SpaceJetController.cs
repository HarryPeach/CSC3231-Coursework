using Unity.VisualScripting;
using UnityEngine;

namespace SpaceJet
{
    public class SpaceJetController : MonoBehaviour
    {
        [SerializeField] private Vector3 pivot;

        private float angle = 90;
        private Vector3 axis = Vector3.up;
        
        private void FixedUpdate()
        {
            transform.RotateAround(pivot, axis, 20.0f * Time.deltaTime);
            // transform.position = rot * (transform.position - pivot) + pivot;
            // transform.rotation = rot * transform.rotation;
        }
    }
}
