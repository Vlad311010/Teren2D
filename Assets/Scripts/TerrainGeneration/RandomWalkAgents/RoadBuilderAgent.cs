using Enums;
using Structs;
using UnityEngine;

public class RoadBuilderAgent : RandomWalkAgent
{
    private GridSystem<TerrainCell> grid;

    private bool walksStraight = true;
    private bool standsOnWater = false;
    private float defaultRotationChance;
    private float rotationChanceAfterDeviationFromStraightPath;
    private Vector2Int startDirection;

    private AgentRotation[] storedRotationsData;

    public RoadBuilderAgent(GridSystem<TerrainCell> grid, int lifetime, float rotationChance, float restoreRotationChance) : base(lifetime, rotationChance)
    {
        this.grid = grid;
        defaultRotationChance = rotationChance;
        rotationChanceAfterDeviationFromStraightPath = restoreRotationChance;
    }

    protected override bool CanMoveTo(Vector2Int coordinates)
    {
        return grid.ValidateCoordinates(coordinates); ;
    }

    protected override AgentRotation[] CalculateRotationsList(AgentRotation[] currentRotationsList, AgentRotationDirection lastRotation)
    {
        if (lastRotation == AgentRotationDirection.Left || lastRotation == AgentRotationDirection.Right)
        {
            walksStraight = !walksStraight;
            rotationChance = walksStraight ? defaultRotationChance : rotationChanceAfterDeviationFromStraightPath;
            if (walksStraight)
            {
                rotationChance = defaultRotationChance;

            }
            else
            {
                rotationChance = rotationChanceAfterDeviationFromStraightPath;

            }
        }

        switch (lastRotation)
        {
            case AgentRotationDirection.None:
            case AgentRotationDirection.TurnAround:
                return currentRotationsList;

            case AgentRotationDirection.Left:
                if (walksStraight)
                    return new AgentRotation[] { new AgentRotation(AgentRotationDirection.Left, 0.75f), new AgentRotation(AgentRotationDirection.Right, 0.25f) };
                else 
                    return new AgentRotation[] { new AgentRotation(AgentRotationDirection.Right, 1) };

            case AgentRotationDirection.Right:
                if (walksStraight)
                    return new AgentRotation[] { new AgentRotation(AgentRotationDirection.Left, 0.25f), new AgentRotation(AgentRotationDirection.Right, 0.75f) };
                else
                    return new AgentRotation[] { new AgentRotation(AgentRotationDirection.Left, 1) };

            default:
                return currentRotationsList;
        }
    }

    protected override void OnExecute()
    {
        rotationsList = new AgentRotation[] { new AgentRotation(AgentRotationDirection.Left, 0.5f), new AgentRotation(AgentRotationDirection.Right, 0.5f) };
        startDirection = forward;
        walksStraight = true;
        standsOnWater = false;
    }

    protected override void OnLoop()
    {
        // make agent walk straight when on water
        bool onWater = grid.GetCell(agentPosition).NoiseValue < 0;
        if (!standsOnWater && onWater)
        {
            storedRotationsData = rotationsList;
            rotationsList = new AgentRotation[0];
            walksStraight = true;
            forward = startDirection;
        }
        else if (!onWater && standsOnWater)
        {
            rotationsList = storedRotationsData;
        }
        standsOnWater = onWater;

        if (agentPosition.x == 0 || agentPosition.x == grid.Width - 1 || agentPosition.y == 0 || agentPosition.y == grid.Height - 1)
            terminate = true;
    }
}
