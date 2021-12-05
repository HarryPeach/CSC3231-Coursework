using UnityEngine;

namespace Weather
{
    public class RainFollowPlayer : MonoBehaviour
    {
	    [SerializeField] private Transform target;
	    
        private void Update()
        {
	        Transform thisTransform = transform;
	        Vector3 targetPosition = target.position;
	        Quaternion targetRotation = target.rotation;
	        Quaternion thisRotation = thisTransform.rotation;
	        
	        thisTransform.position =
		        new Vector3(targetPosition.x, targetPosition.y + 10, targetPosition.z);
	        // thisTransform.rotation = new Quaternion(thisRotation.x, thisRotation.y, thisRotation.z, thisRotation.w);
	        Vector3 ea = target.eulerAngles;
	        thisTransform.rotation = Quaternion.Euler(90, 0, -ea.y);
        }
    }
}
