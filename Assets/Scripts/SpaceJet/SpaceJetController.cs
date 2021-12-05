using Unity.VisualScripting;
using UnityEngine;

namespace SpaceJet
{
    public class SpaceJetController : MonoBehaviour
    {
        [SerializeField] private Vector3 pivot;
        
        private readonly Vector3 _axis = Vector3.up;
        
        private void FixedUpdate()
        {
            transform.RotateAround(pivot, _axis, 20.0f * Time.deltaTime);
            transform.Rotate(0, 0, 1, Space.Self);
        }
    }
}
