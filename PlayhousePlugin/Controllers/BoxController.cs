using System;
using System.Collections.Generic;
using MapEditorReborn.API.Features;
using UnityEngine;

namespace PlayhousePlugin.Controllers
{
    public class BoxType : IEquatable<BoxType>
    {
        private BoxType(string value) { Name = value; }
        public string Name { get; set; }
        
        public static BoxType StandardBox   { get { return new BoxType("Box1"); } }
        public static BoxType BoxWithBrackets   { get { return new BoxType("Box2"); } }
        public static BoxType LongBox    { get { return new BoxType("Box3"); } }

        public bool Equals(BoxType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BoxType) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(BoxType left, BoxType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BoxType left, BoxType right)
        {
            return !Equals(left, right);
        }
    }

    public class Box
    {
        public BoxType BoxType;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public Box(BoxType boxType, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            BoxType = boxType;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
    
    public class BoxController
    {
        public static List<Box> PermenantBoxes = new List<Box>
        {
            new Box(BoxType.LongBox, new Vector3(52.11f,1001.97f,-50.375f), Quaternion.identity, Vector3.one),
            new Box(BoxType.StandardBox, new Vector3(53.735f,1001.97f,-49.735f), Quaternion.identity, Vector3.one),
            new Box(BoxType.BoxWithBrackets, new Vector3(54.121f,1002.37f,-67.909f), Quaternion.identity,Vector3.one*1.5f), 
            new Box(BoxType.BoxWithBrackets, new Vector3(-8.962f,987.7f,-66.150f), Quaternion.identity,Vector3.one), // Elevators
            new Box(BoxType.StandardBox, new Vector3(-6.267f,987.7f,-65.535f), Quaternion.identity,Vector3.one),
            new Box(BoxType.StandardBox, new Vector3(-7.598f,987.7f,-63.764f), Quaternion.identity,Vector3.one),
            new Box(BoxType.LongBox, new Vector3(-18.959f,988.35f,-65.583f), Quaternion.Euler(new Vector3(0, 90, 0)),Vector3.one*2),
            new Box(BoxType.StandardBox, new Vector3(-23.802f,988.35f,-65.306f), Quaternion.identity,Vector3.one*2),
            //new Box(BoxType.StandardBox, new Vector3(0f,0f,0f), Quaternion.identity, Vector3.one),
        };

        public static void SpawnBoxes()
        {
            foreach (var box in PermenantBoxes)
            {
                //var schematicData = MapUtils.GetSchematicDataByName(box.BoxType.Name);
                ObjectSpawner.SpawnSchematic(box.BoxType.Name,
                    box.Position, box.Rotation, box.Scale);
            }
        }
    }
}