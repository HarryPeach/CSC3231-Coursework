using System;
using UnityEngine;

namespace Gyroscope
{
    public class Bob : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The height that the object should bob at")]
        private float baseHeight = 103f;

        [SerializeField]
        [Tooltip("How much the object should bob up and down")]
        private float bobbiness = 1.1f;

        private void Update()
        {
            // Use a sine wave to make the rings bob smoothly
            Transform objTransform = transform;
            objTransform.position = new Vector3(objTransform.position.x,
                (float)(baseHeight + bobbiness * Math.Sin(Time.time)), (objTransform).position.z);
        }
    }
}
