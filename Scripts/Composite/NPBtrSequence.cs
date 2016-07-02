﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class NPBtrSequence : NPBtrComposite
{
    private int currentIndex = -1;

    public NPBtrSequence(params NPBtrNode[] children) : base("Sequence", children)
    {
    }

    protected override void DoStart()
    {
        foreach (NPBtrNode child in Children)
        {
            Assert.AreEqual(child.CurrentState, State.INACTIVE);
        }

        currentIndex = -1;

        ProcessChildren();
    }

    protected override void DoStop()
    {
        Children[currentIndex].Stop();
    }


    protected override void DoChildStopped(NPBtrNode child, bool result)
    {
        if (result)
        {
            ProcessChildren();
        }
        else
        {
            Stopped(false);
        }
    }

    private void ProcessChildren()
    {
        if (++currentIndex < Children.Length)
        {
            if (IsStopRequested)
            {
                Stopped(false);
            }
            else
            {
                Children[currentIndex].Start();
            }
        }
        else
        {
            Stopped(true);
        }
    }

    public override void StopLowerPriorityChildrenForChild(NPBtrNode abortForChild, bool immediateRestart)
    {
        int indexForChild = 0;
        bool found = false;
        foreach (NPBtrNode currentChild in Children)
        {
            if (currentChild == abortForChild)
            {
                found = true;
            }
            else if (!found)
            {
                indexForChild++;
            }
            else if (found && currentChild.IsActive)
            {
                if(immediateRestart) 
                {
                    currentIndex = indexForChild - 1;
                } 
                else 
                {
                    currentIndex = Children.Length; 
                }
                currentChild.Stop();
                break;
            }
        }
    }

    override public string ToString()
    {
        return base.ToString() + "[" + this.currentIndex + "]";
    }
}