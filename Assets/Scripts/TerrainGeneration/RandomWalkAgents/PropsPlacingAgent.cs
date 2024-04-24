using System.Collections.Generic;
using UnityEngine;

public class PropsPlacingAgent : RandomWalkAgent
{
    public List<Vector2Int> CellsToPlaceProps => cellsToPlaceProps;

    private List<Vector2Int> cellsToPlaceProps;
    private Vector2Int objPlacingInterval;

    private int stepsToPlaceObject;

    public PropsPlacingAgent(int lifetime, float rotationChance, Vector2Int objPlacingInterval) : base(lifetime, rotationChance)
    {
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


    private void PlaceObject()
    {
        if (!cellsToPlaceProps.Contains(agentPosition))
        {
            cellsToPlaceProps.Add(agentPosition);
        }
    }
}
