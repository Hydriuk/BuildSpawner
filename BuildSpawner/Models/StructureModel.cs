using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSpawner.Models
{
    public class StructureModel
    {
        public ushort Id { get; set; }
        public float[] Position { get; set; }
        public float[] Rotation { get; set; }

        public StructureModel(ushort id, float[] position, float[] rotation)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
        }

        public StructureModel()
        {
            Id = 0;
            Position = new float[3];
            Rotation = new float[4];
        }
    }
}
