using System.Numerics;
using GameStudies.Source;

namespace GameStudies.Factory
{
    public static class CubeFactory
    {
        public static Vertex[] CreateVertices()
        {
            Vertex[] vertices =
            [

                new() { Position= new(-0.5f, -0.5f, -0.5f), Normal= new(0.0f, 0.0f, -1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 0.0f)},
                new() { Position= new( 0.5f, -0.5f, -0.5f), Normal= new(0.0f, 0.0f, -1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new( 0.5f,  0.5f, -0.5f), Normal= new(0.0f, 0.0f, -1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 1.0f)},
                new() { Position= new( 0.5f,  0.5f, -0.5f), Normal= new(0.0f, 0.0f, -1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 1.0f)},
                new() { Position= new(-0.5f,  0.5f, -0.5f), Normal= new(0.0f, 0.0f, -1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)},
                new() { Position= new(-0.5f, -0.5f, -0.5f), Normal= new(0.0f, 0.0f, -1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 0.0f)},
                new() { Position= new(-0.5f, -0.5f,  0.5f), Normal= new(0.0f, 0.0f,  1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 0.0f)},
                new() { Position= new( 0.5f, -0.5f,  0.5f), Normal= new(0.0f, 0.0f,  1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new( 0.5f,  0.5f,  0.5f), Normal= new(0.0f, 0.0f,  1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 1.0f)},
                new() { Position= new( 0.5f,  0.5f,  0.5f), Normal= new(0.0f, 0.0f,  1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 1.0f)},
                new() { Position= new(-0.5f,  0.5f,  0.5f), Normal= new(0.0f, 0.0f,  1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)},
                new() { Position= new(-0.5f, -0.5f,  0.5f), Normal= new(0.0f, 0.0f,  1.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 0.0f)},
                new() { Position= new(-0.5f,  0.5f,  0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new(-0.5f,  0.5f, -0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 1.0f)},
                new() { Position= new(-0.5f, -0.5f, -0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)},
                new() { Position= new(-0.5f, -0.5f, -0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)},
                new() { Position= new(-0.5f, -0.5f,  0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 0.0f)},
                new() { Position= new(-0.5f,  0.5f,  0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new( 0.5f,  0.5f,  0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new( 0.5f,  0.5f, -0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 1.0f)},
                new() { Position= new( 0.5f, -0.5f, -0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)},
                new() { Position= new( 0.5f, -0.5f, -0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)},
                new() { Position= new( 0.5f, -0.5f,  0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 0.0f)},
                new() { Position= new( 0.5f,  0.5f,  0.5f), Normal= new(1.0f, 0.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new(-0.5f, -0.5f, -0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)},
                new() { Position= new( 0.5f, -0.5f, -0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 1.0f)},
                new() { Position= new( 0.5f, -0.5f,  0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new( 0.5f, -0.5f,  0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new(-0.5f, -0.5f,  0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 0.0f)},
                new() { Position= new(-0.5f, -0.5f, -0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)},
                new() { Position= new(-0.5f,  0.5f, -0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)},
                new() { Position= new( 0.5f,  0.5f, -0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 1.0f)},
                new() { Position= new( 0.5f,  0.5f,  0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new( 0.5f,  0.5f,  0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(1.0f, 0.0f)},
                new() { Position= new(-0.5f,  0.5f,  0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 0.0f)},
                new() { Position= new(-0.5f,  0.5f, -0.5f), Normal= new(0.0f, 1.0f,  0.0f),Color=  new( 0.08f, 0.93f, 0.93f), vUV= new(0.0f, 1.0f)}
            ];

            return vertices;
        }
    }
}


