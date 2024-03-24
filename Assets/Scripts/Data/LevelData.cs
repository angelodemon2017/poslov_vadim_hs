using Events;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class LevelData
    {
        private const int MAPWIDTH = 20;

        public PersonModel Person;
        public BasePointModel BasePoint;
        public List<PatrolPointModel> PatrolPoints = new();

        public LevelData(float borderWidth, float borderLength, Config config)
        {
            BasePoint = new BasePointModel(borderWidth, borderLength);
            Person = config.GeneratePerson(BasePoint.position);
            GeneratePatrolPoint(borderWidth, borderLength, config.PatrolPoints, config.MinimalPointDistance);

            EventsController.Fire(new EventModels.Game.LoadLevelData(this));
        }

        private void GeneratePatrolPoint(float borderWidth, float borderLength, int countPoint, float minimalDistance = 1)
        {
            if (minimalDistance <= 0f)
            {
                throw new System.Exception($"minimalDistance below zero");
            }
            PatrolPoints.Clear();

            List<Vector3> availablePoint = new();
            for (int x = 0; x < MAPWIDTH; x++)
                for (int z = 0; z < MAPWIDTH; z++)
                {
                    var newPoint = new Vector3(borderWidth / MAPWIDTH * x - borderWidth / 2, 0,
                        borderLength / MAPWIDTH * z - borderLength / 2);

                    availablePoint.Add(newPoint);
                }

            while (PatrolPoints.Count < countPoint && availablePoint.Count > 0)
            {
                var randomPoint = availablePoint.GetRandom();
                PatrolPoints.Add(new PatrolPointModel(randomPoint));

                foreach (var pp in PatrolPoints)
                {
                    List<Vector3> pointForDel = new();
                    foreach (var ap in availablePoint)
                    {
                        if (Vector3.Distance(ap, pp.Position) < minimalDistance)
                            pointForDel.Add(ap);
                    }
                    foreach (var pfd in pointForDel)
                    {
                        availablePoint.Remove(pfd);
                    }
                }
            }
        }
    }
}