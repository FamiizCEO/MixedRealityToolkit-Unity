﻿#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class FocusProviderTests
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test01_GazeCursorArticulated()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);
            TestUtilities.InitializePlayspace();

            RenderSettings.skybox = null;

            yield return null;
            // Verify that the gaze cursor is visible at the start
            Assert.IsTrue(MixedRealityToolkit.InputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should be visible at start");

            // raise hand up -- gaze cursor should no longer be visible
            // disable user input
            var inputSimulationService = MixedRealityToolkit.Instance.GetService<InputSimulationService>();
            inputSimulationService.UserInputEnabled = false;
            SimulatedHandPose gesturePose = SimulatedHandPose.GetGesturePose(SimulatedHandPose.GestureId.Open);
            var handOpenPose = GenerateHandPose(SimulatedHandPose.GestureId.Open, Handedness.Right, Vector3.forward * 0.1f);
            inputSimulationService.HandDataRight.Update(true, false, handOpenPose);
            yield return null;
            // Gaze cursor should not be visible
            Assert.IsFalse(MixedRealityToolkit.InputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should not be visible when one articulated hand is up");
            inputSimulationService.HandDataRight.Update(false, false, handOpenPose);
            yield return null;

            // Say "select" to make gaze cursor active again
            // Really we need to tear down the scene and create it again but MRTK doesn't support that yet
            var gazeInputSource = MixedRealityToolkit.InputSystem.DetectedInputSources.Where(x => x.SourceName.Equals("Gaze")).First();
            MixedRealityToolkit.InputSystem.RaiseSpeechCommandRecognized(gazeInputSource, RecognitionConfidenceLevel.High, new System.TimeSpan(), System.DateTime.Now, new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
            yield return null;
            Assert.IsTrue(MixedRealityToolkit.InputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should be visible after select command");
        }

        private SimulatedHandData.HandJointDataGenerator GenerateHandPose(SimulatedHandPose.GestureId gesture, Handedness handedness, Vector3 screenPosition)
        {
            return (jointsOut) =>
            {
                SimulatedHandPose gesturePose = SimulatedHandPose.GetGesturePose(gesture);
                Quaternion rotation = Quaternion.identity;
                Vector3 position = CameraCache.Main.ScreenToWorldPoint(screenPosition);
                gesturePose.ComputeJointPositions(handedness, rotation, position, jointsOut);
            };
        }
    }
}
#endif