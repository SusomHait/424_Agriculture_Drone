using UnityEngine;

public class CropFieldGenerator : MonoBehaviour
{
    [Header("Patch Size")]
    public int rowsPerCrop = 3;
    public int cropsPerRow = 50;

    [Header("Spacing")]
    public float cropSpacingX = 1.5f;
    public float rowSpacingZ = 1.5f;
    public float patchGapX = 3f;

    [Header("Variation")]
    public float randomOffset = 0.2f;
    public float randomYRotation = 10f;
    public Vector2 randomScaleRange = new Vector2(0.9f, 1.1f);

    [Header("Assets")]
    public GameObject carrot;
    public GameObject corn;
    public GameObject eggplant;
    public GameObject pumpkin;
    public GameObject tomato;
    public GameObject turnip;
    public GameObject dirtMound;
    public GameObject dirtPatch; 

    // make a grid for the crops
    [ContextMenu("Generate Field")]
    public void GenerateField()
    {
        GameObject[] crops = new GameObject[] { carrot, corn, eggplant, pumpkin, tomato, turnip };

        for (int cropType = 0; cropType < crops.Length; cropType++)
        {
            GameObject currentCrop = crops[cropType];

            // go and determine the size of the crop patch
            float patchStartX = cropType * ((cropsPerRow * cropSpacingX) + patchGapX);

            for (int row = 0; row < rowsPerCrop; row++)
            {
                for (int col = 0; col < cropsPerRow; col++)
                {
                    // calc the positions
                    Vector3 localPos = new Vector3(
                        patchStartX + col * cropSpacingX + Random.Range(-randomOffset, randomOffset),
                        0f,
                        row * rowSpacingZ + Random.Range(-randomOffset, randomOffset)
                    );

                    Vector3 worldPos = transform.TransformPoint(localPos);

                    // create the crops and then place them
                    GameObject crop = Instantiate(currentCrop, worldPos, Quaternion.identity, transform);
                    GameObject mound = Instantiate(dirtMound, worldPos, Quaternion.identity, transform);
                    GameObject ground = Instantiate(dirtPatch, new Vector3(worldPos.x, worldPos.y + 0.02f, worldPos.z), Quaternion.identity, transform);

                    float yRot = Random.Range(-randomYRotation, randomYRotation);
                    crop.transform.Rotate(0f, yRot, 0f);
                    mound.transform.Rotate(0f, yRot, 0f);
                    ground.transform.Rotate(0f, yRot, 0f);

                    float scale = Random.Range(randomScaleRange.x, randomScaleRange.y);
                    crop.transform.localScale *= scale;
                    mound.transform.localScale *= scale;
                    ground.transform.localScale *= scale * 2;
                }
            }
        }
    }
}