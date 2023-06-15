using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public string[] boardSizeOptions = {
        "Tiny (3x3)",
        "Classic (4x4)",
        "Big (5x5)",
        "Bigger (6x6)",
        "Huge (7x7)"
    };

    public Sprite[] boardSizeSprites = new Sprite[5];

    public TMP_Text boardSizeText;
    public Image boardSizeImage;
    public static int boardSizeSelection = 1;

    // Start is called before the first frame update
    void Start()
    {
        UpdateMenuUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NavigateBoardSize(bool moveRight)
    {
        boardSizeSelection += moveRight ? 1 : -1;
        if (boardSizeSelection > boardSizeOptions.Length - 1)
        {
            boardSizeSelection = 0;
        }
        else if (boardSizeSelection < 0)
        {
            boardSizeSelection = boardSizeOptions.Length - 1;
        }

        UpdateMenuUI();
    }

    public void UpdateMenuUI()
    {
        boardSizeText.text = boardSizeOptions[boardSizeSelection];
        boardSizeImage.sprite = boardSizeSprites[boardSizeSelection];
    }

    public void StartGame()
    {
        SceneManager.LoadScene("2048 Game", LoadSceneMode.Single);
    }
}
