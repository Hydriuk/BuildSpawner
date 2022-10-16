using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSpawner.Models
{
    public class BarricadeModel
    {
        public ushort Id { get; set; }
        public float[] Position { get; set; }
        public float[] Rotation { get; set; }
        public byte[] State { get; set; }
        public ushort DisplayItem { get; set; }
        public byte[] DisplayState { get; set; }
        public ushort DisplaySkin { get; set; }
        public ushort DisplayMythic { get; set; }
        public string DisplayTags { get; set; }
        public string DisplayProps { get; set; }

        public BarricadeModel(ushort id, float[] position, float[] rotation, byte[] state)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            State = state;
            DisplayItem = 0;
            DisplayState = new byte[0];
            DisplaySkin = 0;
            DisplayMythic = 0;
            DisplayTags = string.Empty;
            DisplayProps = string.Empty;
        }

        public BarricadeModel()
        {
            Id = 0;
            Position = new float[3];
            Rotation = new float[4];
            State = new byte[8];
            DisplayItem = 0;
            DisplayState = new byte[0];
            DisplaySkin = 0;
            DisplayMythic = 0;
            DisplayTags = string.Empty;
            DisplayProps = string.Empty;
        }
    }
}
