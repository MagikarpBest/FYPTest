using System;
using System.Collections.Generic;
using UnityEngine;

public class HierarchicalStateMachine
{
    public HSMState Root { get; private set; }
    private bool started;
    
    public void Init(HSMState root)
    {
        Root = root;
        Root.Enter();
        started = true;
    }

    //concept is that root ticks its active child then the child tick their active childs
    //tree concept
    public void Tick(float deltaTime)
    {
        if (started)
        {
            Root.Tick(deltaTime);
        }
    }

    public void PhysicsTick(float fixedDeltaTime)
    {
        if (started)
        {
            Root.PhysicsTick(fixedDeltaTime);
        }
    }

    public void ChangeState(HSMState from, HSMState to)
    {
        if (from == to || from == null || to == null) return;

        HSMState lca = Lca(from, to);

        // Exit current branch up to (but not including) LCA
        for (HSMState s = from; s != lca; s = s.Parent) s.Exit();

        // Enter target branch from LCA down to target
        var stack = new Stack<HSMState>();
        for (HSMState s = to; s != lca; s = s.Parent) stack.Push(s);
        while (stack.Count > 0) stack.Pop().Enter();

        // Print hierarchy after the transition
        //PrintActiveHierarchy();
    }

    // Compute the Lowest Common Ancestor of two states.
    public static HSMState Lca(HSMState a, HSMState b)
    {
        // Create a set of all parents of 'a'
        var ap = new HashSet<HSMState>();
        for (var s = a; s != null; s = s.Parent) ap.Add(s);

        // Find the first parent of 'b' that is also a parent of 'a'
        for (var s = b; s != null; s = s.Parent)
            if (ap.Contains(s)) return s;

        // If no common ancestor found, return null
        return null;
    }

    //For debug
    private void PrintActiveHierarchy()
    {
        HSMState current = Root;

        string hierarchy = current.GetType().Name;

        while (current.ActiveChild != null)
        {
            current = current.ActiveChild;
            hierarchy += " > " + current.GetType().Name;
        }

        Debug.Log($"State Hierarchy: {hierarchy}");
    }
}
