using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

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
        var dropdownItemDict =
            new Dictionary<string, AdvancedDropdownItem>(); //key is all the tags then the coresponding dropdownitem
        var hasChildren = new HashSet<string>();
        var registeredTags = new HashSet<string>(tagNames); //which tags are actually registered

        foreach (string tag in tagNames)
        {
            if (string.IsNullOrEmpty(tag)) continue;

            AdvancedDropdownItem parentItem = root;
            string currentTag = "";

            //split the tag
            foreach (string segment in tag.Split('.'))
            {
                string parentTag = currentTag;

                //set current tag
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

        foreach (string path in hasChildren)
        {
            if (registeredTags.Contains(path))
            {
                dropdownItemDict[path].AddChild(new AdvancedDropdownItem(path));
            }
        }

        return root;
    }
}
