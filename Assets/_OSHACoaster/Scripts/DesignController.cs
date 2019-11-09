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
    public GridController grid;

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


    private Plane buildPlane;

    private Vector3 lastBuildHit;

    [SerializeField]
    private Transform previewTransform;
    [SerializeField]
    private MeshFilter previewMeshFilter;
    [SerializeField]
    private MeshRenderer previewMeshRenderer;

    // Use this materiral when the current build location is valid
    [SerializeField]
    private Material previewValidMaterial;
    // Use this materiral when the current build location is invalid
    [SerializeField]
    private Material previewInvalidMaterial;

    [SerializeField]
    private Transform tilePreviewTransform;
    [SerializeField]
    private MeshRenderer tilePreviewMeshRenderer;

    [SerializeField]
    private GameObject buildCandidateButton;
    [SerializeField]
    private RectTransform scrollableItemsContent;

    public UnityEventBuildTile OnBuildCandidateChanged;

    void Start()
    {
        buildPlane = new Plane(Vector3.up, 0);
        tilePreviewTransform.localScale = new Vector3(grid.tileSize, 0.1f, grid.tileSize);
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
        
        Vector3Int tileLoc = grid.GetTileLocation(buildHit);
        Vector3 buildLoc = grid.GetInstantiationLocation(tileLoc);
        Debug.DrawRay(buildHit, Vector3.up * 10.0f, Color.red, 0.0f);
        Debug.DrawRay(buildLoc, Vector3.up * 5.0f, Color.green, 0.0f);
        lastBuildHit = buildHit;

        Material previewMaterial = grid.CheckBuildEligible(tileLoc) ? previewValidMaterial : previewInvalidMaterial;
        previewTransform.position = buildLoc;
        previewMeshRenderer.material = previewMaterial;
        tilePreviewTransform.position = buildLoc;
        tilePreviewMeshRenderer.material = previewMaterial;

        if(Input.GetButtonDown("Rotate"))
        {
            placementAngle += placementDelta;
            previewTransform.rotation = placementRotation;
        }

        if(Input.GetButtonDown("Fire1") && grid.CheckBuildEligible(tileLoc))
        {
            var obj = Instantiate(CurrentBuildCandidate.buildPrefab, buildLoc, placementRotation);
            grid.WriteGridCell(tileLoc, obj);
        }
    }

    void OnBuildCandidateChangedInternal(BuildTile candidate)
    {
        previewMeshFilter.mesh = candidate.buildMesh;
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