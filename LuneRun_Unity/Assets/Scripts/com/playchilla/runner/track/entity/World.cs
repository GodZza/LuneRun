using System.Collections.Generic;
using UnityEngine;
using shared.algorithm.spatial;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.track.entity
{
    public class World
    {
        private List<RunnerEntity> _entities = new List<RunnerEntity>();
        private SpatialHash _entityGrid = new SpatialHash(40, 10000001);
        private List<EntityView> _views = new List<EntityView>();
        private GameObject _gameCont; // Unity container
        private Level _level;

        public World(Level level, GameObject gameContainer)
        {
            _level = level;
            _gameCont = gameContainer;
        }

        public List<RunnerEntity> GetEntities()
        {
            return _entities;
        }

        public void Tick(int deltaTime)
        {
            UpdateEntities(deltaTime);
            UpdateViews(deltaTime);
        }

        public void Render(int deltaTime, float interpolation)
        {
            foreach (var view in _views)
            {
                view.Render(deltaTime, interpolation);
            }
        }

        public void AddEntity(RunnerEntity entity)
        {
            _entities.Add(entity);
            _entityGrid.Add(entity);
            if (entity is SpeedEntity speedEntity)
            {
                var view = new SpeedView(entity, _level);
                // Instantiate GameObject and attach view component
                // For now, just add to list
                _views.Add(view);
            }
        }

        public void RemoveEntity(RunnerEntity entity)
        {
            int index = _entities.IndexOf(entity);
            Debug.Assert(index != -1, "Trying to remove a not added entity.");
            _entities.RemoveAt(index);
            _entityGrid.Remove(entity);
            // Remove associated view
            var view = _views.Find(v => v is SpeedView && v.Entity == entity);
            if (view != null)
            {
                _views.Remove(view);
                if (view.gameObject != null)
                {
                    Object.Destroy(view.gameObject);
                }
            }
        }

        public RunnerEntity GetClosestEntity(Vec3Const position, double radius)
        {
            var candidates = _entityGrid.GetOverlappingXY(position.x - radius, position.z - radius, position.x + radius, position.z + radius);
            double minDistSqr = radius * radius;
            RunnerEntity closest = null;
            foreach (var candidate in candidates)
            {
                var runnerEntity = candidate as RunnerEntity;
                if (runnerEntity == null) continue;
                double distSqr = runnerEntity.GetPos().distanceSqr(position);
                if (distSqr < minDistSqr)
                {
                    minDistSqr = distSqr;
                    closest = runnerEntity;
                }
            }
            return closest;
        }

        protected virtual void UpdateEntities(int deltaTime)
        {
            var toRemove = new List<RunnerEntity>();
            foreach (var entity in _entities)
            {
                if (entity.CanRemove())
                {
                    toRemove.Add(entity);
                    continue;
                }
                entity.Tick(deltaTime);
            }
            foreach (var entity in toRemove)
            {
                RemoveEntity(entity);
            }
        }

        protected virtual void UpdateViews(int deltaTime)
        {
            var toRemove = new List<EntityView>();
            foreach (var view in _views)
            {
                if (view.CanRemove())
                {
                    toRemove.Add(view);
                }
            }
            foreach (var view in toRemove)
            {
                // Remove GameObject
                if (view.gameObject != null)
                {
                    Object.Destroy(view.gameObject);
                }
                _views.Remove(view);
            }
        }
    }
}