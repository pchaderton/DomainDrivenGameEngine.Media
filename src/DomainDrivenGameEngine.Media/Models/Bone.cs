using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A bone in a skeleton for a skinned model.
    /// </summary>
    public class Bone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bone"/> class.
        /// </summary>
        /// <param name="offsetMatrix">The matrix to use for offsetting this bone from the parent.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="children">The children of this bone.  The bones in this collection will have their parent set to this bone.</param>
        /// <param name="computeWorldToBindMatrices">Optional, defaults to <c>false</c>.  When <c>true</c>, computes the world-to-bind matrices for this bone and all child bones.</param>
        public Bone(Matrix4x4 offsetMatrix,
                    string name,
                    ReadOnlyCollection<Bone> children = null,
                    bool computeWorldToBindMatrices = false)
        {
            OffsetMatrix = offsetMatrix;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Parent = null;
            Children = children ?? new ReadOnlyCollection<Bone>(new Bone[0]);

            foreach (var child in Children)
            {
                child.Parent = this;
            }

            if (computeWorldToBindMatrices)
            {
                ComputeWorldToBindMatrix(Matrix4x4.Identity);
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

        /// <summary>
        /// Gets the matrix used to transform vertices attached to this bone from world space to bind space.
        /// </summary>
        public Matrix4x4 WorldToBindMatrix { get; private set; }

        /// <summary>
        /// Computes the world-to-bind matrix for this bone based on a parent bone offset matrix.
        /// </summary>
        /// <param name="parentGlobalOffsetMatrix">The <see cref="Matrix4x4"/> which represents the bone parent's total offset matrix.</param>
        private void ComputeWorldToBindMatrix(Matrix4x4 parentGlobalOffsetMatrix)
        {
            var transposedOffsetMatrix = Matrix4x4.Transpose(OffsetMatrix);

            var globalOffsetMatrix = parentGlobalOffsetMatrix * transposedOffsetMatrix;

            Matrix4x4.Invert(globalOffsetMatrix, out var worldToBindMatrix);

            WorldToBindMatrix = worldToBindMatrix;

            foreach (var child in Children)
            {
                child.ComputeWorldToBindMatrix(globalOffsetMatrix);
            }
        }
    }
}
