using UnityEngine;
using System.Collections;
using NUnit.Framework;

public class BehaviourTesting {

    [Test]
    public void getPlanFromPlanFunction() {
        WarGames.Plan p;
        p = WarGames.Behaviour.Planning.createPlanFromGoal();
        Assert.That( p != null );
    }
}
