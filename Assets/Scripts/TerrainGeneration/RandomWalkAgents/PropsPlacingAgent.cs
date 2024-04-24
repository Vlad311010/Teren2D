using System.Collections.Generic;
using UnityEngine;

public class PropsPlacingAgent : RandomWalkAgent
{
    public List<Vector2Int> CellsToPlaceProps => cellsToPlaceProps;

    private List<Vector2Int> cellsToPlaceProps;
    private Vector2Int objPlacingInterval;
    private Vector2Int gridSize;

    private int stepsToPlaceObject;

    public PropsPlacingAgent(Vector2Int gridSize, int lifetime, float rotationChance, Vector2Int objPlacingInterval) : base(lifetime, rotationChance)
    {
        this.gridSize = gridSize;
        this.objPlacingInterval = objPlacingInterval;
    }

    protected override void OnLoop()
    {
        stepsToPlaceObject--;
        if (stepsToPlaceObject == 0)
        {
            stepsToPlaceObject = objPlacingInterval.RandomRange();
            PlaceObject();
        }
    }

    protected override void OnExecute()
    {
        stepsToPlaceObject = objPlacingInterval.RandomRange();
        cellsToPlaceProps = new List<Vector2Int>();
    }

    protected override bool CanMoveTo(Vector2Int coordinates)
    {
        return agentPosition.x == 0 || agentPosition.x == gridSize.x - 1 || agentPosition.y == 0 || agentPosition.y == gridSize.y - 1;
    }


    private void PlaceObject()
    {
        if (!cellsToPlaceProps.Contains(agentPosition))
        {
            cellsToPlaceProps.Add(agentPosition);
        }
    }
}
