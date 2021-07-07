using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PMeter : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private Sprite[] arrowSprites;
    [SerializeField]
    private Sprite[] pSprites;

    [SerializeField]
    private Image[] arrows;
    [SerializeField]
    private Image p;

    [SerializeField]
    private float flickerDur;
    private float flickerTimer;

    private int flickerState;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.pActive)
        {
            for (int i = 0; i < arrows.Length; i++)
            {
                if ((int)player.pMeter <= i)
                {
                    arrows[i].sprite = arrowSprites[0];
                }
                else
                {
                    arrows[i].sprite = arrowSprites[1];
                }
            }

            p.sprite = pSprites[0];
        }
        else
        {
            if (flickerTimer > 0.0f)
                flickerTimer -= Time.deltaTime;
            else
            {
                flickerState = flickerState == 1 ? 0 : 1;
                flickerTimer = flickerDur;

                foreach (Image image in arrows)
                {
                    image.sprite = arrowSprites[flickerState];
                }

                p.sprite = pSprites[flickerState];
            }
        }
    }
}
