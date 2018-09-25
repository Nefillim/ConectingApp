﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ConectingApp.ConnectingApplication.PathFinder
{
    public class AStarAI
    {

		public Queue<Point> FindPath(Point start,Point finish)
		{
			Queue<Point> path = new Queue<Point>();
			Point temp = new Point();
			temp = start;
			while (temp.Coordinates != finish.Coordinates)
			{
				temp = temp.Neighbors.Values.OrderBy(p => p.Weight).First();
				path.Enqueue(temp);
			}
			return path;
		}

		 
    }
}




