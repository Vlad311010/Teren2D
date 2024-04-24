using System.Collections.Generic;
using UnityEngine;

public class PropsPlacingAgent : RandomWalkAgent
{
    public List<Vector2Int> TilesToPlaceProps => tilesToPlaceProps;

    private List<Vector2Int> tilesToPlaceProps;
    private Vector2Int objPlacingInterval;

    private int stepsToPlaceObject;

    public PropsPlacingAgent(Vector2Int startPosition, Vector2Int lookDirection, int lifetime, float rotationChance, Vector2Int objPlacingInterval) : base(startPosition, lookDirection, lifetime, rotationChance)
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


    private void PlaceObject()
    {
        if (!tilesToPlaceProps.Contains(agentPosition))
        {
            tilesToPlaceProps.Add(agentPosition);
        }
    }
}
