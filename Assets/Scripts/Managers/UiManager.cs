using TMPro;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager UiManagerInstance { get; private set; }

    [Header("GeneralStuff")]
    [SerializeField] private RectTransform optionsPanel;
    [SerializeField] private Camera mainCam;

    [Header("Storage")]
    [SerializeField] private TextMeshProUGUI wheatCountTxt;
    [SerializeField] private TextMeshProUGUI flourCountTxt;
    [SerializeField] private TextMeshProUGUI breadCountTxt;

    [Header("SliderPanel")]
    [SerializeField] private TextMeshProUGUI productionAmountTxt;
    [SerializeField] private TextMeshProUGUI producingInfoTxt;
    [SerializeField] private TextMeshProUGUI currentQueueTxt;
    [SerializeField] private TextMeshProUGUI maxQueueTxt;
    [SerializeField] private Slider productionSlider;
    [SerializeField] private Image buildTypeImg;

    [Header("OrderBtn")]
    [SerializeField] private TextMeshProUGUI increaseBtnCount;
    [SerializeField] private Image increaseBtnImg;
    [SerializeField] private Button increaseBtn;
    [SerializeField] private Button decreaseBtn;

    private BuildingsBase currentBuilding;
    private CompositeDisposable panelSubscriptions = new CompositeDisposable();

    private void Awake()
    {
        if (UiManagerInstance is null)
            UiManagerInstance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StorageManager.GetResourceProperty(StorageManager.ResourceType.Wheat).Subscribe(count => wheatCountTxt.text = count.ToString()).AddTo(this);
        StorageManager.GetResourceProperty(StorageManager.ResourceType.Flour).Subscribe(count => flourCountTxt.text = count.ToString()).AddTo(this);
        StorageManager.GetResourceProperty(StorageManager.ResourceType.Bread).Subscribe(count => breadCountTxt.text = count.ToString()).AddTo(this);
    }

    public void ShowBuildingPanel(BuildingsBase building)
    {
        if (currentBuilding is not null && currentBuilding != building) currentBuilding.IsPanelOpened = false;

        panelSubscriptions.Dispose();
        panelSubscriptions = new CompositeDisposable();

        optionsPanel.gameObject.SetActive(true);
        optionsPanel.position = mainCam.WorldToScreenPoint(building.transform.position);
        currentBuilding = building;

        Observable.CombineLatest(currentBuilding.ProductionProgress, currentBuilding.InternalStorage,
            (progress, storage) => new { progress, storage })
            .Subscribe(x => UpdateBuildingPanelUI(x.progress, x.storage, currentBuilding))
            .AddTo(panelSubscriptions);

        if (currentBuilding is ResourceRequiringBuilding)
        {
            maxQueueTxt.gameObject.SetActive(true);
            currentQueueTxt.gameObject.SetActive(true);
        }
        else
        {
            maxQueueTxt.gameObject.SetActive(false);
            currentQueueTxt.gameObject.SetActive(false);
        }

        buildTypeImg.sprite = currentBuilding.outputResourcesSprite;

        if (currentBuilding is ResourceRequiringBuilding resourceBuilding)
        {
            increaseBtn.gameObject.SetActive(true);
            decreaseBtn.gameObject.SetActive(true);
            increaseBtnImg.gameObject.SetActive(true);
            increaseBtnCount.gameObject.SetActive(true);

            increaseBtnImg.sprite = resourceBuilding.NeededResourceSprite;
            increaseBtnCount.text = resourceBuilding.InputCostPerOrder.ToString();

            maxQueueTxt.text = currentBuilding.ProductionQueueCapacity.ToString();
            currentBuilding.ProductionQueue
                .Subscribe(queue => currentQueueTxt.text = queue.ToString())
                .AddTo(panelSubscriptions);
        }
        else
        {
            increaseBtn.gameObject.SetActive(false);
            decreaseBtn.gameObject.SetActive(false);
            increaseBtnImg.gameObject.SetActive(false);
            increaseBtnCount.gameObject.SetActive(false);
        }
    }

    private void UpdateBuildingPanelUI(float progress, int storage, BuildingsBase building)
    {
        productionSlider.value = progress;
        productionAmountTxt.text = storage.ToString();

        if (storage >= building.ProductionQueueCapacity)
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
        if (currentBuilding is ResourceRequiringBuilding resourceBuilding)
            resourceBuilding.EnqueueProductionOrder();
    }

    public void DecreaseProduce()
    {
        if (currentBuilding is ResourceRequiringBuilding resourceBuilding)
            resourceBuilding.DequeueProductionOrder();
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