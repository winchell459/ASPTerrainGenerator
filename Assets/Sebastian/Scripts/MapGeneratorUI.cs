using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneratorUI : MonoBehaviour
{
    [SerializeField] Dropdown drawModeDropDown;
    [SerializeField] InputField mapWidthText, mapHeightText;
    [SerializeField] Slider noiseScaleSlider, octavesSlider, persistanceSlider, lacunaritySlider;
    [SerializeField] InputField noiseScaleLabel, octavesLabel, persistanceLabel, lacunarityLabel;
    [SerializeField] InputField seedText;

    [SerializeField] Sebastian.MapGenerator mapGenerator;

    private Sebastian.MapGenerator.DrawMode drawMode = Sebastian.MapGenerator.DrawMode.NoiseMap;
    private int mapWidth = 400;
    private int mapHeight = 300;
    private float noiseScale = 9;
    private int octaves = 4;
    private float persistance = 1;
    private float lacunarity = 0.25f;
    private int seed = 0;

    public bool autoUpdate = false;

    private void Start()
    {
        UpdateUI();

    }


    public void DrawModeOnValueChanged(int index)
    {
        drawMode = (Sebastian.MapGenerator.DrawMode)index;
    }

    public void MapWidthOnValueChanged(string value)
    {
        mapWidth = int.Parse(value);
        if (autoUpdate) GenerateMap();
    }

    public void MapHeightOnValueChanged(string value)
    {
        mapHeight = int.Parse(value);
        if (autoUpdate) GenerateMap();
    }

    public void NoiseScaleOnValueChanged(float value)
    {
        noiseScale = value;
        noiseScaleLabel.text = noiseScale.ToString();
        if (autoUpdate) GenerateMap();
    }

    public void OctavesOnValueChanged(float value)
    {
        octaves = (int)value;
        octavesLabel.text = octaves.ToString();
        if (autoUpdate) GenerateMap();
    }

    public void PersistanceOnValueChanged(float value)
    {
        persistance = value;
        persistanceLabel.text = persistance.ToString();
        if (autoUpdate) GenerateMap();
    }

    public void LacunarityOnValueChanged(float value)
    {
        lacunarity = value;
        lacunarityLabel.text = lacunarity.ToString();
        if (autoUpdate) GenerateMap();
    }

    public void SeedOnValueChanged(string value)
    {
        seed = int.Parse(value);
        seedText.SetTextWithoutNotify(seed.ToString());
        if (autoUpdate) GenerateMap();
    }

    private void UpdateUI()
    {
        drawModeDropDown.SetValueWithoutNotify((int)drawMode);
        mapWidthText.SetTextWithoutNotify( mapWidth.ToString());
        mapHeightText.SetTextWithoutNotify(mapHeight.ToString());
        noiseScaleSlider.SetValueWithoutNotify(noiseScale);
        noiseScaleLabel.text = noiseScale.ToString();
        octavesSlider.SetValueWithoutNotify(octaves);
        octavesLabel.text = octaves.ToString();
        persistanceSlider.SetValueWithoutNotify(persistance);
        persistanceLabel.text = persistance.ToString();
        lacunaritySlider.SetValueWithoutNotify(lacunarity);
        lacunarityLabel.text = lacunarity.ToString();
        seedText.SetTextWithoutNotify(seed.ToString());
    }

    public void GenerateButton()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        mapGenerator.GenerateMap(drawMode, mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, seed);
    }
}
