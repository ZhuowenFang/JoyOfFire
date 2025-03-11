using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class SpeedBarUI : MonoBehaviour
{
    [System.Serializable]
    public class SpeedCharacter
    {
        public ICharacter character;
        public int index;
        public RectTransform iconTransform;     // 图标对象
        public TMP_Text numberText;             // 图标编号显示
    }

    public RectTransform speedArea;             // 速度区域背景
    public RectTransform divider;               // 分界线
    public GameObject characterIconPrefab;      // 图标预制件

    public List<SpeedCharacter> speedCharacters = new List<SpeedCharacter>();  // 管理角色列表
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
        speedAreaHeight = speedArea.rect.height;
        InitializeCharacters();
        SortIconsByTimePoint();
        UpdateDividerPosition();
    }

    private void InitializeCharacters()
    {
        for (int i = 0; i < BattleCharacterManager.instance.allCharacters.Count; i++)
        {
            ICharacter character = BattleCharacterManager.instance.allCharacters[i];

            GameObject icon = Instantiate(characterIconPrefab, speedArea);
            RectTransform iconTransform = icon.GetComponent<RectTransform>();
            if (character is CharacterAttributes ca)
            {
                StartCoroutine(ImageCache.GetTexture(ca.character_picture, (Texture2D texture) =>
                {
                    if (texture != null)
                    {
                        icon.GetComponent<Image>().sprite = Sprite.Create(texture, 
                            new Rect(0, 0, texture.width, texture.height), 
                            new Vector2(0.5f, 0.5f));
                    }
                }));
            }
            else if(character is MonsterAttributes enemy)
            {
                icon.GetComponent<Image>().sprite = Resources.Load<Sprite>($"EnemyPics/{enemy.monsterId}");
            }
            TMP_Text numberText = icon.GetComponentInChildren<TMP_Text>();

            numberText.text = (i + 1).ToString();

            speedCharacters.Add(new SpeedCharacter
            {
                character = character,
                index = character.index,
                iconTransform = iconTransform,
                numberText = numberText
            });

            character.timePoint = MaxDistance / character.speed;
        }
    }

    public void UpdateSpeedBar()
    {
        SortIconsByTimePoint();
        UpdateDividerPosition();
    }

    public ICharacter GetNextCharacter()
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
            BattleManager.instance.UpdateTotalEnergy();
        }
    }

    /// <summary>
    /// 从速度条中删除指定角色
    /// </summary>
    public void RemoveCharacter(ICharacter character)
    {
        SpeedCharacter toRemove = speedCharacters.Find(sc => sc.character == character);
        if (toRemove != null)
        {
            speedCharacters.Remove(toRemove);
            if (toRemove.iconTransform != null)
            {
                Destroy(toRemove.iconTransform.gameObject);
            }
            // 重新排序并更新分界线
            SortIconsByTimePoint();
            UpdateDividerPosition();
        }
    }
}
