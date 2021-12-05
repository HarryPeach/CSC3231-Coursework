using UnityEngine;

namespace Gyroscope
{
    public class AlwaysRotate : MonoBehaviour
    {
        [SerializeField] private float rotateXSpeed;
        [SerializeField] private float rotateYSpeed;
        [SerializeField] private float rotateZSpeed;

        private void FixedUpdate()
        {
            // Continuously rotates using the given variables
            transform.Rotate(rotateXSpeed, rotateYSpeed, rotateZSpeed, Space.World);
        }
    }
}
