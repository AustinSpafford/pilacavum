﻿using UnityEngine;
using System.Collections;

public class BallSpawner : MonoBehaviour
{
	public GameObject BallPrefab = null;

	public int TotalBallCount = 100;
	public float BallsPerSecond = 10.0f;

	public void Update()
	{
		if (spawnedBallCount < TotalBallCount)
		{
			remainingSecondsUntilBallSpawn -= Time.deltaTime;

			while (remainingSecondsUntilBallSpawn <= 0.0f)
			{
				Vector3 localRandomBallPosition = 
					new Vector3(
						Random.Range(-0.5f, 0.5f),
						Random.Range(-0.5f, 0.5f),
						Random.Range(-0.5f, 0.5f));
				
				Vector3 randomBallPosition = transform.TransformPoint(localRandomBallPosition);

				GameObject newBall = 
					GameObject.Instantiate(
						BallPrefab,
						randomBallPosition,
						Random.rotationUniform) as GameObject;

				// Set the ball as a _sibling_. This allows us to use the scale to define the spawning volume.
				newBall.transform.parent = transform.parent;

				++spawnedBallCount;
				remainingSecondsUntilBallSpawn += (1.0f / BallsPerSecond);
			}
		}
	}

	private float remainingSecondsUntilBallSpawn = 0.0f;
	private int spawnedBallCount = 0;
}