using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedBarUI : MonoBehaviour
{
    [System.Serializable]
    public class SpeedCharacter
    {
        public CharacterAttributes character;    // 角色属性数据
        public RectTransform iconTransform;     // 图标对象
        public TMP_Text numberText;             // 图标编号显示
    }

    public RectTransform speedArea;             // 速度区域背景
    public RectTransform divider;               // 分界线
    public GameObject characterIconPrefab;      // 图标预制件

    private List<SpeedCharacter> speedCharacters = new List<SpeedCharacter>();  // 管理角色列表
    private CharacterManager characterManager;   // 引用角色管理器
    private float speedAreaHeight;               // 速度区域高度

    private const float MaxDistance = 5000f;    // 固定路程
    private const float TurnTime = 100f;        // 每回合最大时间
    private int roundNumber = 1;                // 当前回合数
    public static SpeedBarUI instance;
    
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        speedAreaHeight = speedArea.rect.height;
        InitializeCharacters();
        SortIconsByTimePoint();
        UpdateDividerPosition();
        // BattleManager.instance.StartNextTurn();
        
    }

    private void InitializeCharacters()
    {
        for (int i = 0; i < characterManager.allCharacters.Count; i++)
        {
            CharacterAttributes character = characterManager.allCharacters[i];

            // 创建角色图标
            GameObject icon = Instantiate(characterIconPrefab, speedArea);
            RectTransform iconTransform = icon.GetComponent<RectTransform>();
            TMP_Text numberText = icon.GetComponentInChildren<TMP_Text>();

            // 初始化编号显示
            numberText.text = (i + 1).ToString();

            // 添加到管理列表
            speedCharacters.Add(new SpeedCharacter
            {
                character = character,
                iconTransform = iconTransform,
                numberText = numberText
            });

            // 初始化时间点
            character.timePoint = MaxDistance / character.speed;
        }
    }

    public void UpdateSpeedBar()
    {
        SortIconsByTimePoint();
        UpdateDividerPosition();
    }

    public CharacterAttributes GetNextCharacter()
    {
        return speedCharacters.Count > 0 ? speedCharacters[0].character : null;
    }

    public void CompleteCurrentTurn()
    {
        if (speedCharacters.Count == 0) return;

        SpeedCharacter currentCharacter = speedCharacters[0];
        currentCharacter.character.timePoint += MaxDistance / currentCharacter.character.speed;

        UpdateRoundNumber();
        UpdateSpeedBar();
    }

    private void SortIconsByTimePoint()
    {
        speedCharacters.Sort((a, b) => a.character.timePoint.CompareTo(b.character.timePoint));

        for (int i = 0; i < speedCharacters.Count; i++)
        {
            UpdateIconPosition(speedCharacters[i].iconTransform, i);
        }
    }

    private void UpdateIconPosition(RectTransform icon, int index)
    {
        float iconHeight = speedAreaHeight / speedCharacters.Count;
        float yPos = speedAreaHeight / 2f - (index + 0.5f) * iconHeight;
        icon.localPosition = new Vector3(0, yPos, 0);
    }
    
    private void UpdateDividerPosition()
    {
        float currentTurnTime = TurnTime * roundNumber;
        int lastIndex = -1;

        for (int i = 0; i < speedCharacters.Count; i++)
        {
            if (speedCharacters[i].character.timePoint <= currentTurnTime)
            {
                lastIndex = i;
            }
        }

        if (lastIndex >= 0)
        {
            float iconHeight = speedAreaHeight / speedCharacters.Count;
            float yPos = speedAreaHeight / 2f - (lastIndex + 1) * iconHeight;
            divider.localPosition = new Vector3(0, yPos, 0);
        }
        else
        {
            float lastYPos = speedAreaHeight / 2f - speedCharacters.Count * (speedAreaHeight / speedCharacters.Count);
            divider.localPosition = new Vector3(0, lastYPos, 0);
        }
    }

    private void UpdateRoundNumber()
    {
        float requiredTime = TurnTime * roundNumber;

        bool allBeyondCurrentRound = true;
        foreach (var character in speedCharacters)
        {
            if (character.character.timePoint <= requiredTime)
            {
                allBeyondCurrentRound = false;
                break;
            }
        }

        if (allBeyondCurrentRound)
        {
            roundNumber++;
            Debug.Log($"进入第 {roundNumber} 回合！");
        }
    }
}
