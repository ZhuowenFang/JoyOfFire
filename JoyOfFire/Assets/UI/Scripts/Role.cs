using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Role", menuName = "Inventory/New Role")]
public class Role : ScriptableObject
{
    public string roleName; 
    public Sprite roleImage;
    [TextArea]
    public string roleInfo;
}

