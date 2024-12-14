using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public SpeedBarUI speedBarUI;           // 速度条管理器
    public RectTransform actionPanel;       // 操作栏
    public Button attackButton;             // 普通攻击按钮
    public Button endTurnButton;            // 结束回合按钮
    public List<Button> characterButtons;   // 玩家与敌人的按钮列表
    public List<Button> enemyButtons;       // 敌方角色按钮列表

    private bool isSelectingEnemy = false;  // 是否正在选择敌人
    private float animationTimer = 0f;      // 动画计时器
    public static BattleManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (var button in characterButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        // 动态绑定敌人按钮点击事件
        for (int i = 0; i < enemyButtons.Count; i++)
        {
            int index = i;
            enemyButtons[i].onClick.AddListener(() => OnEnemySelected(index));
        }

        attackButton.onClick.AddListener(StartEnemySelection);
        endTurnButton.onClick.AddListener(EndTurn);
        StartNextTurn();
    }

    public async void StartNextTurn()
    {
        actionPanel.gameObject.SetActive(false);
        ResetAllCharacterSizes();

        CharacterAttributes nextCharacter = speedBarUI.GetNextCharacter();
        if (nextCharacter == null)
        {
            Debug.Log("没有角色进行回合！");
            return;
        }

        Debug.Log($"{nextCharacter.index} 开始行动！");

        int characterIndex = nextCharacter.index;

        if (characterIndex >= 3)
        {
            characterButtons[characterIndex].transform.localScale = Vector3.one * 1.2f;
            await Task.Delay(1000);
            Debug.Log($"敌人 {characterIndex + 1} 行动完成！");
            speedBarUI.CompleteCurrentTurn();
            StartNextTurn();
        }
        else
        {
            actionPanel.gameObject.SetActive(true);
            characterButtons[characterIndex].transform.localScale = Vector3.one * 1.2f;
        }
    }

    public void StartEnemySelection()
    {
        Debug.Log("开始选择攻击目标！");
        isSelectingEnemy = true;
        actionPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isSelectingEnemy)
        {
            AnimateEnemySelection();
        }
    }

    private void AnimateEnemySelection()
    {
        animationTimer += Time.deltaTime * 2f;
        float scale = 1f + Mathf.Sin(animationTimer) * 0.2f;

        foreach (var enemy in enemyButtons)
        {
            enemy.transform.localScale = Vector3.one * scale;  // 敌人图片缩放动画
        }
    }

    public void OnEnemySelected(int enemyIndex)
    {
        if (!isSelectingEnemy) return;

        Debug.Log($"攻击敌人 {enemyIndex + 4}！");
        DealDamage(enemyIndex);

        isSelectingEnemy = false;
        ResetAllCharacterSizes();

        EndTurn();
    }

    private void DealDamage(int enemyIndex)
    {
        Debug.Log($"对敌人 {enemyIndex + 4} 造成 10 点伤害！");
    }

    private void ResetAllCharacterSizes()
    {
        foreach (var button in characterButtons)
        {
            button.transform.localScale = Vector3.one;  // 重置图片大小
        }
    }

    public void EndTurn()
    {
        Debug.Log("回合结束！");
        ResetAllCharacterSizes();
        speedBarUI.CompleteCurrentTurn();
        StartNextTurn();
    }
}
