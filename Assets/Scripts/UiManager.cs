using TMPro;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager UiManagerInstance;

    [SerializeField] private TextMeshProUGUI wheatCountTxt;
    [SerializeField] private TextMeshProUGUI flourCountTxt;
    [SerializeField] private TextMeshProUGUI breadCountTxt;
    [SerializeField] private TextMeshProUGUI productionAmountTxt;
    [SerializeField] private TextMeshProUGUI producingInfoTxt;
    [SerializeField] private Slider productionSlider;

    [SerializeField] private RectTransform optionsPanel;
    [SerializeField] private Camera mainCam;

    private BuildingsBase currentBuilding;
    private CompositeDisposable buildingPanelDisposable = new CompositeDisposable();

    private void Awake()
    {
        if (UiManagerInstance == null) UiManagerInstance = this;
    }

    private void Start()
    {
        Storage.WheatCount.Subscribe(count => wheatCountTxt.SetText("{0}", count)).AddTo(this);
        Storage.FlourCount.Subscribe(count => flourCountTxt.SetText("{0}", count)).AddTo(this);
        Storage.BreadCount.Subscribe(count => breadCountTxt.SetText("{0}", count)).AddTo(this);
    }

    public void ShowBuildingPanel(BuildingsBase building)
    {
        if (currentBuilding is not null && currentBuilding != building)
        {
            currentBuilding.isPanelOpened = false;
        }

        buildingPanelDisposable.Dispose();
        buildingPanelDisposable = new CompositeDisposable();

        optionsPanel.gameObject.SetActive(true);
        optionsPanel.position = mainCam.WorldToScreenPoint(building.transform.position);
        currentBuilding = building;

        currentBuilding.ProductionProgress.Subscribe(progress =>
        {
            productionAmountTxt.text = currentBuilding.InternalStorage.Value.ToString();
            if (currentBuilding.InternalStorage.Value >= currentBuilding.Capacity)
            {
                producingInfoTxt.text = "Full";
                productionSlider.value = 1f;
            }
            else
            {
                int remainingSeconds = Mathf.CeilToInt(currentBuilding.ProductionTime * (1 - progress));
                producingInfoTxt.text = $"{remainingSeconds} sn";
                productionSlider.value = progress;
            }
        }).AddTo(buildingPanelDisposable);
    }

    public void IncreaseProduce()
    {
        currentBuilding?.EnqueueProductionOrder();
    }

    public void DecreaseProduce()
    {
        currentBuilding?.CancelProductionOrder();
    }

    public void HideBuildingPanel()
    {
        optionsPanel.gameObject.SetActive(false);
        buildingPanelDisposable.Dispose();
    }
}