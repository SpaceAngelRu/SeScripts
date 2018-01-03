using System.Collections.Generic;
using System.Text;
using Sandbox.Game.Entities;
using Sandbox.Game.World;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;
using VRageMath;

namespace SeScripts.Dron
{
    public sealed class Program : MyGridProgram
    {
        #region Const

        //Текстовая панель
        private const string TEXT_PANEL = "Текстовая панель";
        private const string CAM_NAME = "Зрительная камера";
        private const string ROTOR_NAME = "Ротор камеры";

        #endregion

        #region Private fields

        private IMyCameraBlock camera;

        //    private IMyTextPanel lcd;
        private bool firstrun = true;

        private MyDetectedEntityInfo info;
        private float PITCH = 0;
        private StringBuilder sb = new StringBuilder();
        private double SCAN_DISTANCE = 100;
        private float YAW = 0;
        private List<IMyCubeBlock> blocks = new List<IMyCubeBlock>();
        private IMyMotorStator rotor;
        #endregion

        #region Construct

        public Program()
        {
            PrintForAllTerminals("");

            // Configure this program to run both Once and every 100 update ticks
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            camera = GridTerminalSystem.GetBlockWithName(CAM_NAME) as IMyCameraBlock;
            if (camera != null)
                camera.EnableRaycast = true;
            else
            {
                PrintForAllTerminals($"Не удалось найти камеру с именем {CAM_NAME}");
            }

            rotor = GridTerminalSystem.GetBlockWithName(ROTOR_NAME) as IMyMotorStator;
            if (rotor == null)
            {
                PrintForAllTerminals($"Не удалось ротор с именем {ROTOR_NAME}");
            }
        }

        #endregion

        #region Public

        public void Main(string argument)
        {


            //            if (rotor == null)
            //            {
            //                return;
            //            }

            //            MatrixD m = MatrixD.Identity;
            //
            var grid = this.Me.CubeGrid;
            //
            //            var dd = grid.GetCubeBlock()
            //
            //
            var min = grid.Min;
            var max = grid.Max;
            //
            var size = max - min + Vector3I.One;
            var sizeInMeters = size * grid.GridSize;
            //
            if (camera.CanScan(SCAN_DISTANCE))
                info = camera.Raycast(SCAN_DISTANCE, PITCH, YAW);

            var dist = Vector3.Distance(Me.GetPosition(), info.Position);

            sb.Clear();
            sb.Append($"dist: {dist}");
            sb.AppendLine();
            sb.Append($"sizeInMeters: {sizeInMeters}");
            sb.AppendLine();

            if (rotor != null)
            {
                sb.Append($"angel: {rotor.Angle}");
                sb.AppendLine();
            }

            //            Quaternion qu;
            //            rotor.Orientation.GetQuaternion(out qu);
            //
            //            Vector3 axis;
            //            float angle;
            //            qu.GetAxisAngle(out axis, out angle);
            //
            //            sb.Append($"axis: {axis}");
            //            sb.AppendLine();
            //
            //            sb.Append($"angle: {angle}");
            //            sb.AppendLine();

            //            sb.Append($"min: {grid.GridSize}");
            //            sb.AppendLine();
            //            sb.Append($"Grid size: {size}");
            //            sb.AppendLine();
            //            sb.Append($"Grid volume: {size.Volume()}");
            //            sb.AppendLine();
            //            sb.Append($"Size: {sizeInMeters} m^3");
            //            sb.AppendLine();
            //            sb.Append($"Volume: {sizeInMeters} m^3");
            //            sb.AppendLine();
            //            sb.Append($"blocks: {blocks.Count}");
            //            sb.AppendLine();
            //            sb.Append("EntityID: " + info.EntityId);
            //            sb.AppendLine();
            //            sb.Append("Name: " + info.Name);
            //            sb.AppendLine();
            //            sb.Append("Type: " + info.Type);
            //            sb.AppendLine();
            //            sb.Append("Velocity: " + info.Velocity.ToString("0.000"));
            //            sb.AppendLine();
            //            sb.Append("Relationship: " + info.Relationship);
            //            sb.AppendLine();
            //            sb.Append("Size: " + info.BoundingBox.Size.ToString("0.000"));
            //            sb.AppendLine();
            //            sb.Append("Position: " + info.Position.ToString("0.000"));

            if (info.HitPosition.HasValue)
            {
                //                sb.AppendLine();
                //                sb.Append("Hit: " + info.HitPosition.Value.ToString("0.000"));
                //                sb.AppendLine(); 
                //                sb.Append("Distance: " + Vector3D.Distance(camera.GetPosition(), info.HitPosition.Value).ToString("0.00"));
            }

            sb.AppendLine();
            sb.Append("Range: " + camera.AvailableScanRange.ToString());

            PrintForAllTerminals(sb.ToString());
        }

        //        private string GetDirectionString()
        //        {
        //            var cockpit = MySession.Static.ControlledEntity as MyCockpit;
        //            if (cockpit != null)
        //            {
        //                Quaternion cockpitOrientation;
        //                cockpit.Orientation.GetQuaternion(out cockpitOrientation);
        //                var thrustDir = Vector3I.Transform(ThrustForwardVector, Quaternion.Inverse(cockpitOrientation));
        //                if (thrustDir.X == 1)
        //                    return MyTexts.GetString(MyCommonTexts.Thrust_Left);
        //                else if (thrustDir.X == -1)
        //                    return MyTexts.GetString(MyCommonTexts.Thrust_Right);
        //                else if (thrustDir.Y == 1)
        //                    return MyTexts.GetString(MyCommonTexts.Thrust_Down);
        //                else if (thrustDir.Y == -1)
        //                    return MyTexts.GetString(MyCommonTexts.Thrust_Up);
        //                else if (thrustDir.Z == 1)
        //                    return MyTexts.GetString(MyCommonTexts.Thrust_Forward);
        //                else if (thrustDir.Z == -1)
        //                    return MyTexts.GetString(MyCommonTexts.Thrust_Back);
        //            }
        //            return null;
        //        }

        #endregion

        #region Private methods

        private void PrintForAllTerminals(string str, bool append = false)
        {
            List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType(textPanels);

            foreach (var textPanel in textPanels)
            {
                if (textPanel.CustomName.Contains(TEXT_PANEL))
                {
                    textPanel.WritePublicText(str, append);
                }
            }
        }

        #endregion
    }
}