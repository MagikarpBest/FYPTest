using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using System.Linq;

public class GameplayTagDropdown : AdvancedDropdown
{
    private readonly string[] tagNames;
    private readonly Action<string> onSelected;

    public GameplayTagDropdown(AdvancedDropdownState state, string[] tagNames, Action<string> onSelected) : base(state)
    {
        this.tagNames = tagNames;
        this.onSelected = onSelected;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Gameplay Tags");
        var dropdownItemDict = new Dictionary<string, AdvancedDropdownItem>(); //key is all the tags then the coresponding dropdownitem
        var hasChildren = new HashSet<string>();
        
        foreach (string tag in tagNames)
        {
            if (string.IsNullOrEmpty(tag)) continue;

            AdvancedDropdownItem parentItem = root;
            string currentTag = "";

            //split the tag
            foreach (string segment in tag.Split('.'))
            {
                string parentTag = currentTag;

                //build the tag from left to right
                currentTag = currentTag.Length == 0 ? segment : currentTag + "." + segment;
                
                //create new item if it doesnt exist then add to dict
                if (!dropdownItemDict.TryGetValue(currentTag, out AdvancedDropdownItem dropdownItem))
                {
                    dropdownItem = new AdvancedDropdownItem(currentTag);
                    parentItem.AddChild(dropdownItem);
                    dropdownItemDict[currentTag] = dropdownItem;

                    if (parentTag.Length > 0) hasChildren.Add(parentTag);
                }

                parentItem = dropdownItem;
            }
        }

        foreach (string tag in hasChildren)
        {
            //prevent non registered tags
            if (tagNames.Contains(tag))
            {
                dropdownItemDict[tag].AddChild(new AdvancedDropdownItem(tag));
            }
        }

        return root;
    }
    
    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        onSelected(item.name);
    }
    
}
