using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace PuzzlemakerPro.Scripts.Editor
{
    class Selection
    {
        private VoxelPos start = VoxelPos.Origin;
        private VoxelPos end = VoxelPos.Origin;
        private Vector3 normal = Vector3.Zero;
        private bool started = false;
        private bool completed = false;

        public void StartSelection(VoxelPos pos, Vector3 normal)
        {
            if (started)
            {
                throw new InvalidOperationException("Selection has already been started!");
            }

            this.start = pos;
            this.end = pos;
            this.normal = normal;
            started = true;
            completed = false;
        }

        public void UpdateSelection(VoxelPos pos)
        {
            if (completed)
            {
                throw new InvalidOperationException("Selection has been completed.");
            }
            if (!started)
            {
                throw new InvalidOperationException("Selection has not been started.");
            }

            end = pos;
        }

        public void FinishSelection()
        {
            if (!started)
            {
                throw new InvalidOperationException("Selection has not been started.");
            }

            completed = true;
        }

        public void ResetSelection()
        {
            start = VoxelPos.Origin;
            end = VoxelPos.Origin;
            normal = Vector3.Zero;
            started = false;
            completed = false;
        }

        public void Translate(Vector3 normal)
        {
            start = start.Translate(normal);
            end = end.Translate(normal);
        }

        public bool Started()
        {
            return started;
        }

        public bool Completed()
        {
            return completed;
        }

        public bool Is3D()
        {
            return !(start.x == end.x || start.y == end.y || start.z == end.z);
        }

        public (VoxelPos, VoxelPos, Vector3) GetSelectionTuple()
        {
            return (start, end, normal);
        }

        public override string ToString()
        {
            return $"{{ Start: {start} End: {end} Normal: {normal} 3D: {Is3D()} }}";
        }
    }
}
