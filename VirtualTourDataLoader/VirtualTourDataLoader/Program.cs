using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualTourApi.Models;
using VirtualTourApi.Repositories;
using VirtualTourDataLoader.Constants;

namespace VirtualTourDataLoader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dataRepository = new DataRepository<SceneData>(new Uri("https://mr-virtual-tour-data.documents.azure.com/"), "");
            await dataRepository.InitializeDatabaseAsync("VirtualTour", "SceneData");
            await dataRepository.CreateItemAsync(new SceneData()
            {
                Name = "Building 9",
                Description = "The Cafe 9 Cafeteria by the fountain.",
                StorageContainerName = "mrvritualtour-scene1",
                StorageVideoName = "Building25-01.mp4",
                UnityPrimatives = new List<UnityPrimitive>()
                {
                    new UnityPrimitive()
                    {
                        Name = "Sphere 1",
                        Description = "Test Description",
                        PrimativeType = PrimativeTypes.Sphere,
                        Transform = new Transform()
                        {
                            Position = new Point(0, 0, 4),
                            Rotation = new Point(0, 0, 0),
                            Scale = new Point(1, 1, 1),
                        }
                    }
                }
            });

            await dataRepository.CreateItemAsync(new SceneData()
            {
                Name = "Soccor Field",
                Description = "The Reactor.",
                StorageContainerName = "mrvritualtour-scene1",
                StorageVideoName = "Building25-01.mp4",
                UnityPrimatives = new List<UnityPrimitive>()
                {
                    new UnityPrimitive()
                    {
                        Name = "Sphere 1",
                        Description = "Test Description",
                        PrimativeType = PrimativeTypes.Sphere,
                        Transform = new Transform()
                        {
                            Position = new Point(0, 0, 4),
                            Rotation = new Point(0, 0, 0),
                            Scale = new Point(1, 1, 1),
                        }
                    }
                }
            });

            await dataRepository.CreateItemAsync(new SceneData()
            {
                Name = "Soccor Field",
                Description = "The Reactor.",
                StorageContainerName = "mrvritualtour-scene1",
                StorageVideoName = "Building25-01.mp4",
                UnityPrimatives = new List<UnityPrimitive>()
                {
                    new UnityPrimitive()
                    {
                        Name = "Sphere 1",
                        Description = "Test Description",
                        PrimativeType = PrimativeTypes.Sphere,
                        Transform = new Transform()
                        {
                            Position = new Point(0, 0, 4),
                            Rotation = new Point(0, 0, 0),
                            Scale = new Point(1, 1, 1),
                        }
                    }
                }
            });
        }
    }
}
