using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class MathTest {
/*
	[Test]
	public void MathTestSimplePasses() {
		// Use the Assert class to test conditions.
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator MathTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
*/

	[Test] 
	public void MathfPingPong() {
		
		Assert.AreEqual(Mathf.PingPong(0, 3), 0);
		Assert.AreEqual(Mathf.PingPong(1, 3), 1);
		Assert.AreEqual(Mathf.PingPong(2, 3), 2);
		
		Assert.AreEqual(Mathf.PingPong(3, 3), 3);
		
		Assert.AreEqual(Mathf.PingPong(4, 3), 2);
		Assert.AreEqual(Mathf.PingPong(-1, 3), 1);
	}
	
	[Test] 
	public void MathfMoveTowards() {
		
		Assert.AreEqual(Mathf.MoveTowards(0, 3, 1), 1);
		Assert.AreEqual(Mathf.MoveTowards(1, 3, 1), 2);
		Assert.AreEqual(Mathf.MoveTowards(2, 3, 1), 3);
		Assert.AreEqual(Mathf.MoveTowards(3, 3, 1), 3);
		
	}
	
	[Test] 
	public void MathfRound() {

		Assert.AreEqual(Mathf.CeilToInt(0), 0);
		Assert.AreEqual(Mathf.CeilToInt(0.5f), 1);
		Assert.AreEqual(Mathf.CeilToInt(1), 1);
		
	}
	
}
