using OpenTK.Mathematics;
using GameStudies.Graphics;

namespace GameStudies.Factories
{
    public static class CubeFactory
    {
        // Indices: 6 por face (2 triângulos), total 36.
        public static readonly uint[] Indices =
        {
            // Front
            0, 1, 2,  2, 3, 0,
            // Back
            4, 5, 6,  6, 7, 4,
            // Left
            8, 9,10, 10,11, 8,
            // Right
           12,13,14, 14,15,12,
            // Top
           16,17,18, 18,19,16,
            // Bottom
           20,21,22, 22,23,20
        };

        /// <summary>
        /// Cria 24 vértices (4 por face) com UVs e normais corretas.
        /// size = aresta do cubo (default 1.0f).
        /// </summary>
        public static Vertex[] CreateVertices(float size = 1.0f)
        {
            float h = size * 0.5f; // half-size
            var c = new Vector3(0.08f, 0.93f, 0.93f); // cor base (opcional)

            return new Vertex[]
            {
                // ===== FRONT (+Z) =====
                // v0..v3  (CCW olhando a face)
                new() { Position = new(-h, -h,  h), Normal = new(0, 0, 1), Color = c, vUV = new(0, 0) }, // 0
                new() { Position = new( h, -h,  h), Normal = new(0, 0, 1), Color = c, vUV = new(1, 0) }, // 1
                new() { Position = new( h,  h,  h), Normal = new(0, 0, 1), Color = c, vUV = new(1, 1) }, // 2
                new() { Position = new(-h,  h,  h), Normal = new(0, 0, 1), Color = c, vUV = new(0, 1) }, // 3

                // ===== BACK (-Z) =====
                // atenção à ordem para manter CCW olhando a face (de frente p/ -Z)
                new() { Position = new( h, -h, -h), Normal = new(0, 0, -1), Color = c, vUV = new(0, 0) }, // 4
                new() { Position = new(-h, -h, -h), Normal = new(0, 0, -1), Color = c, vUV = new(1, 0) }, // 5
                new() { Position = new(-h,  h, -h), Normal = new(0, 0, -1), Color = c, vUV = new(1, 1) }, // 6
                new() { Position = new( h,  h, -h), Normal = new(0, 0, -1), Color = c, vUV = new(0, 1) }, // 7

                // ===== LEFT (-X) =====
                new() { Position = new(-h, -h, -h), Normal = new(-1, 0, 0), Color = c, vUV = new(0, 0) }, // 8
                new() { Position = new(-h, -h,  h), Normal = new(-1, 0, 0), Color = c, vUV = new(1, 0) }, // 9
                new() { Position = new(-h,  h,  h), Normal = new(-1, 0, 0), Color = c, vUV = new(1, 1) }, //10
                new() { Position = new(-h,  h, -h), Normal = new(-1, 0, 0), Color = c, vUV = new(0, 1) }, //11

                // ===== RIGHT (+X) =====
                new() { Position = new( h, -h,  h), Normal = new(1, 0, 0), Color = c, vUV = new(0, 0) }, //12
                new() { Position = new( h, -h, -h), Normal = new(1, 0, 0), Color = c, vUV = new(1, 0) }, //13
                new() { Position = new( h,  h, -h), Normal = new(1, 0, 0), Color = c, vUV = new(1, 1) }, //14
                new() { Position = new( h,  h,  h), Normal = new(1, 0, 0), Color = c, vUV = new(0, 1) }, //15

                // ===== TOP (+Y) =====
                new() { Position = new(-h,  h,  h), Normal = new(0, 1, 0), Color = c, vUV = new(0, 0) }, //16
                new() { Position = new( h,  h,  h), Normal = new(0, 1, 0), Color = c, vUV = new(1, 0) }, //17
                new() { Position = new( h,  h, -h), Normal = new(0, 1, 0), Color = c, vUV = new(1, 1) }, //18
                new() { Position = new(-h,  h, -h), Normal = new(0, 1, 0), Color = c, vUV = new(0, 1) }, //19

                // ===== BOTTOM (-Y) =====
                new() { Position = new(-h, -h, -h), Normal = new(0, -1, 0), Color = c, vUV = new(0, 0) }, //20
                new() { Position = new( h, -h, -h), Normal = new(0, -1, 0), Color = c, vUV = new(1, 0) }, //21
                new() { Position = new( h, -h,  h), Normal = new(0, -1, 0), Color = c, vUV = new(1, 1) }, //22
                new() { Position = new(-h, -h,  h), Normal = new(0, -1, 0), Color = c, vUV = new(0, 1) }, //23
            };
        }
    }
}
