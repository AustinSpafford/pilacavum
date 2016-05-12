using UnityEngine;
using System.Collections;

public class VelociGripper : MonoBehaviour
{
	public float PositionCorrectionMetersPerSecondPerDeltaMeter = 50.0f;

	[Tooltip("If the gripped object is ever beyond this distance, it's snapped to the gripper.")]
	public float LeashDistance = 0.5f;

	public bool RemoveGravityFromGrippedObjects = true;

	[Tooltip("Increasing the mass of an object gives a sense of strength when observing collisions.")]
	public float GrippedObjectMassMultiplier = 5.0f;

	[Tooltip("Increasing the linear velocity of released objects gives the throw \"oopmh\".")]
	public float ReleasedLinearVelocityMultiplier = 1.0f;

	[Tooltip("Increasing the angular velocity of released objects might just feel weird.")]
	public float ReleasedAngularVelocityMultiplier = 1.0f;

	public bool DebugEnabled = false;

	public void Awake()
	{
		velocityTracker = GetComponent<VelocityTracker>();
	}

	public void FixedUpdate()
	{
		if (grippedObject != null)
		{
			Vector3 objectToGripperPositionDelta = 
				(transform.position - grippedObject.transform.position);

			Rigidbody grippedRigidBody = grippedObject.GetComponent<Rigidbody>();

			// If either there's no physics to work with or the physics has
			// gone sick (the object broke its leash), just snap it into position.
			if ((grippedRigidBody == null) ||
				(objectToGripperPositionDelta.sqrMagnitude > (LeashDistance * LeashDistance)))
			{
				grippedObject.transform.position = transform.position;
				grippedObject.transform.rotation = transform.rotation;
			}
			else
			{
				grippedRigidBody.velocity =
					(objectToGripperPositionDelta * PositionCorrectionMetersPerSecondPerDeltaMeter);
			}
		}
	}

	public void GripObject(
		GameObject targetObject)
	{
		if (grippedObject != null)
		{
			ReleaseObject();
		}
		
		Rigidbody targetRigidBody = targetObject.GetComponent<Rigidbody>();

		if (targetRigidBody != null)
		{
			targetRigidBody.mass *= GrippedObjectMassMultiplier;

			grippedObjectUsedGravity = targetRigidBody.useGravity;

			if (RemoveGravityFromGrippedObjects)
			{
				targetRigidBody.useGravity = false;
			}
		}

		grippedObject = targetObject;

		if (DebugEnabled)
		{
			Debug.LogFormat("Gripped [0].", grippedObject.name);
		}
	}

	public void ReleaseObject()
	{
		if (grippedObject != null)
		{
			if (DebugEnabled)
			{
				Debug.LogFormat("Releasing [0].", grippedObject.name);
			}
		
			Rigidbody grippedRigidBody = grippedObject.GetComponent<Rigidbody>();

			if (grippedRigidBody != null)
			{
				grippedRigidBody.mass /= GrippedObjectMassMultiplier;
				
				if (RemoveGravityFromGrippedObjects &&
					grippedObjectUsedGravity)
				{
					grippedRigidBody.useGravity = true;
				}

				if (velocityTracker != null)
				{
					grippedRigidBody.velocity = 
						(velocityTracker.AverageLinearVelocity * ReleasedLinearVelocityMultiplier);

					grippedRigidBody.angularVelocity = 
						(velocityTracker.AverageAngularVelocity * ReleasedAngularVelocityMultiplier);
				}
			}

			grippedObject = null;
			grippedObjectUsedGravity = false;
		}
	}
	
	private GameObject grippedObject = null;
	private bool grippedObjectUsedGravity = false;

	private VelocityTracker velocityTracker = null;
}
