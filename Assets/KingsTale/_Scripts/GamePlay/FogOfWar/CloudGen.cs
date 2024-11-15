using UnityEngine;

public class CloudGen : MonoBehaviour {

    public GameObject[] _clouds;
    public float width = 10;
    public float height = 10;
    public float cloudDistance = 5;
	[SerializeField] private float _minSize = 0.9f;
    [SerializeField] private float _maxSize = 1f;
	// Use this for initialization
	public void Start () {
		for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
				var cloud = _clouds[Random.Range(0,_clouds.Length)];
                GameObject go = Instantiate(cloud, new Vector3(transform.position.x + x * cloudDistance, transform.position.y, transform.position.z + y * cloudDistance), Quaternion.identity);

                go.name = "Cloud_" + x + "_" + y;
                go.transform.SetParent(transform);
				go.transform.GetChild(0).localScale = new Vector3(1,1,1) * Random.Range(_minSize,_maxSize);

            }
        }
	}
}
