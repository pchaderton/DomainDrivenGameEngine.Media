using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services.Extensions
{
    /// <summary>
    /// Extension methods for generating and referencing basic models via a <see cref="IMediaReferenceService{Model}"/>.
    /// </summary>
    /// <remarks>
    /// These extension methods are not the most efficient way for generating this basic models, and instead utilized
    /// less efficient, but easier to read, matrix math for generating the vertices for the models.  Further optimization
    /// is certainly possible.
    /// </remarks>
    public static class BasicModelReferenceServiceExtensions
    {
        /// <summary>
        /// Creates and references a flat, single-sided circle model.
        /// </summary>
        /// <param name="modelReferenceService">The <see cref="IMediaReferenceService{Model}"/> to reference the model with.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="segments">The number of segments to divide the circle into.</param>
        /// <param name="options">The common <see cref="BasicModelOptions"/> to use for generating the model.</param>
        /// <returns>A <see cref="IMediaReference{Model}"/> to the generated model.</returns>
        public static IMediaReference<Model> ReferenceCircle(this IMediaReferenceService<Model> modelReferenceService,
                                                             float radius = 1.0f,
                                                             uint segments = 16,
                                                             BasicModelOptions? options = null)
        {
            if (modelReferenceService == null)
            {
                throw new ArgumentNullException(nameof(modelReferenceService));
            }

            if (radius <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(radius)} greater than zero is required.");
            }

            var rotationAngleStep = 2.0f * MathF.PI / segments;
            var coordinateStep = 1.0f / segments;

            var rotation = 0.0f;
            var horizontalCoordinate = 0.0f;
            var vertexPosition = new Vector3(radius, 0.0f, 0.0f);
            var addIndices = false;
            var normal = new Vector3(0.0f, 1.0f, 0.0f);
            var color = options?.Color ?? default;
            var offset = options?.Offset ?? Vector3.Zero;
            var vertices = new List<Vertex>
            {
                new Vertex(offset,
                           normal,
                           Vector3.Zero,
                           color,
                           new Vector2(0.5f, 0.5f)),
            };

            var indices = new List<uint>();
            for (var i = 0; i <= segments; i++)
            {
                var transformationMatrix = Matrix4x4.CreateRotationY(rotation);
                var transformedPosition = Vector3.Transform(vertexPosition, transformationMatrix);

                vertices.Add(new Vertex(new Vector3(transformedPosition.X + offset.X, offset.Y, transformedPosition.Z + offset.Z),
                                        normal,
                                        Vector3.Zero,
                                        color,
                                        new Vector2(0.5f + (MathF.Cos(rotation) * 0.5f), 0.5f + (MathF.Sin(rotation) * 0.5f))));

                if (addIndices)
                {
                    var initialTopCount = (uint)vertices.Count;
                    indices.AddRange(new uint[]
                    {
                        0,
                        initialTopCount - 2,
                        initialTopCount - 1,
                    });
                }

                rotation += rotationAngleStep;
                horizontalCoordinate += coordinateStep;
                addIndices = true;
            }

            vertices = CalculateTangents(vertices, indices);

            var meshes = new List<Mesh>
            {
                new Mesh(vertices,
                         indices,
                         options?.TexturePaths,
                         options?.TextureReferences,
                         options?.DefaultBlendMode ?? BlendMode.None,
                         options?.DefaultShaderPaths,
                         options?.DefaultShaderReference),
            };

            return modelReferenceService.Reference(new Model(meshes));
        }

        /// <summary>
        /// Creates and references a flat, single-sided fan model.
        /// </summary>
        /// <param name="modelReferenceService">The <see cref="IMediaReferenceService{Model}"/> to reference the model with.</param>
        /// <param name="radius">The radius of the fan.</param>
        /// <param name="angle">The angle of the fan.</param>
        /// <param name="segments">The number of segments to divide the fan into.</param>
        /// <param name="options">The common <see cref="BasicModelOptions"/> to use for generating the model.</param>
        /// <returns>A <see cref="IMediaReference{Model}"/> to the generated model.</returns>
        public static IMediaReference<Model> ReferenceFan(this IMediaReferenceService<Model> modelReferenceService,
                                                          float radius = 1.0f,
                                                          float angle = MathF.PI / 2.0f,
                                                          uint segments = 16,
                                                          BasicModelOptions? options = null)
        {
            if (modelReferenceService == null)
            {
                throw new ArgumentNullException(nameof(modelReferenceService));
            }

            if (radius <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(radius)} greater than zero is required.");
            }

            if (angle <= 0.0f || angle > MathF.PI)
            {
                throw new ArgumentException($"An {nameof(angle)} must be greater than zero and less than or equal to PI.");
            }

            var rotationAngleStep = angle / segments;
            var coordinateStep = 1.0f / segments;

            var rotation = 0.0f;
            var horizontalCoordinate = 0.0f;
            var vertexPosition = new Vector3(radius, 0.0f, 0.0f);
            var normal = new Vector3(0.0f, 1.0f, 0.0f);
            var color = options?.Color ?? default;
            var offset = options?.Offset ?? Vector3.Zero;
            var vertices = new List<Vertex>
            {
                new Vertex(offset,
                           normal,
                           Vector3.Zero,
                           color,
                           new Vector2(0.5f, 0.5f)),
            };
            for (var i = 0; i <= segments; i++)
            {
                var transformationMatrix = Matrix4x4.CreateRotationY(rotation);
                var transformedPosition = Vector3.Transform(vertexPosition, transformationMatrix);

                vertices.Add(new Vertex(new Vector3(transformedPosition.X + offset.X, offset.Y, transformedPosition.Z + offset.Z),
                                        normal,
                                        Vector3.Zero,
                                        color,
                                        new Vector2(0.5f + (MathF.Cos(rotation) * 0.5f), 0.5f + (MathF.Sin(rotation) * 0.5f))));

                rotation += rotationAngleStep;
                horizontalCoordinate += coordinateStep;
            }

            var averagePositionSumVector = Vector3.Zero;
            foreach (var vertex in vertices)
            {
                averagePositionSumVector += vertex.Position;
            }

            vertices.Add(new Vertex(new Vector3(averagePositionSumVector.X / vertices.Count, 0.0f, averagePositionSumVector.Z / vertices.Count),
                                    normal,
                                    Vector3.Zero,
                                    color,
                                    new Vector2(0.5f + (MathF.Cos(rotation) * 0.5f), 0.5f + (MathF.Sin(rotation) * 0.5f))));

            var indices = new List<uint>();
            for (var i = 2; i < vertices.Count - 1; i++)
            {
                indices.AddRange(new uint[] { (uint)i - 1, (uint)i, (uint)vertices.Count - 1 });
            }

            indices.AddRange(new uint[] { 1, (uint)vertices.Count - 1, 0 });
            indices.AddRange(new uint[] { (uint)vertices.Count - 2, 0, (uint)vertices.Count - 1 });

            vertices = CalculateTangents(vertices, indices);

            var meshes = new List<Mesh>
            {
                new Mesh(vertices,
                         indices,
                         options?.TexturePaths,
                         options?.TextureReferences,
                         options?.DefaultBlendMode ?? BlendMode.None,
                         options?.DefaultShaderPaths,
                         options?.DefaultShaderReference),
            };

            return modelReferenceService.Reference(new Model(meshes));
        }

        /// <summary>
        /// Creates and references a single-sided plane model.
        /// </summary>
        /// <param name="modelReferenceService">The <see cref="IMediaReferenceService{Model}"/> to reference the model with.</param>
        /// <param name="width">The width of the plane.</param>
        /// <param name="height">The height of the plane.</param>
        /// <param name="options">The common <see cref="BasicModelOptions"/> to use for generating the model.</param>
        /// <returns>A <see cref="IMediaReference{Model}"/> to the generated model.</returns>
        public static IMediaReference<Model> ReferenceSingleSidedPlane(this IMediaReferenceService<Model> modelReferenceService,
                                                                       float width = 1.0f,
                                                                       float height = 1.0f,
                                                                       BasicModelOptions? options = null)
        {
            if (modelReferenceService == null)
            {
                throw new ArgumentNullException(nameof(modelReferenceService));
            }

            if (width <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(width)} greater than zero is required.");
            }

            if (height <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(height)} greater than zero is required.");
            }

            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            var offset = options?.Offset ?? Vector3.Zero;
            var color = options?.Color ?? default;

            var planeVertices = BuildPlaneVertices(width, height, offset, color);
            ApplyPlaneVertices(planeVertices, Matrix4x4.Identity, vertices, indices);

            vertices = CalculateTangents(vertices, indices);

            var meshes = new List<Mesh>
            {
                new Mesh(vertices,
                         indices,
                         options?.TexturePaths,
                         options?.TextureReferences,
                         options?.DefaultBlendMode ?? BlendMode.None,
                         options?.DefaultShaderPaths,
                         options?.DefaultShaderReference),
            };

            return modelReferenceService.Reference(new Model(meshes));
        }

        /// <summary>
        /// Creates and references a double-sided plane model.
        /// </summary>
        /// <param name="modelReferenceService">The <see cref="IMediaReferenceService{Model}"/> to reference the model with.</param>
        /// <param name="width">The width of the plane.</param>
        /// <param name="height">The height of the plane.</param>
        /// <param name="options">The common <see cref="BasicModelOptions"/> to use for generating the model.</param>
        /// <returns>A <see cref="IMediaReference{Model}"/> to the generated model.</returns>
        public static IMediaReference<Model> ReferenceDoubleSidedPlane(this IMediaReferenceService<Model> modelReferenceService,
                                                                       float width = 1.0f,
                                                                       float height = 1.0f,
                                                                       BasicModelOptions? options = null)
        {
            if (modelReferenceService == null)
            {
                throw new ArgumentNullException(nameof(modelReferenceService));
            }

            if (width <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(width)} greater than zero is required.");
            }

            if (height <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(height)} greater than zero is required.");
            }

            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            var offset = options?.Offset ?? Vector3.Zero;
            var color = options?.Color ?? default;

            var planeVertices = BuildPlaneVertices(width, height, offset, color);
            ApplyPlaneVertices(planeVertices, Matrix4x4.Identity, vertices, indices);
            ApplyPlaneVertices(planeVertices, Matrix4x4.CreateRotationY(MathF.PI), vertices, indices);

            vertices = CalculateTangents(vertices, indices);

            var meshes = new List<Mesh>
            {
                new Mesh(vertices,
                         indices,
                         options?.TexturePaths,
                         options?.TextureReferences,
                         options?.DefaultBlendMode ?? BlendMode.None,
                         options?.DefaultShaderPaths,
                         options?.DefaultShaderReference),
            };

            return modelReferenceService.Reference(new Model(meshes));
        }

        /// <summary>
        /// Creates and references a cube model.
        /// </summary>
        /// <param name="modelReferenceService">The <see cref="IMediaReferenceService{Model}"/> to reference the model with.</param>
        /// <param name="width">The width of the cube.</param>
        /// <param name="height">The height of the cube.</param>
        /// <param name="depth">The depth of the cube.</param>
        /// <param name="options">The common <see cref="BasicModelOptions"/> to use for generating the model.</param>
        /// <returns>A <see cref="IMediaReference{Model}"/> to the generated model.</returns>
        public static IMediaReference<Model> ReferenceCube(this IMediaReferenceService<Model> modelReferenceService,
                                                           float width = 1.0f,
                                                           float height = 1.0f,
                                                           float depth = 1.0f,
                                                           BasicModelOptions? options = null)
        {
            if (modelReferenceService == null)
            {
                throw new ArgumentNullException(nameof(modelReferenceService));
            }

            if (width <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(width)} greater than zero is required.");
            }

            if (height <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(height)} greater than zero is required.");
            }

            if (depth <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(depth)} greater than zero is required.");
            }

            var halfWidth = width / 2.0f;
            var halfHeight = height / 2.0f;
            var halfDepth = depth / 2.0f;

            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            var offset = options?.Offset ?? Vector3.Zero;
            var color = options?.Color ?? default;

            var frontBackPlaneVertices = BuildPlaneVertices(width, height, offset, color);
            var frontBackPlaneTranslation = Matrix4x4.CreateTranslation(0, 0, halfDepth);
            ApplyPlaneVertices(frontBackPlaneVertices, frontBackPlaneTranslation, vertices, indices);
            ApplyPlaneVertices(frontBackPlaneVertices, frontBackPlaneTranslation * Matrix4x4.CreateRotationY(MathF.PI), vertices, indices);

            var leftRightPlaneVertices = BuildPlaneVertices(depth, height, offset, color);
            var leftRightPlaneTranslation = Matrix4x4.CreateTranslation(0, 0, halfWidth);
            ApplyPlaneVertices(leftRightPlaneVertices, leftRightPlaneTranslation * Matrix4x4.CreateRotationY(MathF.PI / 2.0f), vertices, indices);
            ApplyPlaneVertices(leftRightPlaneVertices, leftRightPlaneTranslation * Matrix4x4.CreateRotationY(3.0f * MathF.PI / 2.0f), vertices, indices);

            var topBottomPlaneVertices = BuildPlaneVertices(width, depth, offset, color);
            var topBottomPlaneTranslation = Matrix4x4.CreateTranslation(0, 0, halfHeight);
            ApplyPlaneVertices(topBottomPlaneVertices, topBottomPlaneTranslation * Matrix4x4.CreateRotationX(MathF.PI / 2.0f), vertices, indices);
            ApplyPlaneVertices(topBottomPlaneVertices, topBottomPlaneTranslation * Matrix4x4.CreateRotationX(3.0f * MathF.PI / 2.0f), vertices, indices);

            vertices = CalculateTangents(vertices, indices);

            var meshes = new List<Mesh>
            {
                new Mesh(vertices,
                         indices,
                         options?.TexturePaths,
                         options?.TextureReferences,
                         options?.DefaultBlendMode ?? BlendMode.None,
                         options?.DefaultShaderPaths,
                         options?.DefaultShaderReference),
            };

            return modelReferenceService.Reference(new Model(meshes));
        }

        /// <summary>
        /// Creates and references a cube model.
        /// </summary>
        /// <param name="modelReferenceService">The <see cref="IMediaReferenceService{Model}"/> to reference the model with.</param>
        /// <param name="size">The dimensions of the cube.</param>
        /// <param name="options">The common <see cref="BasicModelOptions"/> to use for generating the model.</param>
        /// <returns>A <see cref="IMediaReference{Model}"/> to the generated model.</returns>
        public static IMediaReference<Model> ReferenceCube(this IMediaReferenceService<Model> modelReferenceService,
                                                           float size = 1.0f,
                                                           BasicModelOptions? options = null)
        {
            if (modelReferenceService == null)
            {
                throw new ArgumentNullException(nameof(modelReferenceService));
            }

            if (size <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(size)} greater than zero is required.");
            }

            return modelReferenceService.ReferenceCube(size, size, size, options);
        }

        /// <summary>
        /// Creates and references a sphere model.
        /// </summary>
        /// <param name="modelReferenceService">The <see cref="IMediaReferenceService{Model}"/> to reference the model with.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="segments">The number of segments to divide the sphere into.</param>
        /// <param name="options">The common <see cref="BasicModelOptions"/> to use for generating the model.</param>
        /// <returns>A <see cref="IMediaReference{Model}"/> to the generated model.</returns>
        public static IMediaReference<Model> ReferenceSphere(this IMediaReferenceService<Model> modelReferenceService,
                                                             float radius = 1.0f,
                                                             uint segments = 8,
                                                             BasicModelOptions? options = null)
        {
            if (modelReferenceService == null)
            {
                throw new ArgumentNullException(nameof(modelReferenceService));
            }

            if (radius <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(radius)} greater than zero is required.");
            }

            var horizontalSegments = segments * 2;
            var horizontalRotationAngleStep = 2.0f * MathF.PI / horizontalSegments;
            var horizontalCoordinateStep = 1.0f / horizontalSegments;
            var verticalRotationAngleStep = MathF.PI / segments;
            var verticalCoordinateStep = 1.0f / segments;
            var startingVector = new Vector3(0.0f, radius, 0.0f);

            var offset = options?.Offset ?? Vector3.Zero;
            var color = options?.Color ?? default;

            var vertices = new List<Vertex>();
            var indices = new List<uint>();
            var verticalRotation = 0.0f;
            var verticalUvCoordinate = 0.0f;
            for (uint i = 0; i <= segments; i++)
            {
                var verticalRotationMatrix = Matrix4x4.CreateRotationX(verticalRotation);

                var horizontalRotation = 0.0f;
                var horizontalUvCoordinate = 0.0f;

                var newVertices = new List<Vertex>();
                var newIndices = new List<uint>();

                for (uint j = 0; j <= horizontalSegments; j++)
                {
                    var horizontalRotationMatrix = Matrix4x4.CreateRotationY(horizontalRotation);
                    var vertexPosition = Vector3.Transform(startingVector, verticalRotationMatrix * horizontalRotationMatrix);

                    newVertices.Add(new Vertex(vertexPosition + offset,
                                               Vector3.Normalize(vertexPosition),
                                               Vector3.Zero,
                                               color,
                                               new Vector2(horizontalUvCoordinate, verticalUvCoordinate)));

                    if (vertices.Count > 0 && newVertices.Count > 0)
                    {
                        var initialCount = (uint)vertices.Count;
                        newIndices.AddRange(new uint[]
                        {
                            initialCount - horizontalSegments + j - 1,
                            initialCount + j - 1,
                            initialCount + j,
                            initialCount - horizontalSegments + j - 1,
                            initialCount + j,
                            initialCount - horizontalSegments + j,
                        });
                    }

                    horizontalRotation += horizontalRotationAngleStep;
                    horizontalUvCoordinate += horizontalCoordinateStep;
                }

                vertices.AddRange(newVertices);
                indices.AddRange(newIndices);

                verticalRotation += verticalRotationAngleStep;
                verticalUvCoordinate += verticalCoordinateStep;
            }

            vertices = CalculateTangents(vertices, indices);

            var meshes = new List<Mesh>
            {
                new Mesh(vertices,
                         indices,
                         options?.TexturePaths,
                         options?.TextureReferences,
                         options?.DefaultBlendMode ?? BlendMode.None,
                         options?.DefaultShaderPaths,
                         options?.DefaultShaderReference),
            };

            return modelReferenceService.Reference(new Model(meshes));
        }

        /// <summary>
        /// Creates and references a cylinder model.
        /// </summary>
        /// <param name="modelReferenceService">The <see cref="IMediaReferenceService{Model}"/> to reference the model with.</param>
        /// <param name="radius">The radius of the cylinder.</param>
        /// <param name="height">The height of the cylinder.</param>
        /// <param name="segments">The number of segments to divide the cylinder into.</param>
        /// <param name="options">The common <see cref="BasicModelOptions"/> to use for generating the model.</param>
        /// <returns>A <see cref="IMediaReference{Model}"/> to the generated model.</returns>
        public static IMediaReference<Model> ReferenceCylinder(this IMediaReferenceService<Model> modelReferenceService,
                                                               float radius = 1.0f,
                                                               float height = 1.0f,
                                                               uint segments = 8,
                                                               BasicModelOptions? options = null)
        {
            if (modelReferenceService == null)
            {
                throw new ArgumentNullException(nameof(modelReferenceService));
            }

            if (radius <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(radius)} greater than zero is required.");
            }

            if (height <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(height)} greater than zero is required.");
            }

            var halfHeight = height / 2.0f;
            var rotationAngleStep = 2.0f * MathF.PI / segments;
            var coordinateStep = 1.0f / segments;

            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            var offset = options?.Offset ?? Vector3.Zero;
            var color = options?.Color ?? default;
            var upNormal = new Vector3(0.0f, 1.0f, 0.0f);

            var rotation = 0.0f;
            var horizontalCoordinate = 0.0f;
            var vertexPosition = new Vector3(radius, 0.0f, 0.0f);
            var addIndices = false;
            var topVertices = new List<Vertex>
            {
                new Vertex(new Vector3(offset.X, halfHeight + offset.Y, offset.Z),
                           upNormal,
                           Vector3.Zero,
                           color,
                           new Vector2(0.5f, 0.5f)),
            };
            var topIndices = new List<uint>();
            for (var i = 0; i <= segments; i++)
            {
                var transformationMatrix = Matrix4x4.CreateRotationY(rotation);
                var transformedPosition = Vector3.Transform(vertexPosition, transformationMatrix);
                var transformedNormal = Vector3.Normalize(transformedPosition);

                vertices.Add(new Vertex(new Vector3(transformedPosition.X + offset.X, halfHeight + offset.Y, transformedPosition.Z + offset.Z),
                                        transformedNormal,
                                        Vector3.Zero,
                                        color,
                                        new Vector2(horizontalCoordinate, 0.0f)));

                vertices.Add(new Vertex(new Vector3(transformedPosition.X + offset.X, offset.Y - halfHeight, transformedPosition.Z + offset.Z),
                                        transformedNormal,
                                        Vector3.Zero,
                                        color,
                                        new Vector2(horizontalCoordinate, 1.0f)));

                topVertices.Add(new Vertex(new Vector3(transformedPosition.X + offset.X, halfHeight + offset.Y, transformedPosition.Z + offset.Z),
                                           upNormal,
                                           Vector3.Zero,
                                           color,
                                           new Vector2(0.5f + (MathF.Cos(rotation) * 0.5f), 0.5f + (MathF.Sin(rotation) * 0.5f))));

                if (addIndices)
                {
                    var initialCount = (uint)vertices.Count - 1;
                    indices.AddRange(new uint[]
                    {
                        initialCount - 3,
                        initialCount,
                        initialCount - 1,
                        initialCount - 3,
                        initialCount - 2,
                        initialCount,
                    });

                    var initialTopCount = (uint)topVertices.Count;
                    topIndices.AddRange(new uint[]
                    {
                        0,
                        initialTopCount - 2,
                        initialTopCount - 1,
                    });
                }

                rotation += rotationAngleStep;
                horizontalCoordinate += coordinateStep;
                addIndices = true;
            }

            var finalTopIndices = topIndices.Select(i => (uint)vertices.Count + i).ToList();
            vertices.AddRange(topVertices);
            indices.AddRange(finalTopIndices);

            var bottomRotationMatrix = Matrix4x4.CreateTranslation(-offset) * Matrix4x4.CreateRotationX(MathF.PI) * Matrix4x4.CreateTranslation(offset);
            var bottomVertices = TransformVertices(topVertices, bottomRotationMatrix);

            var finalBottomIndices = topIndices.Select(i => (uint)vertices.Count + i).ToList();
            vertices.AddRange(bottomVertices);
            indices.AddRange(finalBottomIndices);

            vertices = CalculateTangents(vertices, indices);

            var meshes = new List<Mesh>
            {
                new Mesh(vertices,
                         indices,
                         options?.TexturePaths,
                         options?.TextureReferences,
                         options?.DefaultBlendMode ?? BlendMode.None,
                         options?.DefaultShaderPaths,
                         options?.DefaultShaderReference),
            };

            return modelReferenceService.Reference(new Model(meshes));
        }

        /// <summary>
        /// Creates and references a cone model.
        /// </summary>
        /// <param name="modelReferenceService">The <see cref="IMediaReferenceService{Model}"/> to reference the model with.</param>
        /// <param name="radius">The radius of the cone.</param>
        /// <param name="height">The height of the cone.</param>
        /// <param name="segments">The number of segments to divide the cone into.</param>
        /// <param name="options">The common <see cref="BasicModelOptions"/> to use for generating the model.</param>
        /// <returns>A <see cref="IMediaReference{Model}"/> to the generated model.</returns>
        public static IMediaReference<Model> ReferenceCone(this IMediaReferenceService<Model> modelReferenceService,
                                                           float radius = 1.0f,
                                                           float height = 1.0f,
                                                           uint segments = 8,
                                                           BasicModelOptions? options = null)
        {
            if (modelReferenceService == null)
            {
                throw new ArgumentNullException(nameof(modelReferenceService));
            }

            if (radius <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(radius)} greater than zero is required.");
            }

            if (height <= 0.0f)
            {
                throw new ArgumentException($"A {nameof(height)} greater than zero is required.");
            }

            var halfHeight = height / 2.0f;
            var rotationAngleStep = 2.0f * MathF.PI / segments;
            var coordinateStep = 1.0f / segments;

            var offset = options?.Offset ?? Vector3.Zero;
            var color = options?.Color ?? default;
            var downNormal = new Vector3(0.0f, -1.0f, 0.0f);

            var vertices = new List<Vertex>
            {
                new Vertex(new Vector3(0.0f, halfHeight, 0.0f),
                           new Vector3(0.0f, 1.0f, 0.0f),
                           Vector3.Zero,
                           color,
                           new Vector2(0.5f, 0.5f)),
            };
            var indices = new List<uint>();

            var rotation = 0.0f;
            var horizontalCoordinate = 0.0f;
            var vertexPosition = new Vector3(radius, 0.0f, 0.0f);
            var addIndices = false;
            var bottomVertices = new List<Vertex>
            {
                new Vertex(new Vector3(offset.X, offset.Y - halfHeight, offset.Z),
                           downNormal,
                           Vector3.Zero,
                           color,
                           new Vector2(0.5f, 0.5f)),
            };
            var bottomIndices = new List<uint>();
            for (var i = 0; i <= segments; i++)
            {
                var transformationMatrix = Matrix4x4.CreateRotationY(rotation);
                var transformedPosition = Vector3.Transform(vertexPosition, transformationMatrix);

                vertices.Add(new Vertex(new Vector3(transformedPosition.X + offset.X, offset.Y - halfHeight, transformedPosition.Z + offset.Z),
                                        Vector3.Normalize(new Vector3(transformedPosition.X, 0, transformedPosition.Z)),
                                        Vector3.Zero,
                                        color,
                                        new Vector2(horizontalCoordinate, 0.0f)));

                bottomVertices.Add(new Vertex(new Vector3(transformedPosition.X + offset.X, offset.Y - halfHeight, transformedPosition.Z + offset.Z),
                                              downNormal,
                                              Vector3.Zero,
                                              color,
                                              new Vector2(0.5f + (MathF.Cos(rotation) * 0.5f), 0.5f + (MathF.Sin(rotation) * 0.5f))));

                if (addIndices)
                {
                    var initialCount = (uint)vertices.Count - 1;
                    indices.AddRange(new uint[]
                    {
                        initialCount,
                        0,
                        initialCount - 1,
                    });

                    var initialTopCount = (uint)bottomVertices.Count;
                    bottomIndices.AddRange(new uint[]
                    {
                        0,
                        initialTopCount - 1,
                        initialTopCount - 2,
                    });
                }

                rotation += rotationAngleStep;
                horizontalCoordinate += coordinateStep;
                addIndices = true;
            }

            var finalBottomIndices = bottomIndices.Select(i => (uint)vertices.Count + i).ToList();
            vertices.AddRange(bottomVertices);
            indices.AddRange(finalBottomIndices);

            vertices = CalculateTangents(vertices, indices);

            var meshes = new List<Mesh>
            {
                new Mesh(vertices,
                         indices,
                         options?.TexturePaths,
                         options?.TextureReferences,
                         options?.DefaultBlendMode ?? BlendMode.None,
                         options?.DefaultShaderPaths,
                         options?.DefaultShaderReference),
            };

            return modelReferenceService.Reference(new Model(meshes));
        }

        /// <summary>
        /// Builds the vertices for a plane mesh.
        /// </summary>
        /// <param name="width">The width of the plane.</param>
        /// <param name="height">The height of the plane.</param>
        /// <param name="offset">The offset to apply to the vertices.</param>
        /// <param name="color">The color to apply to the vertices.</param>
        /// <returns>A <see cref="List{Vertex}"/> of vertices for a plane mesh.</returns>
        private static List<Vertex> BuildPlaneVertices(float width, float height, Vector3 offset, VertexColor color)
        {
            var halfWidth = width / 2.0f;
            var halfHeight = height / 2.0f;

            var normal = new Vector3(0.0f, 0.0f, 1.0f);

            return new List<Vertex>
            {
                new Vertex(new Vector3(-halfWidth + offset.X, halfHeight + offset.Y, offset.Z), normal, Vector3.Zero, color, new Vector2(0.0f, 0.0f)),
                new Vertex(new Vector3(halfWidth + offset.X, halfHeight + offset.Y, offset.Z), normal, Vector3.Zero, color, new Vector2(1.0f, 0.0f)),
                new Vertex(new Vector3(halfWidth + offset.X, -halfHeight + offset.Y, offset.Z), normal, Vector3.Zero, color, new Vector2(1.0f, 1.0f)),
                new Vertex(new Vector3(-halfWidth + offset.X, -halfHeight + offset.Y, offset.Z), normal, Vector3.Zero, color, new Vector2(0.0f, 1.0f)),
            };
        }

        /// <summary>
        /// Transforms and generates a new set of vertices based on the provided transformation matrix.
        /// </summary>
        /// <param name="vertices">The vertices to transform.</param>
        /// <param name="transformationMatrix">The matrix to transform the vertices by.</param>
        /// <returns>The transformed vertices.</returns>
        private static List<Vertex> TransformVertices(IReadOnlyCollection<Vertex> vertices, Matrix4x4 transformationMatrix)
        {
            var normalTransformationMatrix = Matrix4x4.Invert(Matrix4x4.Transpose(transformationMatrix), out var normalMatrix)
                ? normalMatrix
                : Matrix4x4.Identity;

            return vertices.Select(v => new Vertex(Vector3.Transform(v.Position, transformationMatrix), Vector3.Transform(v.Normal, normalTransformationMatrix), v.Tangent, v.Color, v.TextureCoordinate)).ToList();
        }

        /// <summary>
        /// Transforms and applies a set of plane vertices to a destination set of vertices and indices, to be used when using multiple planes to generate a single model.
        /// </summary>
        /// <param name="sourcePlaneVertices">The source plane vertices to use.</param>
        /// <param name="transformationMatrix">The transformation matrix to apply.</param>
        /// <param name="destinationVertices">The list to add the newly generated vertices to.</param>
        /// <param name="destinationIndices">The list ot add the newly generated indices to.</param>
        private static void ApplyPlaneVertices(List<Vertex> sourcePlaneVertices, Matrix4x4 transformationMatrix, List<Vertex> destinationVertices, List<uint> destinationIndices)
        {
            var originalVertexLength = destinationVertices.Count;
            var transformedVertices = TransformVertices(sourcePlaneVertices, transformationMatrix);
            destinationVertices.AddRange(transformedVertices);
            destinationIndices.AddRange(new List<uint> { 0, 2, 1, 0, 3, 2 }.Select(i => (uint)originalVertexLength + i));
        }

        /// <summary>
        /// Generates a new list of vertices with calculated tangents based on the provided vertices and indices.
        /// </summary>
        /// <remarks>
        /// Based on algorithm provided at https://marti.works/posts/post-calculating-tangents-for-your-mesh/post/, thanks Llorenç Marti Garcia.
        /// </remarks>
        /// <param name="vertices">The vertices to generate tangents for.</param>
        /// <param name="indices">The indices for the triangles in mesh.</param>
        /// <returns>The new vertices with calculated tangents.</returns>
        private static List<Vertex> CalculateTangents(List<Vertex> vertices, List<uint> indices)
        {
            var tangents = vertices.Select(v => Vector3.Zero).ToList();
            var bitangents = vertices.Select(v => Vector3.Zero).ToList();
            for (var triangleIndex = 0; triangleIndex < indices.Count; triangleIndex += 3)
            {
                var index0 = (int)indices[triangleIndex];
                var index1 = (int)indices[triangleIndex + 1];
                var index2 = (int)indices[triangleIndex + 2];

                var position0 = vertices[index0].Position;
                var position1 = vertices[index1].Position;
                var position2 = vertices[index2].Position;

                var tex0 = vertices[index0].TextureCoordinate;
                var tex1 = vertices[index1].TextureCoordinate;
                var tex2 = vertices[index2].TextureCoordinate;

                var edge1 = position1 - position0;
                var edge2 = position2 - position0;

                var uv1 = tex1 - tex0;
                var uv2 = tex2 - tex0;

                var r = 1.0f / ((uv1.X * uv2.Y) - (uv1.Y * uv2.X));

                var tangent = new Vector3(((edge1.X * uv2.Y) - (edge2.X * uv1.Y)) * r, ((edge1.Y * uv2.Y) - (edge2.Y * uv1.Y)) * r, ((edge1.Z * uv2.Y) - (edge2.Z * uv1.Y)) * r);
                var bitangent = new Vector3(((edge1.X * uv2.X) - (edge2.X * uv1.X)) * r, ((edge1.Y * uv2.X) - (edge2.Y * uv1.X)) * r, ((edge1.Z * uv2.X) - (edge2.Z * uv1.X)) * r);

                tangents[index0] += tangent;
                tangents[index1] += tangent;
                tangents[index2] += tangent;

                bitangents[index0] += bitangent;
                bitangents[index1] += bitangent;
                bitangents[index2] += bitangent;
            }

            var newVertices = new List<Vertex>();
            for (var vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
            {
                var vertex = vertices[vertexIndex];

                var normal = vertex.Normal;
                var summedTangents = tangents[vertexIndex];
                var summedBitangents = bitangents[vertexIndex];

                var t = Vector3.Normalize(summedTangents - (normal * Vector3.Dot(normal, summedTangents)));

                var c = Vector3.Cross(normal, summedTangents);
                var w = Vector3.Dot(c, summedBitangents) < 0.0f ? -1.0f : 1.0f;

                newVertices.Add(new Vertex(vertex.Position, vertex.Normal, t * w, vertex.Color, vertex.TextureCoordinate));
            }

            return newVertices;
        }
    }
}
