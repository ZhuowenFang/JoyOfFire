using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelInfo
    {
        public string level_desc;               // 关卡描述
        public string level_name;               // 关卡名称
        public string option_a_text;            // 选项 A 的文本
        public string option_b_text;            // 选项 B 的文本
        public string option_c_text;            // 选项 C 的文本
        public string option_a_experience;      // 选项 A 的经验描述
        public string option_b_experience;      // 选项 B 的经验描述
        public string option_c_experience;      // 选项 C 的经验描述
        public string option_a_res;             // 选项 A 的结果（JSON 格式）
        public string option_b_res;             // 选项 B 的结果（JSON 格式）
        public string option_c_res;             // 选项 C 的结果（JSON 格式）
        public string option_a_type;            // 选项 A 的类型
        public string option_b_type;            // 选项 B 的类型
        public string option_c_type;            // 选项 C 的类型
        public string level_pre_cond;           // 前置条件
        public string stick_point;              // 是否为关键点
        public string f19;                      // 备用字段 F19
        public string f20;                      // 备用字段 F20
    }

    [System.Serializable]
    public class LevelData
    {
        public string id;
        public int level_order;
        public int level_num;
        public LevelInfo levelInfo;
    }

    [System.Serializable]
    public class LevelResponse
    {
        public string code;
        public string message;
        public LevelData data;
    }

    [System.Serializable]
    public class OptionResult
    {
        public Dictionary<string, string> tool_required { get; set; }
        public Dictionary<string, string> success_condition { get; set; }
        public string mini_game { get; set; }
        public Dictionary<string, int> monsters { get; set; }
        public Dictionary<string, float> rewards { get; set; }
        public Dictionary<string, float> tressures { get; set; }
        public Dictionary<string, string> success_bless { get; set; }
        public Dictionary<string, string> fail_bless { get; set; }
        public Dictionary<string, string> success_buff { get; set; }
        public string success_copywriting { get; set; }
        public Dictionary<string, string> fail_buff { get; set; }
        public string fail_copywriting { get; set; }
        public string next_stage { get; set; }
        public string unlock_stage { get; set; }
    }
    
    [System.Serializable]
    public class CharacterData
    {
        public string id;
        public string user_id;
        public string character_id;
        public BasicInformation basic_information;
        public string character_picture;
        public List<string> current_ability;
        public List<string> potential_ability;
        public Talent talent1;
        public List<string> talent_count1;
        public Talent talent2;
        public List<string> talent_count2;
        public Talent talent3;
        public List<string> talent_count3;
        public string experience;
    }

    [System.Serializable]
    public class BasicInformation
    {
        public string appearance;
        public string fighting_ability;
        public string gender;
        public string name;
        public string story;
    }

    [System.Serializable]
    public class Talent
    {
        public string abilitydescription;
        public string cost;
        public List<TalentDescription> description;
        public string icon;

    }

    [System.Serializable]
    public class TalentDescription
    {
        public string talent_name;
        public string talent_description;
    }
    [System.Serializable]
    public class CharacterCreationData
    {
        public string userId;
        public string sex; // 性别
        public string profession; // 职业
        public string clothes; // 衣服
        public string combat; // 战斗
        public string other; // 其他
    }
    [System.Serializable]
    public class CharacterUpdateData
    {
        public string user_id;
        public string character_id;
        public string Update;
    }
    [System.Serializable]
    public class MonsterData
    {
        public string id;
        public string level_order;
        public string enemy;  // 怪物名称
        public string monsterId;
        public int base_gold_value;
        public string random;
        public int level;
        public int hp;
        public int san_value;
        public float physical_attack;
        public float physical_defense;
        public float soul_attack;
        public float soul_defense;
        public float speed;
        public float critical_strike_rate;
        public float hit_rate;
        public float tenacity_rate;
    
        public string passive1;
        public string passive2;
    
        public string skill1;
        public string skill1_desc;
        public List<string> skill1_vector;
        public string skill1_icon;
        public int? skill1_cost;
    
        public string skill2;
        public string skill2_desc;
        public List<string> skill2_vector;
        public string skill2_icon;
        public int? skill2_cost;
    
        public string skill3;
        public string skill3_desc;
        public List<string> skill3_vector;
        public string skill3_icon;
        public int? skill3_cost;
    
        public string skill4;
        public string skill4_desc;
        public List<string> skill4_vector;
        public string skill4_icon;
        public int? skill4_cost;
    }
    



}