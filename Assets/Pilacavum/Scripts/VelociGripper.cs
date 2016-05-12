using UnityEngine;
using System.Collections;

public class VelociGripper : MonoBehaviour
{
	public float PositionCorrectionMetersPerSecondPerDeltaMeter = 50.0f;

	[Tooltip("If the gripped object is ever beyond this distance, it's snapped to the gripper.")]
	public float LeashDistance = 1.0f;

	public bool RemoveGravityFromGrippedObjects = true;

	public bool DebugEnabled = false;

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

		if (RemoveGravityFromGrippedObjects)
		{
			Rigidbody targetRigidBody = targetObject.GetComponent<Rigidbody>();

			if ((targetRigidBody != null) &&
				targetRigidBody.useGravity)
			{
				targetRigidBody.useGravity = false;
				gravityWasRemovedFromGrippedObject = true;
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

			if (gravityWasRemovedFromGrippedObject)
			{
				grippedObject.GetComponent<Rigidbody>().useGravity = true;

				gravityWasRemovedFromGrippedObject = false;
			}

			grippedObject = null;
		}
	}

	[SerializeField]
	private GameObject grippedObject = null;

	private bool gravityWasRemovedFromGrippedObject = false;
}
