using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int value = 0;

    public TMP_Text myText;
    public Image myImage;
    public GameObject replica;

    public float moveTimeInSeconds = 0.1f;

    [HideInInspector]
    public bool merged;

    public Color[] tileColours;

    private GameplayManager gameplayManager;
    private ScoreManager scoreManager;



    // Start is called before the first frame update
    void Start()
    {
        gameplayManager = GetComponentInParent<GameplayManager>();
        scoreManager = gameplayManager.scoreManager;
        replica.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignValue(int val, bool updateVisual=true)
    {
        value = val;
        if (updateVisual)
        {
            UpdateVisual();
        }
    }

    public void Merge()
    {
        value *= 2;
        merged = true;
        scoreManager.AddToScore(value);
    }

    public void ResetMerged()
    {
        merged = false;
    }

    public void UpdateVisual()
    {
        if (value == 0)
        {
            myText.text = "";
            myImage.color = Color.white;
        }
        else
        {
            myText.text = value.ToString();
            myImage.color = tileColours[(int) Mathf.Log(value, 2)];
        }
    }

    public void MoveTile(Tile endTile, int val)
    {
        if (endTile == this)
        {
            AssignValue(val);
            return;
        }
        else
        {
            if (endTile.value != val)
            {
                endTile.AssignValue(val, false);
            }
            else
            {
                endTile.Merge();
            }
        }
        AssignValue(0);
        StartCoroutine(MoveTileCoroutine(endTile, val));
    }

    private IEnumerator MoveTileCoroutine(Tile endTile, int val)
    {
        
        gameplayManager.AddMovingTile(this);

        

        replica.SetActive(true);
        replica.GetComponent<Image>().color = tileColours[(int)Mathf.Log(val, 2)];
        replica.GetComponentInChildren<TMP_Text>().text = val.ToString();

        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        Vector3 endingPos = endTile.transform.position;

        replica.transform.position = startingPos;

        while (elapsedTime < moveTimeInSeconds)
        {
            replica.transform.position = Vector3.Lerp(startingPos, endingPos, (elapsedTime / moveTimeInSeconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        replica.SetActive(false);

        gameplayManager.RemoveMovingTile(this);

        endTile.UpdateVisual();

    }
}
