using Assets.ConectingApp.ConnectingApplication.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.ConectingApp.ConnectingApplication.PathFinder
{
    public class Point
    {
        public Vector3 Coordinates;
        public Dictionary<EDirection, Point> Neighbors;
        public float Weight;

        public void ololo()
        {
            
        }
    }
}