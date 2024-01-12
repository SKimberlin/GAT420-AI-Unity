using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAutonomousAgent : AIAgent
{
    public AIPerception seekPerception = null;
    public AIPerception fleePerception = null;
    public AIPerception flockPerception = null;

    private void Update()
    {
        if (seekPerception != null)
        {
            var gameObjects = seekPerception.GetGameObjects();
            if (gameObjects != null && gameObjects.Length > 0)
            {
                movement.ApplyForce(Seek(gameObjects[0]));
            }
        }

        if (fleePerception != null) 
        {
			var gameObjects = fleePerception.GetGameObjects();
			if (gameObjects != null && gameObjects.Length > 0)
			{
				movement.ApplyForce(Flee(gameObjects[0]));
			}
		}

        if (flockPerception != null)
        {
			var gameObjects = flockPerception.GetGameObjects();
            if (gameObjects != null && gameObjects.Length > 0)
            {
				movement.ApplyForce(Cohesion(gameObjects));
				movement.ApplyForce(Seperation(gameObjects, 3));
				movement.ApplyForce(Alignment(gameObjects));
			}
		}

        transform.position = Utilities.Wrap(transform.position, new Vector3(-10, -10, -10), new Vector3(10, 10, 10));
    }

    private Vector3 Seek(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
        return GetSteeringForce(direction);
    }

	private Vector3 Flee(GameObject target)
	{
		Vector3 direction = transform.position - target.transform.position;
		return GetSteeringForce(direction);
	}

    private Vector3 Cohesion(GameObject[] neighbors) 
    {
        Vector3 positions = Vector3.zero;
        foreach (var neighbor in neighbors) 
        {
            positions += neighbor.transform.position;
        }

        Vector3 center = positions / neighbors.Length;
        Vector3 direction = center - transform.position;
        return GetSteeringForce(direction);
        
    }

    private Vector3 Seperation(GameObject[] neighbors, float radius) 
    { 
        Vector3 seperation = Vector3.zero;
        foreach (var neighbor in neighbors) 
        {
            Vector3 direction = (transform.position - neighbor.transform.position);
            if (direction.sqrMagnitude < radius)
            {
                seperation += direction / direction.sqrMagnitude;
            }
        }

		return GetSteeringForce(seperation);
    }

    private Vector3 Alignment(GameObject[] neighbors) 
    { 
        Vector3 velocities = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            velocities += neighbor.GetComponent<AIAgent>().movement.Velocity;
        }
        Vector3 averageVelocity = velocities / neighbors.Length;
        return GetSteeringForce(averageVelocity);
    }
	private Vector3 GetSteeringForce(Vector3 direction) 
    {
        Vector3 desired = direction.normalized * movement.maxSpeed;
        Vector3 steer = desired - movement.Velocity;
        Vector3 force = Vector3.ClampMagnitude(steer, movement.maxForce);

        return force;
    }
}

