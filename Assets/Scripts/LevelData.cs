using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelData : MonoBehaviour {

    [SerializeField]
    public int carrots;
    [SerializeField]
    public int tomatos;
    [SerializeField]
    public int bananas;

    private Text levelText;
    private Text vegetableText;

    void Start()
    {
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        vegetableText = GameObject.Find("VegetableText").GetComponent<Text>();

        levelText.text = SceneManager.GetActiveScene().name;
        vegetableText.text = "Carrots: " + carrots + "   Tomatos: " + tomatos + "   Bananas: " + bananas;
    }

    public void SetData(int carrots, int tomatos, int bananas)
    {
        this.carrots = carrots;
        this.tomatos = tomatos;
        this.bananas = bananas;

        vegetableText.text = "Carrots: " + carrots + "   Tomatos: " + tomatos + "   Bananas: " + bananas;
    }
}
