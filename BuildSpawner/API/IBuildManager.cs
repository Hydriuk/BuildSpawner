using BuildSpawner.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using UnityEngine;

namespace BuildSpawner.API
{
#if OPENMOD
    [Service]
#endif
    public interface IBuildManager
    {
        /// <summary>
        /// Saves a build and deletes it from the game
        /// </summary>
        /// <param name="buildName"> Name of the build </param>
        /// <param name="userPosition"> Position of the user </param>
        /// <param name="userRotation"> Rotation of the user </param>
        /// <param name="buildSize"> Size of the build </param>
        /// <param name="shift"> How much is the build shifted from the user </param>
        void SaveBuild(string buildName, Vector3 userPosition, Quaternion userRotation, Vector3 buildSize, Vector3 shift);

        /// <summary>
        /// Places a build from a given position
        /// </summary>
        /// <param name="buildName"> Name of the build to place </param>
        /// <param name="userPosition"> Position from which to place the build </param>
        /// <param name="userRotation"> Rotation of the build </param>
        /// <param name="shift"> Shift of the build </param>
        /// <param name="ownerId"> Owner of the build </param>
        /// <param name="groupId"> Group of the build </param>
        /// <param name="replace"> Remove structures in area </param>
        /// <returns> True if the build was found. Otherwise false </returns>
        bool PlaceBuild(string buildName, Vector3 userPosition, Quaternion userRotation, Vector3 shift, ulong ownerId, ulong groupId, bool replace);

        /// <summary>
        /// Place a build on its original position
        /// </summary>
        /// <param name="buildName"> Name of the build to place </param>
        /// <param name="ownerId"> Owner of the build </param>
        /// <param name="groupId"> Group of the build </param>
        /// <param name="replace"> Remove structures in area </param>
        /// <returns> True if the build was found. Otherwise false </returns>
        bool PlaceBuild(string buildName, ulong ownerId, ulong groupId, bool replace);

        /// <summary>
        /// Places a build from a given position
        /// </summary>
        /// <param name="build"> The build to place </param>
        /// <param name="userPosition"> Position from which to place the build </param>
        /// <param name="userRotation"> Rotation of the build </param>
        /// <param name="shift"> Shift of the build </param>
        /// <param name="ownerId"> Owner of the build </param>
        /// <param name="groupId"> Group of the build </param>
        /// <param name="replace"> Remove structures in area </param>
        void PlaceBuild(BuildModel build, Vector3 userPosition, Quaternion userRotation, Vector3 shift, ulong ownerId, ulong groupId, bool replace);

        /// <summary>
        /// List all available builds
        /// </summary>
        /// <returns> The list of builds in a string </returns>
        string ListBuilds();
    }
}