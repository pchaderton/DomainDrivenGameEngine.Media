using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A bone in a skeleton for a skinned model.
    /// </summary>
    public class Bone : IMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bone"/> class.
        /// </summary>
        /// <param name="offsetMatrix">The matrix to use for offsetting this bone from the parent.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="children">The children of this bone.  The bones in this collection will have their parent set to this bone.</param>
        public Bone(Matrix4x4 offsetMatrix,
                    string name,
                    ReadOnlyCollection<Bone> children = null)
        {
            OffsetMatrix = offsetMatrix;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Parent = null;
            Children = children ?? new ReadOnlyCollection<Bone>(new Bone[0]);

            foreach (var child in Children)
            {
                child.Parent = this;
            }
        }

        /// <summary>
        /// Gets the children of this bone.
        /// </summary>
        public IReadOnlyList<Bone> Children { get; }

        /// <summary>
        /// Gets the name of the bone.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the offset matrix for this bone.
        /// </summary>
        public Matrix4x4 OffsetMatrix { get; }

        /// <summary>
        /// Gets the parent of this bone.
        /// </summary>
        public Bone Parent { get; private set; }
    }
}
