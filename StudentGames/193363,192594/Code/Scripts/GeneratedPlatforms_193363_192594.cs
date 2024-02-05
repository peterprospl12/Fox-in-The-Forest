using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class GeneratedPlatforms : MonoBehaviour
{
    public GameObject platformPrefab;
    public const int PLATFORMS_NUM = 6;
    public GameObject[] platforms;
    public Vector3[] positions;
    public float[] angles;
    [Range(0.01f, 20.0f)][SerializeField] public float rotateSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {

    }
    void Awake()
    {
        platforms = new GameObject[PLATFORMS_NUM];
        positions = new Vector3[PLATFORMS_NUM];
        angles = new float[PLATFORMS_NUM];
        float radius = 5.0f;
        for(int i = 0; i < PLATFORMS_NUM; i++)
        {
            angles[i] = (float)(i * 2.0 * Math.PI / PLATFORMS_NUM)%360;
            double Xposition = radius * Math.Cos(angles[i]);
            double Yposition = radius * Math.Sin(angles[i]);
            positions[i].Set((float)Xposition + this.transform.position.x, (float)Yposition + this.transform.position.y, 0.0f);
            platforms[i] = Instantiate(platformPrefab, positions[i], Quaternion.identity);

            BoxCollider2D boxCollider = platforms[i].AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            platforms[i].tag = "MovingPlatform";
        }
    }
    // Update is called once per frame
    void Update()
    {
        float radius = 5.0f;
        for (int i = 0; i < PLATFORMS_NUM; i++)
        {
            angles[i]+=rotateSpeed*(float)Time.deltaTime;
            double Xposition = radius * Math.Cos(angles[i]);
            double Yposition = radius * Math.Sin(angles[i]);
            platforms[i].transform.position = new Vector3((float)Xposition + this.transform.position.x, (float)Yposition + this.transform.position.y, 0.0f);
        }
    }
}
