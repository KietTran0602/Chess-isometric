using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float speed;
    public GameObject characterPrefab;
    private CharacterInfoSelf characterInfo;

    private PathFinder pathFinder;
    private List<OverlayTile> path = new List<OverlayTile>();

    private void Start()
    {
        pathFinder = new PathFinder();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        var focusTileHit = GetFocusOnTile();
        if (focusTileHit.HasValue)
        {
            OverlayTile overlayTile = focusTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

            if (Input.GetMouseButtonDown(0))
            {
                overlayTile.GetComponent<OverlayTile>().ShowTile();

                if (characterInfo == null)
                {
                    //choose character
                    characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfoSelf>();
                    PositionCharacterOnTile(overlayTile);
                }
                else
                {
                    //move character
                    var path = pathFinder.FindPath(characterInfo.activeTile, overlayTile);
                }
            }
        }

        if (path.Count > 0)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        var step = speed + Time.deltaTime;
        var zIndex = path[0].transform.position.z;
        characterInfo.transform.position = Vector2.MoveTowards(characterInfo.transform.position, path[0].transform.position, step);
        characterInfo.transform.position = new Vector3(characterInfo.transform.position.x, characterInfo.transform.position.y, zIndex);
        if (Vector2.Distance(characterInfo.transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }
    }

    public RaycastHit2D? GetFocusOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);
        if (hits.Length > 0)
        {
            return hits.OrderByDescending(h => h.collider.transform.position.z).First();
        }
        return null;
    }

    private void PositionCharacterOnTile(OverlayTile tile)
    {
        characterInfo.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        characterInfo.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        characterInfo.activeTile = tile;
    }
}
