using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    // �������
    public GameObject ShopPanel;
    public GameObject RefleshDoubleCheck;
    public GameObject DoubleCheckConsumable;
    public GameObject DoubleCheckNonConsumable;
    public GameObject ToolDetail;

    // �����水ť
    public Button BackButton;
    public Button RefleshButton;

    // ˢ��ȷ����尴ť
    public Button RefleshNoButton;
    public Button RefleshYesButton;

    // ����Ʒȷ����尴ť
    public Button DoubleCheckConsumableYes;
    public Button DoubleCheckConsumableNo;

    // ������Ʒȷ����尴ť
    public Button DoubleCheckNonConsumableYes;
    public Button DoubleCheckNonConsumableNo;

    // ����������尴ť
    public Button ToolButton;
    public Button ToolDetailBackButton;
    public Button ToolDetailPurchaseButton;

    void Start()
    {
        // ע�ᰴť�¼�
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
        // ���BackButton���ر�ShopPanel
        ShopPanel.SetActive(false);
    }

    void OnRefleshButtonClick()
    {
        // ���RefleshButton����Refleshdoublecheck
        RefleshDoubleCheck.SetActive(true);
    }

    void OnRefleshNoClick()
    {
        // ���Refleshdoublecheck���No���ر�Refleshdoublecheck
        RefleshDoubleCheck.SetActive(false);
    }

    void OnRefleshYesClick()
    {
        // ���Refleshdoublecheck���Yes��ˢ���̵ֻ꣨дע�ͣ�����Ч����
        // TODO: ִ���̵�ˢ���߼�
        RefleshDoubleCheck.SetActive(false);
    }

    void OnConsumableYesClick()
    {
        // ���DoubleCheckConsumable���Yesʱ���й���ֻдע�ͣ�����Ч����
        // TODO: ִ�п����ĵ��ߵĹ����߼�
        DoubleCheckConsumable.SetActive(false);
    }

    void OnConsumableNoClick()
    {
        // ���DoubleCheckConsumable���Noʱ�ر�DoubleCheckConsumable
        DoubleCheckConsumable.SetActive(false);
    }

    void OnNonConsumableYesClick()
    {
        // ���DoubleCheckNon-consumable���Yesʱ���й���ֻдע�ͣ�����Ч����
        // TODO: ִ�в������ĵ��ߵĹ����߼�
        DoubleCheckNonConsumable.SetActive(false);
    }

    void OnNonConsumableNoClick()
    {
        // ���DoubleCheckNon-consumable���Noʱ�ر�DoubleCheckNon-consumable
        DoubleCheckNonConsumable.SetActive(false);
    }

    void OnToolDetailBackClick()
    {
        // ���ToolDetail���BackButtonʱ���ر�ToolDetail
        ToolDetail.SetActive(false);
    }

    void OnToolDetailPurchaseClick()
    {
        // ���ToolDetail���PurchaseButtonʱ�������Ƿ�����ľ��������ĸ�ȷ�Ͽ�
        bool isConsumable = false; // TODO: �жϵ�ǰ�����Ƿ��ǿ����ĵ�
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
        // ���ToolPrefab���ToolButton����ToolDetail
        ToolDetail.SetActive(true);
    }

    public void OnToolPurchaseButtonClick(bool isConsumable)
    {
        // ���ToolPrefab���PurchaseButton�����ݵ������͵�����Ӧȷ�����
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
