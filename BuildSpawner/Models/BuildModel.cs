using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSpawner.Models
{
    public class BuildModel
    {
        public string Id { get; set; }
        public List<StructureModel> Structures { get; set; }
        public List<BarricadeModel> Barricades { get; set; }
        public float[] Size { get; set; }
        public float[] Shift { get; set; }
        public float[] UserPosition { get; set; }
        public float[] UserRotation { get; set; }

        public BuildModel(string id, List<StructureModel> structures, List<BarricadeModel> barricades, float[] size, float[] shift, float[] userPosition, float[] userRotation)
        {
            Id = id;
            Structures = structures;
            Barricades = barricades;
            Size = size;
            Shift = shift;
            UserPosition = userPosition;
            UserRotation = userRotation;
        }

        public BuildModel()
        {
            Id = "";
            Structures = new List<StructureModel>();
            Barricades = new List<BarricadeModel>();
            Size = new float[3];
            Shift = new float[3];
            UserPosition = new float[3];
            UserRotation = new float[4];
        }
    }
}
