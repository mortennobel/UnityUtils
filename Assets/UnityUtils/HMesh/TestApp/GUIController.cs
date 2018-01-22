using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public HMesh hmesh;
    public HMeshRenderer hmeshRenderer;
    public Vertex selectedVertex;
    public Halfedge selectedEdge;
    public Face selectedFace;

    public RectTransform meshButtons;
    public RectTransform vertexButtons;
    public RectTransform edgeButtons;
    public RectTransform faceButtons;

    public Dropdown menu;

	// Use this for initialization
	void Start ()
	{
	    UpdateMenu();
	    hmesh = new HMesh();
	    hmesh.CreateTriangle(new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(1,0,0));
	    hmeshRenderer = FindObjectOfType<HMeshRenderer>();
	    hmeshRenderer.hmesh = hmesh;
	    hmeshRenderer.UpdateMesh();
	}
	
	// Update is called once per frame
	public void UpdateMenu () {
	    meshButtons.gameObject.SetActive(menu.value == 0);
	    vertexButtons.gameObject.SetActive(menu.value == 1);
	    edgeButtons.gameObject.SetActive(menu.value == 2);
	    faceButtons.gameObject.SetActive(menu.value == 3);
	}

    public void CreateTestMesh()
    {
        hmesh.CreateTriangle(new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(1,0,0));
        hmeshRenderer.UpdateMesh();
    }

    public void ClearMesh()
    {
        hmesh.Clear();
        hmeshRenderer.UpdateMesh();
    }


}
