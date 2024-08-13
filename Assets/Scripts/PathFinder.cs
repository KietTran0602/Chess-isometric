using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> openlist = new List<OverlayTile>();
        List<OverlayTile> closelist = new List<OverlayTile>();

        openlist.Add(start);
        while (openlist.Count > 0)
        {
            OverlayTile currrentOverlayTile = openlist.OrderBy(x => x.F).First();

            openlist.Remove(currrentOverlayTile);
            closelist.Add(currrentOverlayTile);
            if (currrentOverlayTile == end)
            {
                //finalize your path
                return GetFinishedList(start, end);
            }
            var neighbourTiles = GetNeighbourTile(currrentOverlayTile);

            foreach (var neighbourTile in neighbourTiles)
            {
                if (neighbourTile.isBlocked || closelist.Contains(neighbourTile) || Mathf.Abs(currrentOverlayTile.gridlocation.z - neighbourTile.gridlocation.z) > 1)
                {
                    continue;
                }

                neighbourTile.G = GetManhattenDistance(start, neighbourTile);
                neighbourTile.G = GetManhattenDistance(end, neighbourTile);
                neighbourTile.previous = currrentOverlayTile;

                if (!openlist.Contains(neighbourTile))
                {
                    openlist.Add(neighbourTile);
                }
            }
        }

        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishList = new List<OverlayTile>();
        OverlayTile currentTile = end;
        while (currentTile != start)
        {
            finishList.Add(currentTile);
            currentTile = currentTile.previous;
        }
        finishList.Reverse();
        return finishList;
    }

    private int GetManhattenDistance(OverlayTile start, OverlayTile neighbourTile)
    {
        return Mathf.Abs(start.gridlocation.x - neighbourTile.gridlocation.x) + Mathf.Abs(start.gridlocation.y - neighbourTile.gridlocation.y);
    }

    private List<OverlayTile> GetNeighbourTile(OverlayTile currrentOverlayTile)
    {
        var map = MapManager.Instance.map;

        List<OverlayTile> neighbour = new List<OverlayTile>();

        //top
        Vector2Int locationToCheck = new Vector2Int(currrentOverlayTile.gridlocation.x, currrentOverlayTile.gridlocation.y + 1);
        if (map.ContainsKey(locationToCheck))
        {
            neighbour.Add(map[locationToCheck]);
        }

        //bottom
        locationToCheck = new Vector2Int(currrentOverlayTile.gridlocation.x, currrentOverlayTile.gridlocation.y - 1);
        if (map.ContainsKey(locationToCheck))
        {
            neighbour.Add(map[locationToCheck]);
        }

        //right
        locationToCheck = new Vector2Int(currrentOverlayTile.gridlocation.x + 1, currrentOverlayTile.gridlocation.y);
        if (map.ContainsKey(locationToCheck))
        {
            neighbour.Add(map[locationToCheck]);
        }

        //left
        locationToCheck = new Vector2Int(currrentOverlayTile.gridlocation.x - 1, currrentOverlayTile.gridlocation.y);
        if (map.ContainsKey(locationToCheck))
        {
            neighbour.Add(map[locationToCheck]);
        }

        return neighbour;
    }
}
