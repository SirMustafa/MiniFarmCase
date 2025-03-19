using TMPro;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager UiManagerInstance { get; private set; }

    [SerializeField] private TextMeshProUGUI wheatCountTxt;
    [SerializeField] private TextMeshProUGUI flourCountTxt;
    [SerializeField] private TextMeshProUGUI breadCountTxt;
    [SerializeField] private TextMeshProUGUI productionAmountTxt;
    [SerializeField] private TextMeshProUGUI producingInfoTxt;
    [SerializeField] private Slider productionSlider;
    [SerializeField] private RectTransform optionsPanel;
    [SerializeField] private TextMeshProUGUI increaseBtnCount;
    [SerializeField] private Image increaseBtnImg;
    [SerializeField] private Camera mainCam;

    private BuildingsBase currentBuilding;
    private CompositeDisposable panelSubscriptions = new CompositeDisposable();

    private void Awake()
    {
        if (UiManagerInstance is null) UiManagerInstance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StorageManager.WheatCount.Subscribe(count => wheatCountTxt.text = count.ToString()).AddTo(this);
        StorageManager.FlourCount.Subscribe(count => flourCountTxt.text = count.ToString()).AddTo(this);
        StorageManager.BreadCount.Subscribe(count => breadCountTxt.text = count.ToString()).AddTo(this);
    }

    public void ShowBuildingPanel(BuildingsBase building)
    {
        if (currentBuilding != null && currentBuilding != building) currentBuilding.IsPanelOpened = false;

        panelSubscriptions.Dispose();
        panelSubscriptions = new CompositeDisposable();

        optionsPanel.gameObject.SetActive(true);
        optionsPanel.position = mainCam.WorldToScreenPoint(building.transform.position);
        currentBuilding = building;

        Observable.CombineLatest(currentBuilding.ProductionProgress,currentBuilding.InternalStorage,(progress, storage) => new { progress, storage })
        .Subscribe(x => UpdateBuildingPanelUI(x.progress, x.storage, currentBuilding))
        .AddTo(panelSubscriptions);

        if (currentBuilding is INeedResource resourceNeed)
        {
            increaseBtnImg.gameObject.SetActive(true);
            increaseBtnCount.gameObject.SetActive(true);
            increaseBtnImg.sprite = resourceNeed.NeededResourceSprite();
            increaseBtnCount.text = resourceNeed.InputResource().ToString();
        }
        else
        {
            increaseBtnImg.gameObject.SetActive(false);
            increaseBtnCount.gameObject.SetActive(false);
        }
    }

    private void UpdateBuildingPanelUI(float progress, int storage, BuildingsBase building)
    {
        productionSlider.value = progress;
        productionAmountTxt.text = storage.ToString();

        if (storage >= building.Capacity)
        {
            productionSlider.value = 1f;
            producingInfoTxt.text = "Full";
        }
        else if (!building.IsProducing)
        {
            producingInfoTxt.text = $"{Mathf.CeilToInt(building.ProductionTime)} s";
        }
        else
        {
            int remainingSeconds = Mathf.CeilToInt(building.ProductionTime * (1 - progress));
            producingInfoTxt.text = $"{remainingSeconds} s";
        }
    }

    public void IncreaseProduce()
    {
        currentBuilding?.EnqueueProductionOrder();
    }

    public void DecreaseProduce()
    {
        currentBuilding?.DequeueProductionOrder();
    }

    public void HideBuildingPanel()
    {
        optionsPanel.gameObject.SetActive(false);
        panelSubscriptions.Dispose();

        if (currentBuilding is not null)
        {
            currentBuilding.IsPanelOpened = false;
            currentBuilding = null;
        }
    }
}