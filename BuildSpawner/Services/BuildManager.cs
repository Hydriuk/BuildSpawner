using BuildSpawner.API;
using BuildSpawner.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuildSpawner.Services
{
#if OPENMOD
    [PluginServiceImplementation]
#endif
    public class BuildManager : IBuildManager
    {
        private IBuildStore _buildStore;
        private IThreadAdapter _threadAdapter;

        public BuildManager(IBuildStore buildStore, IThreadAdapter threadAdapter)
        {
            _buildStore = buildStore;
            _threadAdapter = threadAdapter;
        }

        public void SaveBuild(string buildName, Vector3 userPosition, Quaternion userRotation, Vector3 buildSize, Vector3 shift)
        {
            BuildModel build = new BuildModel
            (
                buildName,
                new List<StructureModel>(),
                new List<BarricadeModel>(),
                new float[3] { buildSize.x, buildSize.y, buildSize.z },
                new float[3] { shift.x, shift.y, shift.z },
                new float[3] { userPosition.x, userPosition.y, userPosition.z },
                new float[4] { userRotation.x, userRotation.y, userRotation.z, userRotation.w }
            );

            // Gets the center of the build
            Vector3 buildCenter = GetBuildCenter(userPosition, userRotation, buildSize, shift);

            // Gets all regions in the area of the build
            List<RegionCoordinate> regions = new List<RegionCoordinate>();
            Regions.getRegionsInRadius(buildCenter, Math.Max(buildSize.x, buildSize.z) * 2, regions);

            // Gets and removes all structures and barricades
            build.Structures.AddRange(PopStructures(regions, buildCenter, buildSize, userRotation));
            build.Barricades.AddRange(PopBarricades(regions, buildCenter, buildSize, userRotation));

            // Saves the build
            _buildStore.SaveBuild(build);
        }

        public bool PlaceBuild(string buildName, Vector3 userPosition, Quaternion userRotation, Vector3 shift, ulong ownerId = 0, ulong groupId = 0)
        {
            BuildModel build = _buildStore.GetBuild(buildName);

            if (build is null)
            {
                return false;
            }

            _threadAdapter.RunOnMainThread(() => PlaceBuild(build, userPosition, userRotation, shift, ownerId, groupId));

            return true;
        }

        public bool PlaceBuild(string buildName, ulong ownerId = 0, ulong groupId = 0)
        {
            BuildModel build = _buildStore.GetBuild(buildName);

            if (build is null)
            {
                return false;
            }

            // Recovers the saving user's position when he saved the build
            Vector3 buildPosition = new Vector3(build.UserPosition[0], build.UserPosition[1], build.UserPosition[2]);
            Quaternion buildRotation = new Quaternion(build.UserRotation[0], build.UserRotation[1], build.UserRotation[2], build.UserRotation[3]);

            // Places the build using the position of the user who saved it
            _threadAdapter.RunOnMainThread(() => PlaceBuild(build, buildPosition, buildRotation, Vector3.zero, ownerId, groupId));

            return true;
        }

        public string ListBuilds()
        {
            var buildNames = _buildStore.GetBuildNames();

            StringBuilder sb = new StringBuilder();

            if (buildNames.Count() > 0)
            {
                foreach (var name in buildNames)
                {
                    sb.Append($"{name}, ");
                }
                sb.Remove(sb.Length - 2, 2);
            }
            else
            {
                sb.Append("");
            }

            return sb.ToString();
        }

        public void PlaceBuild(BuildModel build, Vector3 userPosition, Quaternion userRotation, Vector3 shift, ulong ownerId = 0, ulong groupId = 0)
        {
            // Adds the given shift and the shift of the build when it was saved
            Vector3 buildShift = new Vector3(build.Shift[0], build.Shift[1], build.Shift[2]);
            Vector3 buildSize = new Vector3(build.Size[0], build.Size[1], build.Size[2]);
            shift += buildShift;

            // Gets the build center
            Vector3 buildCenter = GetBuildCenter(userPosition, userRotation, buildSize, shift);

            // Removes structures and barricades from the build zone
            List<RegionCoordinate> regions = new List<RegionCoordinate>();
            Regions.getRegionsInRadius(buildCenter, Math.Max(buildSize.x, buildSize.z) * 2, regions);
            PopStructures(regions, buildCenter, buildSize, userRotation);
            PopBarricades(regions, buildCenter, buildSize, userRotation);

            // Places all structures of the build
            foreach (var structureModel in build.Structures)
            {
                // Gets position and rotation of the structure from their relative to the player
                Vector3 relativePostion = new Vector3(structureModel.Position[0], structureModel.Position[1], structureModel.Position[2]);
                Quaternion relativeRotation = new Quaternion(structureModel.Rotation[0], structureModel.Rotation[1], structureModel.Rotation[2], structureModel.Rotation[3]);

                Vector3 point = userRotation * relativePostion + buildCenter;
                Quaternion rotation = userRotation * relativeRotation;

                // Creates and places the structure
                ItemStructureAsset structureAsset = (ItemStructureAsset)Assets.find(EAssetType.ITEM, structureModel.Id);
                Structure structure = new Structure(structureAsset, structureAsset.health);
                StructureManager.dropReplicatedStructure(structure, point, rotation, ownerId, groupId);
            }

            // Places all barricades of the build
            List<(Transform transform, Item item, BarricadeModel barricadeModel)> storageDisplaysToUpdate = new List<(Transform transform, Item item, BarricadeModel barricadeModel)>();
            foreach (var barricadeModel in build.Barricades)
            {
                // Gets relative position and rotation of the barricade to the player
                Vector3 relativePostion = new Vector3(barricadeModel.Position[0], barricadeModel.Position[1], barricadeModel.Position[2]);
                Quaternion relativeRotation = new Quaternion(barricadeModel.Rotation[0], barricadeModel.Rotation[1], barricadeModel.Rotation[2], barricadeModel.Rotation[3]);

                Vector3 point = userRotation * relativePostion + buildCenter;
                Quaternion rotation = userRotation * relativeRotation;

                // Creates the barricade
                ItemBarricadeAsset barricadeAsset = (ItemBarricadeAsset)Assets.find(EAssetType.ITEM, barricadeModel.Id);

                Barricade barricade = new Barricade(barricadeAsset)
                {
                    state = barricadeModel.State
                };

                // Sets storage a permissions
                if (barricade.state.Length >= 16)
                {
                    byte[] ownerBytes = ownerId != 0 ? BitConverter.GetBytes(ownerId) : new byte[8];
                    ownerBytes.CopyTo(barricade.state, 0);

                    byte[] groupBytes = groupId != 0 ? BitConverter.GetBytes(groupId) : new byte[8];
                    groupBytes.CopyTo(barricade.state, 8);
                }

                // Places the barricade
                Transform transform = BarricadeManager.dropNonPlantedBarricade(barricade, point, rotation, ownerId, groupId);

                // Prepare storage display update
                if (barricade.asset is ItemStorageAsset itemStorage && itemStorage.isDisplay)
                {
                    Item item = new Item(
                        barricadeModel.DisplayItem,
                        1,
                        0,
                        barricadeModel.DisplayState
                    );

                    storageDisplaysToUpdate.Add((transform, item, barricadeModel));
                }
            }

            // Update displays
            _threadAdapter.RunOnMainThread(() =>
            {
                foreach (var storageDisplay in storageDisplaysToUpdate)
                {
                    BarricadeManager.sendStorageDisplay(
                        storageDisplay.transform,
                        storageDisplay.item,
                        storageDisplay.barricadeModel.DisplaySkin,
                        storageDisplay.barricadeModel.DisplayMythic,
                        storageDisplay.barricadeModel.DisplayTags,
                        storageDisplay.barricadeModel.DisplayProps
                    );
                }
            });
        }

        /// <summary>
        /// Gets the center of a box from the size of an initial position, rotation, the size of a box and a shift
        /// </summary>
        /// <param name="userPosition"> The initial position </param>
        /// <param name="userRotation"> The initial rotation </param>
        /// <param name="buildSize"> The box's size</param>
        /// <param name="shift"> The shift </param>
        /// <returns> The center of the box </returns>
        private Vector3 GetBuildCenter(Vector3 userPosition, Quaternion userRotation, Vector3 buildSize, Vector3 shift)
        {
            // Gets the direction of the box's center from the initial position
            var radianAngle = userRotation.eulerAngles.y * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(radianAngle), 0, Mathf.Cos(radianAngle));

            // Shift to move from the user's position to the box's center, with a slight additionnal shift for ease of use
            Vector3 baseShift = new Vector3(direction.x * 2, buildSize.y / 2 - 2, direction.z * 2) + direction * buildSize.z / 2;

            // Adds the user's position and the shifts
            return userPosition + baseShift + userRotation * shift;
        }

        /// <summary>
        /// Removes and returns all structures in a given area
        /// </summary>
        /// <param name="regions"> The regions to search in </param>
        /// <param name="buildCenter"> The center of the box </param>
        /// <param name="buildSize"> The size of the box </param>
        /// <param name="userRotation"> The rotation of the user </param>
        /// <returns> The removed structures </returns>
        private List<StructureModel> PopStructures(List<RegionCoordinate> regions, Vector3 buildCenter, Vector3 buildSize, Quaternion userRotation)
        {
            List<StructureModel> structures = new List<StructureModel>();

            foreach (var regionCoord in regions)
            {
                StructureManager.tryGetRegion(regionCoord.x, regionCoord.y, out var structRegion);

                // Gets all structures in the region
                List<StructureDrop> regionStructures = structRegion.drops.FindAll(structure => IsInBox(buildCenter, buildSize, userRotation, structure.model.position));

                foreach (var structure in regionStructures)
                {
                    // Gets relative position and rotation of the structure to the user
                    Vector3 position = Quaternion.Inverse(userRotation) * (structure.model.position - buildCenter);
                    Quaternion rotation = Quaternion.Inverse(userRotation) * structure.model.rotation;

                    structures.Add(new StructureModel(
                        structure.asset.id,
                        new float[3] { position.x, position.y, position.z },
                        new float[4] { rotation.x, rotation.y, rotation.z, rotation.w }
                    ));
                }

                // Removes the structures
                _threadAdapter.RunOnMainThread(() =>
                {
                    foreach (var structure in regionStructures)
                        StructureManager.destroyStructure(structure, regionCoord.x, regionCoord.y, Vector3.one * 100);
                });
            }

            return structures;
        }

        /// <summary>
        /// Checks if a point is contained inside a box
        /// </summary>
        /// <param name="buildCenter"> Center of the box </param>
        /// <param name="buildSize"> Size of the box </param>
        /// <param name="userRotation"> Rotation of the user (to apply to the box) </param>
        /// <param name="point"> Point to check </param>
        /// <returns></returns>
        private bool IsInBox(Vector3 buildCenter, Vector3 buildSize, Quaternion userRotation, Vector3 point)
        {
            // TODO Comment Math
            Vector3 relativeDistance = Quaternion.Inverse(userRotation) * (point - buildCenter);
            relativeDistance.Set(
                Mathf.Abs(relativeDistance.x),
                Mathf.Abs((point - buildCenter).y),
                Mathf.Abs(relativeDistance.z)
            );

            Vector3 box = buildSize / 2;

            return relativeDistance.x < box.x && relativeDistance.y < box.y && relativeDistance.z < box.z;
        }

        /// <summary>
        /// Removes and returns all barricades in a given area
        /// </summary>
        /// <param name="regions"> The regions to search in </param>
        /// <param name="buildCenter"> The center of the box </param>
        /// <param name="buildSize"> The size of the box </param>
        /// <param name="userDirection"> The rotation of the user </param>
        /// <returns> The removed barricades </returns>
        private List<BarricadeModel> PopBarricades(List<RegionCoordinate> regions, Vector3 buildCenter, Vector3 buildSize, Quaternion userDirection)
        {
            List<BarricadeModel> barricades = new List<BarricadeModel>();

            foreach (var regionCoord in regions)
            {
                BarricadeManager.tryGetRegion(regionCoord.x, regionCoord.y, ushort.MaxValue, out var barricadeRegion);

                // Gets all barricades in the region
                List<BarricadeDrop> regionBarricades = barricadeRegion.drops.FindAll(barricade => IsInBox(buildCenter, buildSize, userDirection, barricade.model.position));

                foreach (var barricade in regionBarricades)
                {
                    // Gets relative position and rotation of the barricade to the user
                    Vector3 position = Quaternion.Inverse(userDirection) * (barricade.model.position - buildCenter);
                    Quaternion rotation = Quaternion.Inverse(userDirection) * barricade.model.rotation;
                    BarricadeModel barricadeModel = new BarricadeModel(
                        barricade.asset.id,
                        new float[3] { position.x, position.y, position.z },
                        new float[4] { rotation.x, rotation.y, rotation.z, rotation.w },
                        barricade.GetServersideData().barricade.state
                    );

                    // Removes storages content for it not to drop on the floor
                    if (barricade.interactable is InteractableStorage storage)
                    {
                        if (storage.displayItem != null)
                        {
                            barricadeModel.DisplayItem = storage.displayItem.id;
                            barricadeModel.DisplayState = storage.displayItem.state;
                            barricadeModel.DisplaySkin = storage.displaySkin;
                            barricadeModel.DisplayMythic = storage.displayMythic;
                            barricadeModel.DisplayTags = storage.displayTags;
                            barricadeModel.DisplayProps = storage.displayDynamicProps;
                        }

                        storage.items.clear();
                    }

                    if (barricade.interactable is InteractableSentry sentry)
                    {
                        sentry.items.clear();
                    }

                    barricades.Add(barricadeModel);
                }

                // Removes the barricades
                _threadAdapter.RunOnMainThread(() =>
                {
                    foreach (var barricade in regionBarricades)
                        BarricadeManager.destroyBarricade(barricade, regionCoord.x, regionCoord.y, ushort.MaxValue);
                });
            }

            return barricades;
        }
    }
}