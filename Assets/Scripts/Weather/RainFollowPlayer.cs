using UnityEngine;

namespace Weather
{
    public class RainFollowPlayer : MonoBehaviour
    {
	    [SerializeField] private Transform target;
	    
        private void Update()
        {
	        // Keep the rain always above the player, and oriented to the ground
	        Transform thisTransform = transform;
	        Vector3 targetPosition = target.position;
	        Vector3 ea = target.eulerAngles;
	        
	        thisTransform.position =
		        new Vector3(targetPosition.x, targetPosition.y + 10, targetPosition.z);
	        thisTransform.rotation = Quaternion.Euler(90, 0, -ea.y);
        }
    }
}
