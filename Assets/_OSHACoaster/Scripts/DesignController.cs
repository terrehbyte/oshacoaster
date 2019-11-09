using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DesignController : MonoBehaviour
{
    public Transform gridTransform;
    public Camera targetCamera;
    public BuildTile[] buildCandidates;
    private int _buildCandidateIndex = -1;
   
    public int BuildCandidateIndex
    {
        get
        {
            return _buildCandidateIndex;
        }
        set
        {
            var oldIndex = _buildCandidateIndex;
            _buildCandidateIndex = value;
            if(oldIndex != value)
            {
                OnBuildCandidateChangedInternal(buildCandidates[value]);
                OnBuildCandidateChanged.Invoke(buildCandidates[value]);
            }
        }
    }
    public BuildTile CurrentBuildCandidate
    {
        get
        {
            try
            {
                return buildCandidates[BuildCandidateIndex];
            }
            catch (System.IndexOutOfRangeException)
            {
                return null;
            }
        }
    }

    public int tileSize = 2;
    private Plane buildPlane;

    public Vector3 lastBuildHit;
    public Transform previewTransform;
    public MeshFilter previewMeshFilter;

    public Transform tilePreviewTransform;

    public UnityEventBuildTile OnBuildCandidateChanged;

    void Start()
    {
        buildPlane = new Plane(Vector3.up, 0);
        tilePreviewTransform.localScale = new Vector3(tileSize, 0.1f, tileSize);
        BuildCandidateIndex = 0;
    }

    void Update()
    {
        Debug.Assert(buildPlane.normal == Vector3.up, "Only build planes facing world up are supported at this time.");
        Vector3 buildHit = buildPlane.GetIntersection(targetCamera.ScreenPointToRay(Input.mousePosition));
        // HACK: GetIntersection sometimes provides the wrong Y level. Let's just hard code this to be the same as the plane.
        buildHit.y = buildPlane.distance;
        
        Vector3Int tileLoc = GetTileLocation(buildHit);
        Vector3 buildLoc = GetInstantiationLocation(tileLoc);
        Debug.DrawRay(buildHit, Vector3.up * 10.0f, Color.red, 0.0f);
        Debug.DrawRay(buildLoc, Vector3.up * 5.0f, Color.green, 0.0f);
        lastBuildHit = buildHit;

        previewTransform.position = buildLoc;
        tilePreviewTransform.position = buildLoc;

        if(Input.GetButtonDown("Fire1"))
        {
            Instantiate(CurrentBuildCandidate.buildPrefab, buildLoc, Quaternion.identity);
        }
    }

    // Returns the position of a tile in world space
    // This represents the bottom-left corner of the tile
    public Vector3 GetWorldLocation(Vector3Int worldTileLocation)
    {
        return Vector3.Scale((Vector3)worldTileLocation, new Vector3(tileSize, tileSize, tileSize));
    }

    void OnBuildCandidateChangedInternal(BuildTile candidate)
    {
        previewMeshFilter.mesh = candidate.buildMesh;
    }

    // Returns the position of a tile in tile space
    public Vector3Int GetTileLocation(Vector3 worldLocation)
    {
        worldLocation.Scale(new Vector3(1.0f / tileSize, 1.0f / tileSize, 1.0f / tileSize));
        return Vector3Int.FloorToInt(worldLocation);
    }

    // Returns the center of a tile in world space, given a world space location
    public Vector3 GetInstantiationLocation(Vector3 worldLocation)
    {
        return GetInstantiationLocation(GetTileLocation(worldLocation));
    }

    // Returns the center of a tile in world space, given a tile space locatin
    public Vector3 GetInstantiationLocation(Vector3Int worldTileLocation)
    {
        Vector3 worldLocation = GetWorldLocation(worldTileLocation);
        Vector3 offset = new Vector3((float)tileSize / 2.0f, 0.0f, (float)tileSize / 2.0f);

        return worldLocation + offset;
    }
}

[System.Serializable]
public class UnityEventBuildTile : UnityEvent<BuildTile> { }