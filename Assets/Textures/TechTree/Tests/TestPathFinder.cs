using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestPathFinder
    {
        private Vector2 startPosition;
        private Vector2 endPosition;
        private int gridWidth;
        private int gridHeight;
        // A Test behaves as an ordinary method
        [Test]
        public void TestPathFinderSimplePasses()
        {
            // Use the Assert class to test conditions
            startPosition = new Vector2(1, 0);
            endPosition = new Vector2(4, 4);
            gridWidth = 5;
            gridHeight = 5;

            List<Vector2> expectedWayPoints = new List<Vector2>();
            expectedWayPoints.Add(new Vector2(2, 1));
            expectedWayPoints.Add(new Vector2(3, 2));
            expectedWayPoints.Add(new Vector2(4, 3));

            AStarPathfinding pathFinder = new AStarPathfinding(startPosition, endPosition, gridWidth, gridHeight);
            
            List<Vector2> wayPoints = pathFinder.getWaypoints();

            int i = 0;
            foreach(Vector2 wayPoint in wayPoints)
            {
                Assert.True(wayPoint.ToString().Equals(expectedWayPoints[i++].ToString()));
            }

            Debug.Log("Hi Debug Console");
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator TestPathFinderWithEnumeratorPasses()
        //{
        //    // Use the Assert class to test conditions.
        //    // Use yield to skip a frame.
        //    yield return null;
        //}
    }


}
