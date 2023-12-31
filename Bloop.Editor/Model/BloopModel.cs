﻿namespace Bloop.Editor.Model
{
    internal abstract class BloopModel
    {
        public BloopModel(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; set; }
        public string Path { get; }
        public BloopFolder? Parent { get; set; }

        public abstract void Save();
    }
}