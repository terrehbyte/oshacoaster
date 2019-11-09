using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System.Collections.ObjectModel;

public class DesignController : MonoBehaviour
{
    // Transform of the grid we're working with
    // Grid items will be spawned and parented to this Transform
    public Transform gridTransform;

    // Player camera used for design mode
    public Camera targetCamera;

    [SerializeField]
    private float placementAngle = 0.0f;
    private float placementDelta = 90.0f;
    private Quaternion placementRotation
    {
        get
        {
            return Quaternion.AngleAxis(placementAngle, Vector3.up);
        }
    }

    // List of all build candidates
    [SerializeField]
    private List<BuildTile> _buildCandidates;

    public static DesignController instance;
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        if (instance == null)
            instance = this;

    }

    // Read-only access to all build candidates
    public ReadOnlyCollection<BuildTile> buildCandidates
    {
        get
        {
            return _buildCandidates.AsReadOnly();
        }
    }
    // Backing-field for BuildCandidateIndex
    // Authoritative field for considering which build candidate is selected
    private int _buildCandidateIndex = -1;

    // Wrapper and utility for changing the current build index
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
                OnBuildCandidateChangedInternal(_buildCandidates[value]);
                OnBuildCandidateChanged.Invoke(_buildCandidates[value]);
            }
        }
    }

    // Wrapper for changing/accessing the current tile by BuildTile references
    //
    // Will return null if no build tile is selected
    public BuildTile CurrentBuildCandidate
    {
        get
        {
            try
            {
                return _buildCandidates[BuildCandidateIndex];
            }
            catch (System.IndexOutOfRangeException)
            {
                return null;
            }
        }
        set
        {
            int index = _buildCandidates.IndexOf(value);
            if (index != -1) { BuildCandidateIndex = index; }
        }
    }

    // Specifies the NxN dimensions of each tile
    //
    // Do NOT change this at run-time or else bad things :(
    [SerializeField]
    private int _tileSize = 2;
    public int tileSize
    {
        get { return _tileSize; }
    }
    private Plane buildPlane;

    private Vector3 lastBuildHit;
    [SerializeField]
    private Transform previewTransform;
    [SerializeField]
    private MeshFilter previewMeshFilter;

    [SerializeField]
    private Transform tilePreviewTransform;

    [SerializeField]
    private GameObject buildCandidateButton;
    [SerializeField]
    private RectTransform scrollableItemsContent;

    public UnityEventBuildTile OnBuildCandidateChanged;

    void Start()
    {
        buildPlane = new Plane(Vector3.up, 0);
        tilePreviewTransform.localScale = new Vector3(_tileSize, 0.1f, _tileSize);
        BuildCandidateIndex = 0;

        for (int i = 0; i < _buildCandidates.Count; ++i)
        {
            LayoutBuildTile(_buildCandidates[i], i);
        }
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

        if(Input.GetButtonDown("Rotate"))
        {
            placementAngle += placementDelta;
            previewTransform.rotation = placementRotation;
        }

        if(Input.GetButtonDown("Fire1"))
        {
            Instantiate(CurrentBuildCandidate.buildPrefab, buildLoc, placementRotation);
        }
    }

    // Returns the position of a tile in world space
    // This represents the bottom-left corner of the tile
    public Vector3 GetWorldLocation(Vector3Int worldTileLocation)
    {
        return Vector3.Scale((Vector3)worldTileLocation, new Vector3(_tileSize, _tileSize, _tileSize));
    }

    void OnBuildCandidateChangedInternal(BuildTile candidate)
    {
        previewMeshFilter.mesh = candidate.buildMesh;
    }

    // Returns the position of a tile in tile space
    public Vector3Int GetTileLocation(Vector3 worldLocation)
    {
        worldLocation.Scale(new Vector3(1.0f / _tileSize, 1.0f / _tileSize, 1.0f / _tileSize));
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
        Vector3 offset = new Vector3((float)_tileSize / 2.0f, 0.0f, (float)_tileSize / 2.0f);

        return worldLocation + offset;
    }

    // Creates a button and performs layout for a build tile
    private void LayoutBuildTile(BuildTile tile, int tileNumber)
    {
        var candidate = tile;

        var baby = Instantiate<GameObject>(buildCandidateButton, Vector3.zero, Quaternion.identity, scrollableItemsContent);
        
        var switcher = baby.GetComponent<BuildTileSwitcher>();
        switcher.assignedTile = candidate;
        switcher.GetComponentInChildren<UnityEngine.UI.Text>().text = candidate.buildPrefab.name;
        switcher.designController = this;
        
        var switcherRect = switcher.GetComponent<RectTransform>();
        switcherRect.offsetMin = new Vector2(tileNumber * 150, -100);
        switcherRect.offsetMax = new Vector2((++tileNumber) * 150, 0);
        scrollableItemsContent.offsetMax = new Vector2(_buildCandidates.Count * 150, scrollableItemsContent.offsetMax.y);
    }

    // Adds a new BuildTile to the build menu + performs layout
    public void AddBuildTile(BuildTile newTile)
    {
        if (_buildCandidates.Contains(newTile))
            return;
        _buildCandidates.Add(newTile);
        LayoutBuildTile(newTile, _buildCandidates.Count);
    }
}

[System.Serializable]
public class UnityEventBuildTile : UnityEvent<BuildTile> { }