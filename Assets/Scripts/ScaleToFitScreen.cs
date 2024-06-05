using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToFitScreen : MonoBehaviour
{
    private float width;
    private float height;

	// Start is called before the first frame update
	void Start()
    {
		width = transform.localScale.x;
		height = transform.localScale.y;

		ScaleToFit();
	}

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        ScaleToFit();
#endif
    }

    private void ScaleToFit()
    {
		float worldScreenHeight = Camera.main.orthographicSize * 2;

        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        float newHeight = worldScreenWidth / width * height;

		transform.localScale = new Vector3(
			worldScreenWidth,
			newHeight,
            1
            );
    }
}
