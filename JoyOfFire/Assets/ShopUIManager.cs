using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    // 面板引用
    public GameObject ShopPanel;
    public GameObject RefleshDoubleCheck;
    public GameObject DoubleCheckConsumable;
    public GameObject DoubleCheckNonConsumable;
    public GameObject ToolDetail;

    // 主界面按钮
    public Button BackButton;
    public Button RefleshButton;

    // 刷新确认面板按钮
    public Button RefleshNoButton;
    public Button RefleshYesButton;

    // 消耗品确认面板按钮
    public Button DoubleCheckConsumableYes;
    public Button DoubleCheckConsumableNo;

    // 非消耗品确认面板按钮
    public Button DoubleCheckNonConsumableYes;
    public Button DoubleCheckNonConsumableNo;

    // 道具详情面板按钮
    public Button ToolButton;
    public Button ToolDetailBackButton;
    public Button ToolDetailPurchaseButton;

    void Start()
    {
        // 注册按钮事件
        BackButton.onClick.AddListener(OnBackButtonClick);
        RefleshButton.onClick.AddListener(OnRefleshButtonClick);

        RefleshNoButton.onClick.AddListener(OnRefleshNoClick);
        RefleshYesButton.onClick.AddListener(OnRefleshYesClick);

        DoubleCheckConsumableYes.onClick.AddListener(OnConsumableYesClick);
        DoubleCheckConsumableNo.onClick.AddListener(OnConsumableNoClick);

        DoubleCheckNonConsumableYes.onClick.AddListener(OnNonConsumableYesClick);
        DoubleCheckNonConsumableNo.onClick.AddListener(OnNonConsumableNoClick);

        ToolDetailBackButton.onClick.AddListener(OnToolDetailBackClick);
        ToolDetailPurchaseButton.onClick.AddListener(OnToolDetailPurchaseClick);
    }

    void OnBackButtonClick()
    {
        // 点击BackButton，关闭ShopPanel
        ShopPanel.SetActive(false);
    }

    void OnRefleshButtonClick()
    {
        // 点击RefleshButton，打开Refleshdoublecheck
        RefleshDoubleCheck.SetActive(true);
    }

    void OnRefleshNoClick()
    {
        // 点击Refleshdoublecheck里的No，关闭Refleshdoublecheck
        RefleshDoubleCheck.SetActive(false);
    }

    void OnRefleshYesClick()
    {
        // 点击Refleshdoublecheck里的Yes，刷新商店（只写注释，不做效果）
        // TODO: 执行商店刷新逻辑
        RefleshDoubleCheck.SetActive(false);
    }

    void OnConsumableYesClick()
    {
        // 点击DoubleCheckConsumable里的Yes时进行购买（只写注释，不做效果）
        // TODO: 执行可消耗道具的购买逻辑
        DoubleCheckConsumable.SetActive(false);
    }

    void OnConsumableNoClick()
    {
        // 点击DoubleCheckConsumable里的No时关闭DoubleCheckConsumable
        DoubleCheckConsumable.SetActive(false);
    }

    void OnNonConsumableYesClick()
    {
        // 点击DoubleCheckNon-consumable里的Yes时进行购买（只写注释，不做效果）
        // TODO: 执行不可消耗道具的购买逻辑
        DoubleCheckNonConsumable.SetActive(false);
    }

    void OnNonConsumableNoClick()
    {
        // 点击DoubleCheckNon-consumable里的No时关闭DoubleCheckNon-consumable
        DoubleCheckNonConsumable.SetActive(false);
    }

    void OnToolDetailBackClick()
    {
        // 点击ToolDetail里的BackButton时，关闭ToolDetail
        ToolDetail.SetActive(false);
    }

    void OnToolDetailPurchaseClick()
    {
        // 点击ToolDetail里的PurchaseButton时，根据是否可消耗决定弹出哪个确认框
        bool isConsumable = false; // TODO: 判断当前道具是否是可消耗的
        if (isConsumable)
        {
            DoubleCheckConsumable.SetActive(true);
        }
        else
        {
            DoubleCheckNonConsumable.SetActive(true);
        }
    }

    public void OnToolButtonClick()
    {
        // 点击ToolPrefab里的ToolButton，打开ToolDetail
        ToolDetail.SetActive(true);
    }

    public void OnToolPurchaseButtonClick(bool isConsumable)
    {
        // 点击ToolPrefab里的PurchaseButton，根据道具类型弹出对应确认面板
        if (isConsumable)
        {
            DoubleCheckConsumable.SetActive(true);
        }
        else
        {
            DoubleCheckNonConsumable.SetActive(true);
        }
    }
}
