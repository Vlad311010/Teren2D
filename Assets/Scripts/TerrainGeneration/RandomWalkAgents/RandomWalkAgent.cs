using Enums;
using Structs;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkAgent
{
    protected Vector2Int agentPosition;
    private int startLifetime;
    protected float rotationChance;

    private Vector2Int forward;
    protected AgentRotation[] rotationsList;
    protected bool terminate = false;
    
    private int currentLifetime;
    private List<Vector2Int> path;

    public RandomWalkAgent(Vector2Int startPosition, Vector2Int lookDirection, int lifetime, float rotationChance)
    {
        agentPosition = startPosition;
        forward = lookDirection;

        this.startLifetime = lifetime;
        this.rotationChance = rotationChance;

        rotationsList = new AgentRotation[] { new AgentRotation(AgentRotationDirection.Left, 0.5f), new AgentRotation(AgentRotationDirection.Right, 0.5f) };
    }

    private void Move()
    {
        if (CanMoveTo(agentPosition + forward))
        {
            agentPosition += forward;
            path.Add(agentPosition);
        }
    }

    private void Rotate()
    {
        AgentRotationDirection rotateTo = AgentRotationDirection.None;
        if (Random.value < rotationChance)
        {
            float rotationPick = Random.value;
            for (int i = 0; i < rotationsList.Length; i++)
            {
                if (rotationPick <= rotationsList[i].probability)
                {
                    rotateTo = rotationsList[i].direction;
                    break;
                }
            }
        }
        ChangeRotation(rotateTo);
        rotationsList = CalculateRotationsList(rotationsList, rotateTo);
    }

    private void ChangeRotation(AgentRotationDirection rotateTo)
    {
        switch (rotateTo) 
        { 
            case AgentRotationDirection.None:
                break;
            case AgentRotationDirection.Left:
                forward = forward.Rotate(90);
                break; 
            case AgentRotationDirection.Right:
                forward = forward.Rotate(-90);
                break; 
            case AgentRotationDirection.TurnAround:
                forward = -forward;
                break;
        }
    }


    protected virtual bool CanMoveTo(Vector2Int coordinates)
    {
        return true;
    }

    protected virtual AgentRotation[] CalculateRotationsList(AgentRotation[] currentRotationsList, AgentRotationDirection lastRotation)
    {
        return currentRotationsList;
    }

    protected virtual void OnLoop()
    {
        
    }


    public List<Vector2Int> Execute(Vector2Int newPosition, Vector2Int newLookDirection)
    {
        agentPosition = newPosition;
        forward = newLookDirection;

        return Execute();
    }


    public List<Vector2Int> Execute()
    {
        terminate = false;
        currentLifetime = startLifetime;
        path = new List<Vector2Int>() { agentPosition };
        return Loop();
    }


    private List<Vector2Int> Loop()
    {
        OnLoop();
        Rotate();
        Move();

        currentLifetime--;
        if (currentLifetime <= 0 || terminate)
            return path;
        else
            return Loop();
    }

}
