using LiteDB;
using System.Collections.Generic;

namespace BuildSpawner.Models
{
    public class BuildModel
    {
        [BsonId]
        public string Name { get; set; }
        public List<StructureModel> Structures { get; set; }
        public List<BarricadeModel> Barricades { get; set; }
        public float[] Size { get; set; }
        public float[] Shift { get; set; }
        public float[] UserPosition { get; set; }
        public float[] UserRotation { get; set; }

        public BuildModel(string name, List<StructureModel> structures, List<BarricadeModel> barricades, float[] size, float[] shift, float[] userPosition, float[] userRotation)
        {
            Name = name;
            Structures = structures;
            Barricades = barricades;
            Size = size;
            Shift = shift;
            UserPosition = userPosition;
            UserRotation = userRotation;
        }

        public BuildModel()
        {
            Name = "";
            Structures = new List<StructureModel>();
            Barricades = new List<BarricadeModel>();
            Size = new float[3];
            Shift = new float[3];
            UserPosition = new float[3];
            UserRotation = new float[4];
        }
    }
}