using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Transform mainCamera;

    public float parallaxIntensityX;
    public float parallaxIntensityY;
    public float independantSpeed;

    private float cameraSize;
    private Vector2 initialPos;
    private float translationOffset = 0;

    private void Start()
    {
        mainCamera = Camera.main.transform;
        cameraSize = Camera.main.orthographicSize;

        transform.position = new Vector2(mainCamera.position.x, transform.position.y);
        initialPos = transform.position;
    }

    private void LateUpdate()
    {
        translationOffset += independantSpeed * Time.deltaTime * parallaxIntensityX;

        float parallaxOffsetX = (mainCamera.position.x * (1 - (parallaxIntensityX / 2))) + translationOffset;
        
        transform.position = new Vector2(initialPos.x + parallaxOffsetX, initialPos.y);


        
    }
}


